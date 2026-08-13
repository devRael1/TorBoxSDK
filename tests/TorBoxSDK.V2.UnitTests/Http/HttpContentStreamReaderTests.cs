using System.Text;
using TorBoxSDK.Http;

namespace TorBoxSDK.V2.UnitTests.Http;

public sealed class HttpContentStreamReaderTests
{
	[Fact]
	public async Task ReadAsync_WithPreCancelledToken_ThrowsBeforeReadingContent()
	{
		// Arrange
		using StringContent content = new("payload", Encoding.UTF8, "application/json");
		using CancellationTokenSource cancellationTokenSource = new();
		cancellationTokenSource.Cancel();

		// Act
		// Assert
		await Assert.ThrowsAnyAsync<OperationCanceledException>(
			() => HttpContentStreamReader.ReadAsync(content, cancellationTokenSource.Token));
	}

	[Fact]
	public async Task ReadAsync_WithContent_ReturnsAReadableStream()
	{
		// Arrange
		using StringContent content = new("payload", Encoding.UTF8, "application/json");

		// Act
		using Stream stream = await HttpContentStreamReader.ReadAsync(content, CancellationToken.None);
		using StreamReader reader = new(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
		string payload = await reader.ReadToEndAsync();

		// Assert
		Assert.Equal("payload", payload);
	}
}
