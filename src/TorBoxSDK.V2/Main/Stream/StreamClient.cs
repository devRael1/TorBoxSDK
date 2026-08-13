using System.Globalization;
using TorBoxSDK.Http;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Stream;

namespace TorBoxSDK.Main.Stream;

internal sealed class StreamClient : IStreamClient
{
	private readonly HttpClient _httpClient;
	private readonly ITorBoxApiTransport _transport;

	internal StreamClient(HttpClient httpClient, ITorBoxApiTransport transport)
	{
		if (httpClient is null)
		{
			throw new ArgumentNullException(nameof(httpClient));
		}

		if (transport is null)
		{
			throw new ArgumentNullException(nameof(transport));
		}

		_httpClient = httpClient;
		_transport = transport;
	}

	public async Task<TorBoxResponse<string>> CreateStreamAsync(
		CreateStreamRequest request,
		CancellationToken cancellationToken = default)
	{
		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		if (request.Type is not null && string.IsNullOrWhiteSpace(request.Type))
		{
			throw new ArgumentException("The stream type cannot be whitespace.", nameof(request));
		}

		string query = QueryStringBuilder.Build(
			("id", request.DownloadId.ToString(CultureInfo.InvariantCulture)),
			("file_id", request.FileId is long fileId ? fileId.ToString(CultureInfo.InvariantCulture) : null),
			("type", request.Type),
			("chosen_subtitle_index", request.ChosenSubtitleIndex is int subtitleIndex ? subtitleIndex.ToString(CultureInfo.InvariantCulture) : null),
			("chosen_audio_index", request.ChosenAudioIndex is int audioIndex ? audioIndex.ToString(CultureInfo.InvariantCulture) : null),
			("chosen_resolution_index", request.ChosenResolutionIndex is int resolutionIndex ? resolutionIndex.ToString(CultureInfo.InvariantCulture) : null),
			("scrobbling_enabled", request.ScrobblingEnabled is bool enabled ? (enabled ? "true" : "false") : null));
		using HttpRequestMessage httpRequest = new(HttpMethod.Get, "stream/createstream" + query);

		return await _transport
			.SendAsync<string>(_httpClient, httpRequest, cancellationToken)
			.ConfigureAwait(false);
	}

	public async Task<TorBoxResponse<StreamData>> GetStreamDataAsync(
		GetStreamDataRequest request,
		CancellationToken cancellationToken = default)
	{
		if (request is null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		if (string.IsNullOrWhiteSpace(request.PresignedToken))
		{
			throw new ArgumentException("The presigned token cannot be whitespace.", nameof(request));
		}

		if (string.IsNullOrWhiteSpace(request.Token))
		{
			throw new ArgumentException("The stream token cannot be whitespace.", nameof(request));
		}

		string query = QueryStringBuilder.Build(
			("presigned_token", request.PresignedToken),
			("token", request.Token),
			("chosen_subtitle_index", request.ChosenSubtitleIndex is int subtitleIndex ? subtitleIndex.ToString(CultureInfo.InvariantCulture) : null),
			("chosen_audio_index", request.ChosenAudioIndex is int audioIndex ? audioIndex.ToString(CultureInfo.InvariantCulture) : null),
			("chosen_resolution_index", request.ChosenResolutionIndex is int resolutionIndex ? resolutionIndex.ToString(CultureInfo.InvariantCulture) : null));
		using HttpRequestMessage httpRequest = new(HttpMethod.Get, "stream/getstreamdata" + query);

		return await _transport
			.SendAsync<StreamData>(_httpClient, httpRequest, cancellationToken)
			.ConfigureAwait(false);
	}
}
