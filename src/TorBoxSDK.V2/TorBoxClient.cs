using TorBoxSDK.Http;
using TorBoxSDK.Http.Handlers;
using TorBoxSDK.Main;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;

namespace TorBoxSDK;

/// <summary>
/// Provides the root navigation surface for the TorBox API families.
/// </summary>
/// <remarks>
/// Direct construction creates and owns one HTTP pipeline for each API family. Clients created
/// through dependency injection use factory-managed pipelines and do not dispose them.
/// </remarks>
public sealed class TorBoxClient : ITorBoxClient
{
	private readonly HttpClient _mainHttpClient;
	private readonly HttpClient _searchHttpClient;
	private readonly HttpClient _relayHttpClient;
	private readonly bool _ownsHttpClients;
	private bool _disposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="TorBoxClient"/> class with direct SDK-owned HTTP pipelines.
	/// </summary>
	/// <param name="options">The API key, family base URLs, and timeout to validate before construction.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">Thrown when an option value is invalid.</exception>
	public TorBoxClient(TorBoxClientOptions options)
		: this(CreateDirectClientSet(options))
	{
	}

	internal TorBoxClient(
		HttpClient mainHttpClient,
		HttpClient searchHttpClient,
		HttpClient relayHttpClient,
		ITorBoxApiTransport transport,
		bool ownsHttpClients)
	{
		if (mainHttpClient is null)
		{
			throw new ArgumentNullException(nameof(mainHttpClient));
		}

		if (searchHttpClient is null)
		{
			throw new ArgumentNullException(nameof(searchHttpClient));
		}

		if (relayHttpClient is null)
		{
			throw new ArgumentNullException(nameof(relayHttpClient));
		}

		if (transport is null)
		{
			throw new ArgumentNullException(nameof(transport));
		}

		_mainHttpClient = mainHttpClient;
		_searchHttpClient = searchHttpClient;
		_relayHttpClient = relayHttpClient;
		_ownsHttpClients = ownsHttpClients;

		try
		{
			Main = new MainApiClient(mainHttpClient, transport);
			Search = new SearchApiClient(searchHttpClient, transport);
			Relay = new RelayApiClient(relayHttpClient, transport);
		}
		catch
		{
			if (ownsHttpClients)
			{
				DisposeOwnedHttpClients();
			}

			throw;
		}
	}

	private TorBoxClient(DirectClientSet directClientSet)
		: this(
			directClientSet.MainHttpClient,
			directClientSet.SearchHttpClient,
			directClientSet.RelayHttpClient,
			directClientSet.Transport,
			ownsHttpClients: true)
	{
	}

	/// <summary>
	/// Gets the Main API family and its resource clients.
	/// </summary>
	public IMainApiClient Main { get; }

	/// <summary>
	/// Gets the Search API family.
	/// </summary>
	public ISearchApiClient Search { get; }

	/// <summary>
	/// Gets the Relay API family.
	/// </summary>
	public IRelayApiClient Relay { get; }

	/// <summary>
	/// Disposes the HTTP pipelines owned by a directly constructed client.
	/// </summary>
	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;

		if (_ownsHttpClients)
		{
			DisposeOwnedHttpClients();
		}

		GC.SuppressFinalize(this);
	}

	private static DirectClientSet CreateDirectClientSet(TorBoxClientOptions options)
	{
		if (options is null)
		{
			throw new ArgumentNullException(nameof(options));
		}

		ValidatedTorBoxClientOptions validatedOptions = options.ValidateAndNormalize();
		HttpClient? mainHttpClient = null;
		HttpClient? searchHttpClient = null;
		HttpClient? relayHttpClient = null;

		try
		{
			mainHttpClient = CreateHttpClient(
				validatedOptions.MainApiBaseUri,
				validatedOptions.ApiKey,
				validatedOptions.Timeout);
			searchHttpClient = CreateHttpClient(
				validatedOptions.SearchApiBaseUri,
				validatedOptions.ApiKey,
				validatedOptions.Timeout);
			relayHttpClient = CreateHttpClient(
				validatedOptions.RelayApiBaseUri,
				validatedOptions.ApiKey,
				validatedOptions.Timeout);

			return new DirectClientSet(
				mainHttpClient,
				searchHttpClient,
				relayHttpClient,
				new TorBoxApiTransport());
		}
		catch
		{
			mainHttpClient?.Dispose();
			searchHttpClient?.Dispose();
			relayHttpClient?.Dispose();
			throw;
		}
	}

	private static HttpClient CreateHttpClient(Uri baseAddress, string apiKey, TimeSpan timeout)
	{
		AuthHandler authHandler = new(apiKey)
		{
			InnerHandler = TorBoxHttpClientHandlerFactory.Create(),
		};
		HttpClient httpClient = new(authHandler, disposeHandler: true);

		try
		{
			httpClient.BaseAddress = baseAddress;
			httpClient.Timeout = timeout;
			return httpClient;
		}
		catch
		{
			httpClient.Dispose();
			throw;
		}
	}

	private void DisposeOwnedHttpClients()
	{
		_mainHttpClient.Dispose();

		if (!ReferenceEquals(_searchHttpClient, _mainHttpClient))
		{
			_searchHttpClient.Dispose();
		}

		if (!ReferenceEquals(_relayHttpClient, _mainHttpClient)
			&& !ReferenceEquals(_relayHttpClient, _searchHttpClient))
		{
			_relayHttpClient.Dispose();
		}
	}

	private sealed class DirectClientSet
	{
		internal DirectClientSet(
			HttpClient mainHttpClient,
			HttpClient searchHttpClient,
			HttpClient relayHttpClient,
			ITorBoxApiTransport transport)
		{
			MainHttpClient = mainHttpClient;
			SearchHttpClient = searchHttpClient;
			RelayHttpClient = relayHttpClient;
			Transport = transport;
		}

		internal HttpClient MainHttpClient { get; }

		internal HttpClient SearchHttpClient { get; }

		internal HttpClient RelayHttpClient { get; }

		internal ITorBoxApiTransport Transport { get; }
	}
}
