using System.Net;
using System.Net.Http;
using System.Reflection;
using TorBoxSDK.Http;
using TorBoxSDK.Internal;
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

namespace TorBoxSDK.V2.UnitTests;

public sealed class TorBoxClientTests
{
	[Fact]
	public void ITorBoxClient_InheritsIDisposable()
	{
		// Arrange
		Type clientType = typeof(ITorBoxClient);

		// Act
		bool implementsDisposable = typeof(IDisposable).IsAssignableFrom(clientType);

		// Assert
		Assert.True(implementsDisposable);
	}

	[Fact]
	public void Constructor_WithValidOptions_ExposesAllApprovedApiFamilies()
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();

		// Act
		using TorBoxClient client = new(options);

		// Assert
		Assert.NotNull(client.Main);
		Assert.NotNull(client.Search);
		Assert.NotNull(client.Relay);
	}

	[Fact]
	public void Main_ExposesTheElevenApprovedResources()
	{
		// Arrange
		using TorBoxClient client = new(CreateValidOptions());
		string[] expectedPropertyNames =
		[
			"General",
			"Torrents",
			"Usenet",
			"WebDownloads",
			"User",
			"Notifications",
			"Rss",
			"Stream",
			"Integrations",
			"Vendors",
			"Queued",
		];

		// Act
		PropertyInfo[] properties = typeof(IMainApiClient)
			.GetProperties()
			.OrderBy(static property => property.Name, StringComparer.Ordinal)
			.ToArray();
		string[] actualPropertyNames = properties.Select(static property => property.Name).ToArray();

		// Assert
		Assert.Equal(expectedPropertyNames.OrderBy(static name => name, StringComparer.Ordinal), actualPropertyNames);
		Assert.IsAssignableFrom<IGeneralClient>(client.Main.General);
		Assert.IsAssignableFrom<ITorrentsClient>(client.Main.Torrents);
		Assert.IsAssignableFrom<IUsenetClient>(client.Main.Usenet);
		Assert.IsAssignableFrom<IWebDownloadsClient>(client.Main.WebDownloads);
		Assert.IsAssignableFrom<IUserClient>(client.Main.User);
		Assert.IsAssignableFrom<INotificationsClient>(client.Main.Notifications);
		Assert.IsAssignableFrom<IRssClient>(client.Main.Rss);
		Assert.IsAssignableFrom<IStreamClient>(client.Main.Stream);
		Assert.IsAssignableFrom<IIntegrationsClient>(client.Main.Integrations);
		Assert.IsAssignableFrom<IVendorsClient>(client.Main.Vendors);
		Assert.IsAssignableFrom<IQueuedClient>(client.Main.Queued);
	}

	[Fact]
	public void FamilyAndResourceInterfaces_ContainNoEndpointMethods()
	{
		// Arrange
		Type[] clientInterfaces =
		[
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
		IEnumerable<MethodInfo> endpointMethods = clientInterfaces.SelectMany(static clientInterface => clientInterface.GetMethods());

		// Assert
		Assert.Empty(endpointMethods);
	}

	[Fact]
	public void PublicClientNavigation_DoesNotExposeRawHttpTypes()
	{
		// Arrange
		Type[] clientTypes =
		[
			typeof(ITorBoxClient),
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
		IEnumerable<Type> exposedTypes = clientTypes
			.SelectMany(static clientType => clientType.GetProperties().Select(static property => property.PropertyType))
			.Concat(clientTypes.SelectMany(static clientType => clientType.GetMethods().Select(static method => method.ReturnType)));

		// Assert
		Assert.DoesNotContain(typeof(HttpClient), exposedTypes);
		Assert.DoesNotContain(typeof(HttpResponseMessage), exposedTypes);
	}

	[Fact]
	public async Task Factory_WithUnownedHttpClients_DisposeLeavesAllFamilyClientsUsable()
	{
		// Arrange
		TrackingHttpMessageHandler mainHandler = new();
		TrackingHttpMessageHandler searchHandler = new();
		TrackingHttpMessageHandler relayHandler = new();
		using HttpClient mainHttpClient = CreateHttpClient(mainHandler, "https://main.example.test/v1/api/");
		using HttpClient searchHttpClient = CreateHttpClient(searchHandler, "https://search.example.test/");
		using HttpClient relayHttpClient = CreateHttpClient(relayHandler, "https://relay.example.test/v1/");
		ITorBoxClient client = TorBoxClientFactory.Create(
			mainHttpClient,
			searchHttpClient,
			relayHttpClient,
			new TorBoxApiTransport(),
			ownsHttpClients: false);

		// Act
		client.Dispose();
		using HttpResponseMessage mainResponse = await mainHttpClient.GetAsync("probe", CancellationToken.None);
		using HttpResponseMessage searchResponse = await searchHttpClient.GetAsync("probe", CancellationToken.None);
		using HttpResponseMessage relayResponse = await relayHttpClient.GetAsync("probe", CancellationToken.None);

		// Assert
		Assert.Equal(HttpStatusCode.OK, mainResponse.StatusCode);
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
	public void Factory_WithOwnedHttpClients_DisposeDisposesAllFamilyHandlers()
	{
		// Arrange
		TrackingHttpMessageHandler mainHandler = new();
		TrackingHttpMessageHandler searchHandler = new();
		TrackingHttpMessageHandler relayHandler = new();
		using HttpClient mainHttpClient = CreateHttpClient(mainHandler, "https://main.example.test/v1/api/");
		using HttpClient searchHttpClient = CreateHttpClient(searchHandler, "https://search.example.test/");
		using HttpClient relayHttpClient = CreateHttpClient(relayHandler, "https://relay.example.test/v1/");
		ITorBoxClient client = TorBoxClientFactory.Create(
			mainHttpClient,
			searchHttpClient,
			relayHttpClient,
			new TorBoxApiTransport(),
			ownsHttpClients: true);

		// Act
		client.Dispose();

		// Assert
		Assert.True(mainHandler.IsDisposed);
		Assert.True(searchHandler.IsDisposed);
		Assert.True(relayHandler.IsDisposed);
	}

	private static TorBoxClientOptions CreateValidOptions() => new()
	{
		ApiKey = "test-api-key",
	};

	private static HttpClient CreateHttpClient(HttpMessageHandler handler, string baseAddress) => new(handler)
	{
		BaseAddress = new Uri(baseAddress),
	};

	private sealed class TrackingHttpMessageHandler : HttpMessageHandler
	{
		public int SendCount { get; private set; }

		public bool IsDisposed { get; private set; }

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
}
