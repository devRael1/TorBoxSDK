using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using TorBoxSDK.DependencyInjection;
using TorBoxSDK.Main;
using TorBoxSDK.Main.General;
using TorBoxSDK.Main.Integrations;
using TorBoxSDK.Main.Notifications;
using TorBoxSDK.Main.Queued;
using TorBoxSDK.Main.Rss;
using TorBoxSDK.Main.Stream;
using TorBoxSDK.Main.Torrents;
using TorBoxSDK.Main.Usenet;
using TorBoxSDK.Main.User;
using TorBoxSDK.Main.Vendors;
using TorBoxSDK.Main.WebDownloads;
using TorBoxSDK.Relay;
using TorBoxSDK.Search;
using TorBoxSDK.V2.Testing;

namespace TorBoxSDK.V2.UnitTests.DependencyInjection;

public sealed class TorBoxServiceCollectionExtensionsTests
{
	private const string MainHttpClientName = "TorBoxSDK.Main";
	private const string SearchHttpClientName = "TorBoxSDK.Search";
	private const string RelayHttpClientName = "TorBoxSDK.Relay";

	[Fact]
	public void AddTorBox_WithDelegate_RegistersOnlyTransientRootClient()
	{
		// Arrange
		ServiceCollection services = new();
		IServiceCollection returnedServices = services.AddTorBox(static options => options.ApiKey = "test-key");
		ConfigureSyntheticPrimaryHandlers(services);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		using ITorBoxClient firstClient = provider.GetRequiredService<ITorBoxClient>();
		using ITorBoxClient secondClient = provider.GetRequiredService<ITorBoxClient>();

		// Assert
		Assert.Same(services, returnedServices);
		Assert.NotSame(firstClient, secondClient);
		Assert.Null(provider.GetService<TorBoxClient>());
		Assert.Null(provider.GetService<IMainApiClient>());
		Assert.Null(provider.GetService<ISearchApiClient>());
		Assert.Null(provider.GetService<IRelayApiClient>());
	}

