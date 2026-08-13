using System.Net.Http.Headers;

namespace TorBoxSDK.Http.Handlers;

internal sealed class AuthHandler : DelegatingHandler
{
	private readonly string? _apiKey;

	internal AuthHandler(string apiKey)
	{
		_apiKey = apiKey;
	}

	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		if (string.IsNullOrWhiteSpace(_apiKey))
		{
			throw new InvalidOperationException("A non-blank TorBox API key is required before sending a request.");
		}

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		return base.SendAsync(request, cancellationToken);
	}
}
