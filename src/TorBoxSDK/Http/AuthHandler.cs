using Microsoft.Extensions.Options;

namespace TorBoxSDK.Http;

/// <summary>
/// HTTP message handler that injects the TorBox API key as a Bearer token
/// into the <c>Authorization</c> header of every outgoing request.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthHandler"/> class.
/// </remarks>
/// <param name="options">The options containing the API key.</param>
internal sealed class AuthHandler(IOptions<TorBoxClientOptions> options) : DelegatingHandler
{
    private readonly IOptions<TorBoxClientOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string apiKey = _options.Value.ApiKey;

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
