using System.Text;

namespace TorBoxSDK.Http;

internal static class QueryStringBuilder
{
	internal static string Build(params (string Name, string? Value)[] parameters)
	{
		if (parameters is null)
		{
			throw new ArgumentNullException(nameof(parameters));
		}

		StringBuilder builder = new();
		bool hasValue = false;

		foreach ((string name, string? value) in parameters)
		{
			if (name is null)
			{
				throw new ArgumentNullException(nameof(parameters), "Query parameter names cannot be null.");
			}

			if (value is null)
			{
				continue;
			}

			builder.Append(hasValue ? '&' : '?');
			builder.Append(Uri.EscapeDataString(name));
			builder.Append('=');
			builder.Append(Uri.EscapeDataString(value));
			hasValue = true;
		}

		return builder.ToString();
	}
}
