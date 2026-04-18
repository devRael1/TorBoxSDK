using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TorBoxSDK.Http.Validation;

namespace TorBoxSDK.Http.Handlers;

/// <summary>
/// HTTP message handler that injects the TorBox API key as a Bearer token
/// into the <c>Authorization</c> header of every outgoing request.
/// </summary>
internal sealed class AuthHandler : DelegatingHandler
{
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthHandler"/> class
    /// for standalone usage with a direct API key.
    /// </summary>
    /// <param name="apiKey">The TorBox API key.</param>
    internal AuthHandler(string apiKey)
    {
        _apiKey = apiKey ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthHandler"/> class
    /// for DI usage with <see cref="IOptions{TorBoxClientOptions}"/>.
    /// </summary>
    /// <param name="options">The options containing the API key.</param>
    [ActivatorUtilitiesConstructor]
    public AuthHandler(IOptions<TorBoxClientOptions> options)
    {
        Guard.ThrowIfNull(options);
        _apiKey = options.Value.ApiKey;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
