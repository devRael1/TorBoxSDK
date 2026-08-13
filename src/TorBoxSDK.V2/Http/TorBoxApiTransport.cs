using System.Net;
using System.Net.Http.Headers;
using TorBoxSDK.Models.Common;

namespace TorBoxSDK.Http;

internal sealed class TorBoxApiTransport : ITorBoxApiTransport
{
	private readonly TorBoxEnvelopeJsonConverterFactory _envelopeConverter = new();

	public async Task<TorBoxResponse<T>> SendAsync<T>(
		HttpClient httpClient,
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ValidateArguments(httpClient, request);

		HttpResponseMessage response = await httpClient
			.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
			.ConfigureAwait(false);

		try
		{
			if (!IsJsonContent(response.Content.Headers.ContentType))
			{
				throw await CreateUnsupportedContentTypeExceptionAsync(response, request.RequestUri, cancellationToken)
					.ConfigureAwait(false);
			}

			using Stream body = await HttpContentStreamReader
				.ReadAsync(response.Content, cancellationToken)
				.ConfigureAwait(false);

			return await _envelopeConverter
				.DeserializeEnvelopeAsync<T>(body, request.RequestUri, response.StatusCode, cancellationToken)
				.ConfigureAwait(false);
		}
		finally
		{
			response.Dispose();
		}
	}

	public async Task<TorBoxResponse> SendAsync(
		HttpClient httpClient,
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ValidateArguments(httpClient, request);

		HttpResponseMessage response = await httpClient
			.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
			.ConfigureAwait(false);

		try
		{
			if (!IsJsonContent(response.Content.Headers.ContentType))
			{
				throw await CreateUnsupportedContentTypeExceptionAsync(response, request.RequestUri, cancellationToken)
					.ConfigureAwait(false);
			}

			using Stream body = await HttpContentStreamReader
				.ReadAsync(response.Content, cancellationToken)
				.ConfigureAwait(false);

			return await _envelopeConverter
				.DeserializeEnvelopeAsync(body, request.RequestUri, response.StatusCode, cancellationToken)
				.ConfigureAwait(false);
		}
		finally
		{
			response.Dispose();
		}
	}

	public async Task<TorBoxStreamResponse> SendStreamAsync(
		HttpClient httpClient,
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		ValidateArguments(httpClient, request);

		HttpResponseMessage response = await httpClient
			.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
			.ConfigureAwait(false);
		bool ownershipTransferred = false;

		try
		{
			if (IsRedirect(response.StatusCode))
			{
				if (response.Headers.Location is null)
				{
					throw await CreateProtocolExceptionForUnexpectedStreamResponseAsync(
						response,
						request.RequestUri,
						"A redirect response did not provide a location header.",
						cancellationToken).ConfigureAwait(false);
				}

				TorBoxStreamResponse redirectResponse = new(
					response,
					success: true,
					response.StatusCode,
					redirectUri: response.Headers.Location);
				ownershipTransferred = true;
				return redirectResponse;
			}

			if (IsJsonContent(response.Content.Headers.ContentType))
			{
				using Stream body = await HttpContentStreamReader
					.ReadAsync(response.Content, cancellationToken)
					.ConfigureAwait(false);
				TorBoxResponse envelope = await _envelopeConverter
					.DeserializeEnvelopeAsync(body, request.RequestUri, response.StatusCode, cancellationToken)
					.ConfigureAwait(false);

				if (!envelope.Success)
				{
					return new TorBoxStreamResponse(
						ownedResponse: null,
						success: false,
						response.StatusCode,
						error: envelope.Error,
						detail: envelope.Detail);
				}

				throw new TorBoxProtocolException(
					"A stream endpoint returned a successful JSON envelope instead of a stream or redirect.",
					request.RequestUri,
					response.StatusCode,
					envelope.Detail);
			}

			if (!response.IsSuccessStatusCode)
			{
				throw await CreateProtocolExceptionForUnexpectedStreamResponseAsync(
					response,
					request.RequestUri,
					"A stream endpoint returned a non-success response without a TorBox JSON envelope.",
					cancellationToken).ConfigureAwait(false);
			}

			Stream stream = await HttpContentStreamReader
				.ReadAsync(response.Content, cancellationToken)
				.ConfigureAwait(false);
			HttpContentHeaders headers = response.Content.Headers;
			TorBoxStreamResponse streamResponse = new(
				response,
				success: true,
				response.StatusCode,
				stream,
				mediaType: headers.ContentType?.MediaType,
				contentLength: headers.ContentLength,
				fileName: GetFileName(headers.ContentDisposition));
			ownershipTransferred = true;
			return streamResponse;
		}
		finally
		{
			if (!ownershipTransferred)
			{
				response.Dispose();
			}
		}
	}

	private static async Task<TorBoxProtocolException> CreateUnsupportedContentTypeExceptionAsync(
		HttpResponseMessage response,
		Uri? requestUri,
		CancellationToken cancellationToken)
	{
		using Stream body = await HttpContentStreamReader
			.ReadAsync(response.Content, cancellationToken)
			.ConfigureAwait(false);
		string? diagnostic = await BoundedDiagnosticReader.ReadAsync(body, cancellationToken).ConfigureAwait(false);

		return new TorBoxProtocolException(
			"The HTTP response content type is not supported for a TorBox JSON endpoint.",
			requestUri,
			response.StatusCode,
			diagnostic);
	}

	private static async Task<TorBoxProtocolException> CreateProtocolExceptionForUnexpectedStreamResponseAsync(
		HttpResponseMessage response,
		Uri? requestUri,
		string message,
		CancellationToken cancellationToken)
	{
		using Stream body = await HttpContentStreamReader
			.ReadAsync(response.Content, cancellationToken)
			.ConfigureAwait(false);
		string? diagnostic = await BoundedDiagnosticReader.ReadAsync(body, cancellationToken).ConfigureAwait(false);

		return new TorBoxProtocolException(message, requestUri, response.StatusCode, diagnostic);
	}

	private static string? GetFileName(ContentDispositionHeaderValue? contentDisposition)
	{
		string? fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName;
		return fileName?.Trim('"');
	}

	private static bool IsJsonContent(MediaTypeHeaderValue? contentType)
	{
		string? mediaType = contentType?.MediaType;

		return mediaType is not null
			&& (string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase)
				|| mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
	}

	private static bool IsRedirect(HttpStatusCode statusCode) => (int)statusCode is >= 300 and < 400;

	private static void ValidateArguments(HttpClient httpClient, HttpRequestMessage request)
	{
		if (httpClient is null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}

		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}
	}
}
