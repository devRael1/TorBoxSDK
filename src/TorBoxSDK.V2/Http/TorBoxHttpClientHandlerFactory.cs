namespace TorBoxSDK.Http;

internal static class TorBoxHttpClientHandlerFactory
{
	internal static HttpClientHandler Create() => new()
	{
		AllowAutoRedirect = false,
	};
}
