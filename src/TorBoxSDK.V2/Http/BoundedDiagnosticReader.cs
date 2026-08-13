using System.Text;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Http;

internal static class BoundedDiagnosticReader
{
	internal const int MaxByteCount = 64 * 1024;

	internal static async Task<string?> ReadAsync(Stream stream, CancellationToken cancellationToken)
	{
		if (stream is null)
		{
			throw new ArgumentNullException(nameof(stream));
		}

		cancellationToken.ThrowIfCancellationRequested();

		byte[] buffer = new byte[8192];
		using MemoryStream retainedBytes = new(MaxByteCount);

		while (retainedBytes.Length < MaxByteCount)
		{
			int countToRead = (int)Math.Min(buffer.Length, MaxByteCount - retainedBytes.Length);
			int bytesRead = await stream
				.ReadAsync(buffer, 0, countToRead, cancellationToken)
				.ConfigureAwait(false);

			if (bytesRead == 0)
			{
				break;
			}

			retainedBytes.Write(buffer, 0, bytesRead);
		}

		if (retainedBytes.Length == 0)
		{
			return null;
		}

		string diagnostic = Encoding.UTF8.GetString(retainedBytes.ToArray());
		return TorBoxProtocolException.BoundDiagnostic(diagnostic);
	}
}
