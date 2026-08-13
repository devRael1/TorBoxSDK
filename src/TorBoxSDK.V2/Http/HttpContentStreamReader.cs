namespace TorBoxSDK.Http;

internal static class HttpContentStreamReader
{
	internal static Task<Stream> ReadAsync(HttpContent content, CancellationToken cancellationToken)
	{
		if (content is null)
		{
			throw new ArgumentNullException(nameof(content));
		}

		cancellationToken.ThrowIfCancellationRequested();

#if NETSTANDARD2_0
		return content.ReadAsStreamAsync();
#else
		return content.ReadAsStreamAsync(cancellationToken);
#endif
	}
}
