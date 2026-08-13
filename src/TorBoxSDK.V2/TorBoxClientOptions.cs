namespace TorBoxSDK;

/// <summary>
/// Provides mutable configuration values used to construct a <see cref="TorBoxClient"/>.
/// </summary>
/// <remarks>
/// The options object is configuration-bindable. Its values are validated and normalized into an
/// internal immutable snapshot when a client is constructed.
/// </remarks>
public sealed class TorBoxClientOptions
{
	private static readonly TimeSpan _maximumHttpClientTimeout = TimeSpan.FromMilliseconds(int.MaxValue);

	/// <summary>
	/// Gets or sets the TorBox API key used by the authentication handler.
	/// </summary>
	public string? ApiKey { get; set; }

	/// <summary>
	/// Gets or sets the resolved Main API base URL.
	/// </summary>
	public string? MainApiBaseUrl { get; set; } = "https://api.torbox.app/v1/api/";

	/// <summary>
	/// Gets or sets the Search API base URL.
	/// </summary>
	public string? SearchApiBaseUrl { get; set; } = "https://search-api.torbox.app/";

	/// <summary>
	/// Gets or sets the resolved Relay API base URL.
	/// </summary>
	public string? RelayApiBaseUrl { get; set; } = "https://relay.torbox.app/v1/";

	/// <summary>
	/// Gets or sets the timeout applied to each family HTTP client.
	/// </summary>
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

	internal ValidatedTorBoxClientOptions ValidateAndNormalize()
	{
		string apiKey = ApiKey ?? throw new ArgumentException(
			"A non-blank TorBox API key is required.",
			nameof(ApiKey));

		if (string.IsNullOrWhiteSpace(apiKey))
		{
			throw new ArgumentException("A non-blank TorBox API key is required.", nameof(ApiKey));
		}

		if (Timeout <= TimeSpan.Zero || Timeout > _maximumHttpClientTimeout)
		{
			throw new ArgumentException(
				"The TorBox HTTP timeout must be greater than zero and cannot exceed the maximum supported by HttpClient.",
				nameof(Timeout));
		}

		return new ValidatedTorBoxClientOptions(
			apiKey,
			ValidateAndNormalizeBaseUrl(MainApiBaseUrl, nameof(MainApiBaseUrl)),
			ValidateAndNormalizeBaseUrl(SearchApiBaseUrl, nameof(SearchApiBaseUrl)),
			ValidateAndNormalizeBaseUrl(RelayApiBaseUrl, nameof(RelayApiBaseUrl)),
			Timeout);
	}

	private static Uri ValidateAndNormalizeBaseUrl(string? value, string parameterName)
	{
		string baseUrl = value ?? throw new ArgumentException(
			"A TorBox family base URL is required.",
			parameterName);

		if (string.IsNullOrWhiteSpace(baseUrl))
		{
			throw new ArgumentException("A TorBox family base URL is required.", parameterName);
		}

		if (!baseUrl.EndsWith("/", StringComparison.Ordinal))
		{
			throw new ArgumentException("A TorBox family base URL must end with a trailing slash.", parameterName);
		}

		if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? parsedUri)
			|| (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
		{
			throw new ArgumentException("A TorBox family base URL must be an absolute HTTP or HTTPS URI.", parameterName);
		}

		if (!string.IsNullOrEmpty(parsedUri.Query) || !string.IsNullOrEmpty(parsedUri.Fragment))
		{
			throw new ArgumentException("A TorBox family base URL cannot include a query string or fragment.", parameterName);
		}

		return new Uri(parsedUri.AbsoluteUri, UriKind.Absolute);
	}
}

internal sealed class ValidatedTorBoxClientOptions
{
	internal ValidatedTorBoxClientOptions(
		string apiKey,
		Uri mainApiBaseUri,
		Uri searchApiBaseUri,
		Uri relayApiBaseUri,
		TimeSpan timeout)
	{
		ApiKey = apiKey;
		MainApiBaseUri = mainApiBaseUri;
		SearchApiBaseUri = searchApiBaseUri;
		RelayApiBaseUri = relayApiBaseUri;
		Timeout = timeout;
	}

	internal string ApiKey { get; }

	internal Uri MainApiBaseUri { get; }

	internal Uri SearchApiBaseUri { get; }

	internal Uri RelayApiBaseUri { get; }

	internal TimeSpan Timeout { get; }
}
