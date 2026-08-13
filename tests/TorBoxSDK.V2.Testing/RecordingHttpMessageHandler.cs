using System.Net;
using System.Text;

namespace TorBoxSDK.V2.Testing;

public sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
	private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _responseFactory;

	private RecordingHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responseFactory)
	{
		_responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
	}

	public int SendCount { get; private set; }

	public CancellationToken LastCancellationToken { get; private set; }

	public Uri? LastRequestUri { get; private set; }

	public IReadOnlyList<string> LastAuthorizationHeaderValues { get; private set; } = Array.Empty<string>();

	public static RecordingHttpMessageHandler FromResponse(
		Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responseFactory) => new(responseFactory);

	public static RecordingHttpMessageHandler Json(
		HttpStatusCode statusCode,
		string json,
		string mediaType = "application/json")
	{
		ArgumentNullException.ThrowIfNull(json);
		if (string.IsNullOrWhiteSpace(mediaType))
		{
			throw new ArgumentException("The media type cannot be null or whitespace.", nameof(mediaType));
		}

		return new RecordingHttpMessageHandler(
			(request, cancellationToken) =>
			{
				StringContent content = new(json, Encoding.UTF8, mediaType);
				return new HttpResponseMessage(statusCode)
				{
					Content = content,
				};
			});
	}

	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request);

		SendCount++;
		LastCancellationToken = cancellationToken;
		LastRequestUri = request.RequestUri;
		LastAuthorizationHeaderValues = request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values)
			? values.ToArray()
			: Array.Empty<string>();

		return Task.FromResult(_responseFactory(request, cancellationToken));
	}
}
