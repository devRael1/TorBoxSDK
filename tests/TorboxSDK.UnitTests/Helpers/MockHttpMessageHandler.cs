using System.Net;

namespace TorboxSDK.UnitTests.Helpers;

internal sealed class MockHttpMessageHandler(string response, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
{
	private readonly string _response = response;
	private readonly HttpStatusCode _statusCode = statusCode;

	public HttpRequestMessage? LastRequest { get; private set; }
	public string? LastRequestContent { get; private set; }

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		LastRequest = request;
		if (request.Content is not null)
		{
			LastRequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
		}

		return new HttpResponseMessage(_statusCode)
		{
			Content = new StringContent(_response, System.Text.Encoding.UTF8, "application/json"),
		};
	}
}
