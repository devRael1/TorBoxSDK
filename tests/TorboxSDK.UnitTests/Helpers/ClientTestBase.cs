using System.Net;
using System.Reflection;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorboxSDK.UnitTests.Helpers;

internal static class ClientTestBase
{
	private static readonly Dictionary<Type, string> _baseAddresses = new()
	{
		[typeof(SearchApiClient)] = "https://search-api.torbox.app/",
		[typeof(RelayApiClient)] = "https://relay.torbox.app/v1/",
	};

	internal static (TClient client, MockHttpMessageHandler handler) CreateClient<TClient>(
		string json,
		HttpStatusCode statusCode = HttpStatusCode.OK,
		string? baseAddress = null)
		where TClient : class
	{
		string resolvedBaseAddress = baseAddress
			?? (_baseAddresses.TryGetValue(typeof(TClient), out string? mapped) ? mapped : "https://api.torbox.app/v1/api/");

		MockHttpMessageHandler handler = new(json, statusCode);
		HttpClient httpClient = new(handler) { BaseAddress = new Uri(resolvedBaseAddress) };

		ConstructorInfo ctor = typeof(TClient)
			.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.Where(c =>
			{
				ParameterInfo[] ps = c.GetParameters();
				return ps.Length > 0 && ps[0].ParameterType == typeof(HttpClient);
			})
			.OrderBy(c => c.GetParameters().Length)
			.FirstOrDefault()
			?? throw new InvalidOperationException($"No constructor accepting HttpClient found on {typeof(TClient).Name}.");

		ParameterInfo[] parameters = ctor.GetParameters();
		object[] args = new object[parameters.Length];
		args[0] = httpClient;
		for (int i = 1; i < parameters.Length; i++)
		{
			if (parameters[i].ParameterType == typeof(string))
			{
				// Use a non-empty placeholder for string params that represent tokens/keys,
				// but keep empty for URL params so the HttpClient.BaseAddress fallback is used.
				args[i] = parameters[i].Name is "baseUrl" ? string.Empty : "test-value";
			}
			else if (parameters[i].ParameterType.IsValueType)
			{
				// Value types always produce a non-null default via Activator.CreateInstance.
				args[i] = Activator.CreateInstance(parameters[i].ParameterType)!;
			}
			else
			{
				throw new InvalidOperationException(
					$"Unsupported constructor parameter type '{parameters[i].ParameterType.Name}' " +
					$"at position {i} on {typeof(TClient).Name}. Add explicit handling in CreateClient.");
			}
		}

		TClient client = (TClient?)ctor.Invoke(args)
			?? throw new InvalidOperationException($"Failed to create instance of {typeof(TClient).Name}.");

		return (client, handler);
	}
}