	[Fact]
	public void AddTorBox_WithNullDelegate_ThrowsArgumentNullException()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
			() => services.AddTorBox((Action<TorBoxClientOptions>)null!));

		// Assert
		Assert.Equal("configure", exception.ParamName);
	}

	[Fact]
	public void AddTorBox_WithNullConfiguration_ThrowsArgumentNullException()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
			() => services.AddTorBox((IConfiguration)null!));

		// Assert
		Assert.Equal("configuration", exception.ParamName);
	}

	[Fact]
	public void AddTorBox_WhenCalledMoreThanOnce_ComposesOptionsWithoutDuplicateRootRegistration()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddTorBox(options =>
		{
			options.ApiKey = "test-key";
			options.MainApiBaseUrl = "https://first-main.example.test/v1/api/";
		});
		services.AddTorBox(options => options.MainApiBaseUrl = "https://second-main.example.test/v1/api/");
		RecordingHttpMessageHandler mainHandler = CreateOkHandler();
		ConfigurePrimaryHandler(services, MainHttpClientName, mainHandler);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		ServiceDescriptor[] rootClientDescriptors = services
			.Where(static descriptor => descriptor.ServiceType == typeof(ITorBoxClient))
			.ToArray();
		IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
		using HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);

		// Assert
		Assert.Single(rootClientDescriptors);
		Assert.Equal(new Uri("https://second-main.example.test/v1/api/"), mainHttpClient.BaseAddress);
	}

	[Fact]
	public async Task AddTorBox_WithConfiguration_BindsOnlyTheTorBoxSection()
	{
		// Arrange
		RecordingHttpMessageHandler mainHandler = RecordingHttpMessageHandler.FromResponse(
			static (_, _) => new HttpResponseMessage(HttpStatusCode.OK));
		IConfiguration configuration = CreateConfiguration(
			("ApiKey", "outside-section-key"),
			("TorBox:ApiKey", "configuration-key"),
			("TorBox:MainApiBaseUrl", "https://configured-main.example.test/v1/api/"));
		ServiceCollection services = new();
		services.AddTorBox(configuration);
		ConfigurePrimaryHandler(services, MainHttpClientName, mainHandler);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
		using HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);
		using HttpResponseMessage response = await mainHttpClient.GetAsync("probe", CancellationToken.None);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal(new Uri("https://configured-main.example.test/v1/api/"), mainHttpClient.BaseAddress);
		Assert.Equal(new Uri("https://configured-main.example.test/v1/api/probe"), mainHandler.LastRequestUri);
		Assert.Equal(["Bearer configuration-key"], mainHandler.LastAuthorizationHeaderValues);
	}

	[Fact]
	public void AddTorBox_DoesNotRegisterFamilyOrResourceInterfaces()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddTorBox(static options => options.ApiKey = "test-key");
		ConfigureSyntheticPrimaryHandlers(services);
		using ServiceProvider provider = services.BuildServiceProvider();
		Type[] clientTypes =
		[
			typeof(IMainApiClient),
			typeof(ISearchApiClient),
			typeof(IRelayApiClient),
			typeof(IGeneralClient),
			typeof(ITorrentsClient),
			typeof(IUsenetClient),
			typeof(IWebDownloadsClient),
			typeof(IUserClient),
			typeof(INotificationsClient),
			typeof(IRssClient),
			typeof(IStreamClient),
			typeof(IIntegrationsClient),
			typeof(IVendorsClient),
			typeof(IQueuedClient),
		];

		// Act
		IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
		using HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);
		using HttpClient searchHttpClient = httpClientFactory.CreateClient(SearchHttpClientName);
		using HttpClient relayHttpClient = httpClientFactory.CreateClient(RelayHttpClientName);
		object?[] resolvedClients = clientTypes.Select(provider.GetService).ToArray();

		// Assert
		Assert.All(resolvedClients, Assert.Null);
	}

	[Fact]
	public async Task AddTorBox_WithDelegate_UsesSeparateNamedPipelinesForEachFamily()
	{
		// Arrange
		RecordingHttpMessageHandler mainHandler = RecordingHttpMessageHandler.FromResponse(
			static (_, _) => new HttpResponseMessage(HttpStatusCode.OK));
		RecordingHttpMessageHandler searchHandler = RecordingHttpMessageHandler.FromResponse(
			static (_, _) => new HttpResponseMessage(HttpStatusCode.OK));
		RecordingHttpMessageHandler relayHandler = RecordingHttpMessageHandler.FromResponse(
			static (_, _) => new HttpResponseMessage(HttpStatusCode.OK));
		ServiceCollection services = new();
		services.AddTorBox(options =>
		{
			options.ApiKey = "test-key";
			options.MainApiBaseUrl = "https://main.example.test/v1/api/";
			options.SearchApiBaseUrl = "https://search.example.test/";
			options.RelayApiBaseUrl = "https://relay.example.test/v1/";
		});
		ConfigurePrimaryHandler(services, MainHttpClientName, mainHandler);
		ConfigurePrimaryHandler(services, SearchHttpClientName, searchHandler);
		ConfigurePrimaryHandler(services, RelayHttpClientName, relayHandler);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
		using HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);
		using HttpClient searchHttpClient = httpClientFactory.CreateClient(SearchHttpClientName);
		using HttpClient relayHttpClient = httpClientFactory.CreateClient(RelayHttpClientName);
		using HttpResponseMessage mainResponse = await mainHttpClient.GetAsync("probe", CancellationToken.None);
		using HttpResponseMessage searchResponse = await searchHttpClient.GetAsync("probe", CancellationToken.None);
		using HttpResponseMessage relayResponse = await relayHttpClient.GetAsync("probe", CancellationToken.None);

		// Assert
		Assert.Equal(HttpStatusCode.OK, mainResponse.StatusCode);
		Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
		Assert.Equal(HttpStatusCode.OK, relayResponse.StatusCode);
		Assert.Equal(new Uri("https://main.example.test/v1/api/"), mainHttpClient.BaseAddress);
		Assert.Equal(new Uri("https://search.example.test/"), searchHttpClient.BaseAddress);
		Assert.Equal(new Uri("https://relay.example.test/v1/"), relayHttpClient.BaseAddress);
		Assert.Equal(new Uri("https://main.example.test/v1/api/probe"), mainHandler.LastRequestUri);
		Assert.Equal(new Uri("https://search.example.test/probe"), searchHandler.LastRequestUri);
		Assert.Equal(new Uri("https://relay.example.test/v1/probe"), relayHandler.LastRequestUri);
		Assert.Equal(["Bearer test-key"], mainHandler.LastAuthorizationHeaderValues);
		Assert.Equal(["Bearer test-key"], searchHandler.LastAuthorizationHeaderValues);
		Assert.Equal(["Bearer test-key"], relayHandler.LastAuthorizationHeaderValues);
		Assert.NotSame(mainHandler, searchHandler);
		Assert.NotSame(mainHandler, relayHandler);
		Assert.NotSame(searchHandler, relayHandler);
		Assert.Equal(1, mainHandler.SendCount);
		Assert.Equal(1, searchHandler.SendCount);
		Assert.Equal(1, relayHandler.SendCount);
	}

	[Fact]
	public void AddTorBox_WithWhitespaceDelegateApiKey_ThrowsBeforeCreatingOrSendingNamedPipelines()
	{
		// Arrange
		CountingPrimaryHandlerFactory primaryHandlerFactory = new();
		ServiceCollection services = new();
		services.AddTorBox(static options => options.ApiKey = " ");
		ConfigurePrimaryHandlerFactory(services, MainHttpClientName, primaryHandlerFactory.Create);
		ConfigurePrimaryHandlerFactory(services, SearchHttpClientName, primaryHandlerFactory.Create);
		ConfigurePrimaryHandlerFactory(services, RelayHttpClientName, primaryHandlerFactory.Create);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(
			() => provider.GetRequiredService<ITorBoxClient>());

		// Assert
		Assert.Equal("ApiKey", exception.ParamName);
		Assert.Equal(0, primaryHandlerFactory.CreatedHandlerCount);
		Assert.Equal(0, primaryHandlerFactory.SentRequestCount);
	}

	[Fact]
	public void AddTorBox_WithInvalidDelegateBaseUrl_ThrowsWithoutExposingApiKeyOrCreatingPipelines()
	{
		// Arrange
		const string ApiKey = "not-for-exception-output";
		CountingPrimaryHandlerFactory primaryHandlerFactory = new();
		ServiceCollection services = new();
		services.AddTorBox(options =>
		{
			options.ApiKey = ApiKey;
			options.MainApiBaseUrl = "not-an-absolute-url/";
		});
		ConfigurePrimaryHandlerFactory(services, MainHttpClientName, primaryHandlerFactory.Create);
		ConfigurePrimaryHandlerFactory(services, SearchHttpClientName, primaryHandlerFactory.Create);
		ConfigurePrimaryHandlerFactory(services, RelayHttpClientName, primaryHandlerFactory.Create);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(
			() => provider.GetRequiredService<ITorBoxClient>());

		// Assert
		Assert.Equal("MainApiBaseUrl", exception.ParamName);
		Assert.DoesNotContain(ApiKey, exception.Message, StringComparison.Ordinal);
		Assert.Equal(0, primaryHandlerFactory.CreatedHandlerCount);
		Assert.Equal(0, primaryHandlerFactory.SentRequestCount);
	}

	[Fact]
	public void AddTorBox_WithConfigurationMissingTorBoxApiKey_ThrowsBeforeCreatingOrSendingNamedPipelines()
	{
		// Arrange
		CountingPrimaryHandlerFactory primaryHandlerFactory = new();
		IConfiguration configuration = CreateConfiguration(("ApiKey", "outside-section-key"));
		ServiceCollection services = new();
		services.AddTorBox(configuration);
		ConfigurePrimaryHandlerFactory(services, MainHttpClientName, primaryHandlerFactory.Create);
		ConfigurePrimaryHandlerFactory(services, SearchHttpClientName, primaryHandlerFactory.Create);
		ConfigurePrimaryHandlerFactory(services, RelayHttpClientName, primaryHandlerFactory.Create);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(
			() => provider.GetRequiredService<ITorBoxClient>());

		// Assert
		Assert.Equal("ApiKey", exception.ParamName);
		Assert.Equal(0, primaryHandlerFactory.CreatedHandlerCount);
		Assert.Equal(0, primaryHandlerFactory.SentRequestCount);
	}

	[Fact]
	public async Task AddTorBox_WhenRootClientIsDisposed_LeavesFactoryManagedPipelinesUsable()
	{
		// Arrange
		TrackingHttpMessageHandler mainHandler = new();
		TrackingHttpMessageHandler searchHandler = new();
		TrackingHttpMessageHandler relayHandler = new();
		ServiceCollection services = new();
		services.AddTorBox(static options => options.ApiKey = "test-key");
		ConfigurePrimaryHandler(services, MainHttpClientName, mainHandler);
		ConfigurePrimaryHandler(services, SearchHttpClientName, searchHandler);
		ConfigurePrimaryHandler(services, RelayHttpClientName, relayHandler);
		using ServiceProvider provider = services.BuildServiceProvider();
		ITorBoxClient rootClient = provider.GetRequiredService<ITorBoxClient>();
		IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

		// Act
		rootClient.Dispose();
		using HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);
		using HttpClient searchHttpClient = httpClientFactory.CreateClient(SearchHttpClientName);
		using HttpClient relayHttpClient = httpClientFactory.CreateClient(RelayHttpClientName);
		using HttpResponseMessage response = await mainHttpClient.GetAsync("probe", CancellationToken.None);
		using HttpResponseMessage searchResponse = await searchHttpClient.GetAsync("probe", CancellationToken.None);
		using HttpResponseMessage relayResponse = await relayHttpClient.GetAsync("probe", CancellationToken.None);

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
		Assert.Equal(HttpStatusCode.OK, relayResponse.StatusCode);
		Assert.Equal(1, mainHandler.SendCount);
		Assert.Equal(1, searchHandler.SendCount);
		Assert.Equal(1, relayHandler.SendCount);
		Assert.False(mainHandler.IsDisposed);
		Assert.False(searchHandler.IsDisposed);
		Assert.False(relayHandler.IsDisposed);
	}

	[Fact]
	public void AddTorBox_WithDelegateDefaults_ConfiguresEachNamedFamilyBaseAddress()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddTorBox(static options => options.ApiKey = "test-key");
		ConfigureSyntheticPrimaryHandlers(services);
		using ServiceProvider provider = services.BuildServiceProvider();

		// Act
		IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
		using HttpClient mainHttpClient = httpClientFactory.CreateClient(MainHttpClientName);
		using HttpClient searchHttpClient = httpClientFactory.CreateClient(SearchHttpClientName);
		using HttpClient relayHttpClient = httpClientFactory.CreateClient(RelayHttpClientName);

		// Assert
		Assert.Equal(new Uri("https://api.torbox.app/v1/api/"), mainHttpClient.BaseAddress);
		Assert.Equal(new Uri("https://search-api.torbox.app/"), searchHttpClient.BaseAddress);
		Assert.Equal(new Uri("https://relay.torbox.app/v1/"), relayHttpClient.BaseAddress);
	}

	private static IConfiguration CreateConfiguration(params (string Key, string? Value)[] values)
		=> new DictionaryConfiguration(values);

	private static void ConfigurePrimaryHandler(
		IServiceCollection services,
		string clientName,
		HttpMessageHandler handler)
	{
		ConfigurePrimaryHandlerFactory(services, clientName, () => handler);
	}

	private static void ConfigurePrimaryHandlerFactory(
		IServiceCollection services,
		string clientName,
		Func<HttpMessageHandler> handlerFactory)
	{
		services
			.AddHttpClient(clientName)
			.ConfigurePrimaryHttpMessageHandler(handlerFactory);
	}

	private static void ConfigureSyntheticPrimaryHandlers(IServiceCollection services)
	{
		ConfigurePrimaryHandler(services, MainHttpClientName, CreateOkHandler());
		ConfigurePrimaryHandler(services, SearchHttpClientName, CreateOkHandler());
		ConfigurePrimaryHandler(services, RelayHttpClientName, CreateOkHandler());
	}

	private static RecordingHttpMessageHandler CreateOkHandler() => RecordingHttpMessageHandler.FromResponse(
		static (_, _) => new HttpResponseMessage(HttpStatusCode.OK));

	private sealed class CountingPrimaryHandlerFactory
	{
		private readonly List<TrackingHttpMessageHandler> _createdHandlers = [];

		internal int CreatedHandlerCount => _createdHandlers.Count;

		internal int SentRequestCount => _createdHandlers.Sum(static handler => handler.SendCount);

		internal HttpMessageHandler Create()
		{
			TrackingHttpMessageHandler handler = new();
			_createdHandlers.Add(handler);
			return handler;
		}
	}

	private sealed class TrackingHttpMessageHandler : HttpMessageHandler
	{
		internal int SendCount { get; private set; }

		internal bool IsDisposed { get; private set; }

		protected override void Dispose(bool disposing)
		{
			IsDisposed = true;
			base.Dispose(disposing);
		}

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			SendCount++;
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}
	}

	private sealed class DictionaryConfiguration : IConfiguration, IConfigurationSection
	{
		private readonly Dictionary<string, string?> _values;
		private readonly string _path;

		internal DictionaryConfiguration(IEnumerable<(string Key, string? Value)> values)
		{
			_values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
			_path = string.Empty;

			foreach ((string key, string? value) in values)
			{
				_values[key] = value;
			}
		}

		private DictionaryConfiguration(Dictionary<string, string?> values, string path)
		{
			_values = values;
			_path = path;
		}

		public string? this[string key]
		{
			get => _values.TryGetValue(GetChildPath(key), out string? value) ? value : null;
			set => _values[GetChildPath(key)] = value;
		}

		public string Key => GetKey(_path);

		public string Path => _path;

		public string? Value
		{
			get => _values.TryGetValue(_path, out string? value) ? value : null;
			set => _values[_path] = value;
		}

		public IEnumerable<IConfigurationSection> GetChildren()
		{
			string prefix = string.IsNullOrEmpty(_path) ? string.Empty : _path + ":";

			return _values.Keys
				.Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				.Select(key => key.Substring(prefix.Length).Split(':')[0])
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Select(GetSection);
		}

		public IChangeToken GetReloadToken() => new CancellationChangeToken(CancellationToken.None);

		public IConfigurationSection GetSection(string key) => new DictionaryConfiguration(_values, GetChildPath(key));

		private static string GetKey(string path)
		{
			int separatorIndex = path.LastIndexOf(':');
			return separatorIndex < 0 ? path : path.Substring(separatorIndex + 1);
		}

		private string GetChildPath(string key) => string.IsNullOrEmpty(_path) ? key : _path + ":" + key;
	}
}
