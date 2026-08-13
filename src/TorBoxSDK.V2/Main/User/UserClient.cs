using TorBoxSDK.Http;

namespace TorBoxSDK.Main.User;

internal sealed class UserClient : IUserClient
{
	internal UserClient(HttpClient httpClient, ITorBoxApiTransport transport)
	{
		if (httpClient is null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}

		if (transport is null)
		{
			throw new ArgumentNullException(nameof(transport));
		}
	}
}
