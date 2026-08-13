using System.Net;
using System.Text;
using System.Text.Json;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Serialization;

namespace TorBoxSDK.Http;

internal sealed class TorBoxEnvelopeJsonConverterFactory
{
	private const int MaximumPropertyNameByteCount = 256;
	private const int MaximumNestingDepth = 128;

	internal async Task<TorBoxResponse<T>> DeserializeEnvelopeAsync<T>(
		Stream stream,
		Uri? requestUri,
		HttpStatusCode statusCode,
		CancellationToken cancellationToken)
	{
		using JsonEnvelopeStreamReader reader = new(stream, cancellationToken);

		try
		{
			EnvelopeValues values = await reader.ReadEnvelopeAsync(captureData: true).ConfigureAwait(false);
			T? data = values.Success
				? await DeserializeDataAsync<T>(values.DataCapture, reader, requestUri, statusCode, cancellationToken)
					.ConfigureAwait(false)
				: default;

			return new TorBoxResponse<T>
			{
				Success = values.Success,
				Error = values.Error,
				Detail = values.Detail,
				Data = data,
				StatusCode = statusCode,
			};
		}
		catch (JsonException exception)
		{
			throw CreateProtocolException(requestUri, statusCode, reader.GetDiagnostic(), exception);
		}
		catch (IOException exception)
		{
			throw CreateProtocolException(requestUri, statusCode, reader.GetDiagnostic(), exception);
		}
	}

	internal async Task<TorBoxResponse> DeserializeEnvelopeAsync(
		Stream stream,
		Uri? requestUri,
		HttpStatusCode statusCode,
		CancellationToken cancellationToken)
	{
		using JsonEnvelopeStreamReader reader = new(stream, cancellationToken);

		try
		{
			EnvelopeValues values = await reader.ReadEnvelopeAsync(captureData: false).ConfigureAwait(false);

			return new TorBoxResponse
			{
				Success = values.Success,
				Error = values.Error,
				Detail = values.Detail,
				StatusCode = statusCode,
			};
		}
		catch (JsonException exception)
		{
			throw CreateProtocolException(requestUri, statusCode, reader.GetDiagnostic(), exception);
		}
		catch (IOException exception)
		{
			throw CreateProtocolException(requestUri, statusCode, reader.GetDiagnostic(), exception);
		}
	}

	private static async Task<T?> DeserializeDataAsync<T>(
		CapturedValue? dataCapture,
		JsonEnvelopeStreamReader reader,
		Uri? requestUri,
		HttpStatusCode statusCode,
		CancellationToken cancellationToken)
	{
		if (dataCapture is null)
		{
			return default;
		}

		using (dataCapture)
		{
			try
			{
				return await JsonSerializer.DeserializeAsync<T>(
					dataCapture.OpenRead(),
					TorBoxJsonOptions.Default,
					cancellationToken).ConfigureAwait(false);
			}
			catch (JsonException exception)
			{
				throw CreateProtocolException(requestUri, statusCode, reader.GetDiagnostic(), exception);
			}
		}
	}

	private static TorBoxProtocolException CreateProtocolException(
		Uri? requestUri,
		HttpStatusCode statusCode,
		string? diagnostic,
		Exception innerException) => new(
		"The HTTP response did not contain a valid TorBox JSON envelope.",
		requestUri,
		statusCode,
		diagnostic,
		innerException);

	private sealed class EnvelopeValues
	{
		internal EnvelopeValues(bool success, string? error, string? detail, CapturedValue? dataCapture)
		{
			Success = success;
			Error = error;
			Detail = detail;
			DataCapture = dataCapture;
		}

		internal bool Success { get; }

		internal string? Error { get; }

		internal string? Detail { get; }

		internal CapturedValue? DataCapture { get; }
	}

	private sealed class CapturedValue : IDisposable
	{
		private const int MaximumInMemoryByteCount = BoundedDiagnosticReader.MaxByteCount;
		private FileStream? _file;
		private MemoryStream? _memory = new(MaximumInMemoryByteCount);

		internal void WriteByte(byte value)
		{
			if (_file is null)
			{
				MemoryStream memory = _memory ?? throw new ObjectDisposedException(nameof(CapturedValue));

				if (memory.Length < MaximumInMemoryByteCount)
				{
					memory.WriteByte(value);
					return;
				}

				SpillToTemporaryFile(memory);
			}

			FileStream file = _file ?? throw new ObjectDisposedException(nameof(CapturedValue));
			file.WriteByte(value);
		}

		internal Stream OpenRead()
		{
			if (_file is FileStream file)
			{
				file.Flush();
				file.Position = 0;
				return file;
			}

			MemoryStream memory = _memory ?? throw new ObjectDisposedException(nameof(CapturedValue));
			memory.Position = 0;
			return memory;
		}

		public void Dispose()
		{
			_memory?.Dispose();
			_memory = null;
			_file?.Dispose();
			_file = null;
		}

		private void SpillToTemporaryFile(MemoryStream memory)
		{
			string temporaryFilePath = Path.Combine(Path.GetTempPath(), $"TorBoxSDK-{Guid.NewGuid():N}.tmp");
			FileStream file = new(
				temporaryFilePath,
				FileMode.CreateNew,
				FileAccess.ReadWrite,
				FileShare.None,
				bufferSize: 8192,
				FileOptions.DeleteOnClose);

			try
			{
				memory.Position = 0;
				memory.CopyTo(file);
			}
			catch
			{
				file.Dispose();
				throw;
			}

			memory.Dispose();
			_memory = null;
			_file = file;
		}
	}

	private sealed class JsonEnvelopeStreamReader : IDisposable
	{
		private readonly byte[] _buffer = new byte[8192];
		private readonly CancellationToken _cancellationToken;
		private readonly MemoryStream _diagnosticBytes = new(BoundedDiagnosticReader.MaxByteCount);
		private readonly Stream _stream;
		private CapturedValue? _capture;
		private CapturedValue? _dataCapture;
		private int _bufferCount;
		private int _bufferPosition;

		internal JsonEnvelopeStreamReader(Stream stream, CancellationToken cancellationToken)
		{
			_stream = stream ?? throw new ArgumentNullException(nameof(stream));
			_cancellationToken = cancellationToken;
		}

		internal async Task<EnvelopeValues> ReadEnvelopeAsync(bool captureData)
		{
			await SkipWhitespaceAsync().ConfigureAwait(false);
			await ExpectByteAsync((byte)'{').ConfigureAwait(false);

			bool hasSuccess = false;
			bool hasError = false;
			bool hasDetail = false;
			bool hasData = false;
			bool success = false;
			string? error = null;
			string? detail = null;
			bool firstProperty = true;

			while (true)
			{
				await SkipWhitespaceAsync().ConfigureAwait(false);
				int nextByte = await PeekByteAsync().ConfigureAwait(false);

				if (nextByte == '}')
				{
					await ReadByteAsync().ConfigureAwait(false);
					break;
				}

				if (!firstProperty)
				{
					await ExpectByteAsync((byte)',').ConfigureAwait(false);
					await SkipWhitespaceAsync().ConfigureAwait(false);
				}

				if (await PeekByteAsync().ConfigureAwait(false) != '"')
				{
					throw new JsonException("A TorBox JSON envelope property name must be a string.");
				}

				DecodedString propertyName = await ReadStringAsync(MaximumPropertyNameByteCount).ConfigureAwait(false);
				await SkipWhitespaceAsync().ConfigureAwait(false);
				await ExpectByteAsync((byte)':').ConfigureAwait(false);
				await SkipWhitespaceAsync().ConfigureAwait(false);

				if (!propertyName.WasTruncated && string.Equals(propertyName.Value, "success", StringComparison.Ordinal))
				{
					if (hasSuccess)
					{
						throw new JsonException("A TorBox JSON envelope cannot contain duplicate success properties.");
					}

					success = await ReadBooleanAsync().ConfigureAwait(false);
					hasSuccess = true;

					if (!success)
					{
						_dataCapture?.Dispose();
						_dataCapture = null;
					}
				}
				else if (!propertyName.WasTruncated && string.Equals(propertyName.Value, "error", StringComparison.Ordinal))
				{
					if (hasError)
					{
						throw new JsonException("A TorBox JSON envelope cannot contain duplicate error properties.");
					}

					error = await ReadNullableStringAsync().ConfigureAwait(false);
					hasError = true;
				}
				else if (!propertyName.WasTruncated && string.Equals(propertyName.Value, "detail", StringComparison.Ordinal))
				{
					if (hasDetail)
					{
						throw new JsonException("A TorBox JSON envelope cannot contain duplicate detail properties.");
					}

					detail = await ReadNullableStringAsync().ConfigureAwait(false);
					hasDetail = true;
				}
				else if (!propertyName.WasTruncated && string.Equals(propertyName.Value, "data", StringComparison.Ordinal))
				{
					if (hasData)
					{
						throw new JsonException("A TorBox JSON envelope cannot contain duplicate data properties.");
					}

					if (captureData && (!hasSuccess || success))
					{
						_dataCapture = await ReadCapturedValueAsync().ConfigureAwait(false);
					}
					else
					{
						await ReadValueAsync(0).ConfigureAwait(false);
					}
					hasData = true;
				}
				else
				{
					await ReadValueAsync(0).ConfigureAwait(false);
				}

				firstProperty = false;
			}

			await SkipWhitespaceAsync().ConfigureAwait(false);

			if (await PeekByteAsync().ConfigureAwait(false) != -1)
			{
				throw new JsonException("The TorBox JSON envelope contains trailing content.");
			}

			if (!hasSuccess)
			{
				throw new JsonException("The TorBox JSON envelope does not contain a success property.");
			}

			if (!success)
			{
				return new EnvelopeValues(success, error, detail, dataCapture: null);
			}

			CapturedValue? dataCapture = _dataCapture;
			_dataCapture = null;
			return new EnvelopeValues(success, error, detail, dataCapture);
		}

		internal string? GetDiagnostic()
		{
			if (_diagnosticBytes.Length == 0)
			{
				return null;
			}

			string diagnostic = Encoding.UTF8.GetString(_diagnosticBytes.ToArray());
			return TorBoxProtocolException.BoundDiagnostic(diagnostic);
		}

		public void Dispose()
		{
			_dataCapture?.Dispose();
			_diagnosticBytes.Dispose();
		}

		private async Task<CapturedValue> ReadCapturedValueAsync()
		{
			CapturedValue capture = new();
			_capture = capture;

			try
			{
				await ReadValueAsync(0).ConfigureAwait(false);
				return capture;
			}
			catch
			{
				capture.Dispose();
				throw;
			}
			finally
			{
				_capture = null;
			}
		}

		private async Task ReadValueAsync(int nestingDepth)
		{
			if (nestingDepth > MaximumNestingDepth)
			{
				throw new JsonException("The TorBox JSON envelope exceeds the supported nesting depth.");
			}

			await SkipWhitespaceAsync().ConfigureAwait(false);
			int nextByte = await PeekByteAsync().ConfigureAwait(false);

			switch (nextByte)
			{
				case '"':
					_ = await ReadStringAsync(0).ConfigureAwait(false);
					return;
				case '{':
					await ReadObjectAsync(nestingDepth + 1).ConfigureAwait(false);
					return;
				case '[':
					await ReadArrayAsync(nestingDepth + 1).ConfigureAwait(false);
					return;
				case 't':
					await ReadLiteralAsync("true").ConfigureAwait(false);
					return;
				case 'f':
					await ReadLiteralAsync("false").ConfigureAwait(false);
					return;
				case 'n':
					await ReadLiteralAsync("null").ConfigureAwait(false);
					return;
				case '-':
				case >= '0' and <= '9':
					await ReadNumberAsync().ConfigureAwait(false);
					return;
				default:
					throw new JsonException("The TorBox JSON envelope contains an invalid JSON value.");
			}
		}

		private async Task ReadObjectAsync(int nestingDepth)
		{
			await ExpectByteAsync((byte)'{').ConfigureAwait(false);
			await SkipWhitespaceAsync().ConfigureAwait(false);

			if (await PeekByteAsync().ConfigureAwait(false) == '}')
			{
				await ReadByteAsync().ConfigureAwait(false);
				return;
			}

			bool firstProperty = true;

			while (true)
			{
				if (!firstProperty)
				{
					await ExpectByteAsync((byte)',').ConfigureAwait(false);
					await SkipWhitespaceAsync().ConfigureAwait(false);
				}

				if (await PeekByteAsync().ConfigureAwait(false) != '"')
				{
					throw new JsonException("A JSON object property name must be a string.");
				}

				_ = await ReadStringAsync(0).ConfigureAwait(false);
				await SkipWhitespaceAsync().ConfigureAwait(false);
				await ExpectByteAsync((byte)':').ConfigureAwait(false);
				await ReadValueAsync(nestingDepth).ConfigureAwait(false);
				await SkipWhitespaceAsync().ConfigureAwait(false);

				if (await PeekByteAsync().ConfigureAwait(false) == '}')
				{
					await ReadByteAsync().ConfigureAwait(false);
					return;
				}

				firstProperty = false;
			}
		}

		private async Task ReadArrayAsync(int nestingDepth)
		{
			await ExpectByteAsync((byte)'[').ConfigureAwait(false);
			await SkipWhitespaceAsync().ConfigureAwait(false);

			if (await PeekByteAsync().ConfigureAwait(false) == ']')
			{
				await ReadByteAsync().ConfigureAwait(false);
				return;
			}

			bool firstValue = true;

			while (true)
			{
				if (!firstValue)
				{
					await ExpectByteAsync((byte)',').ConfigureAwait(false);
				}

				await ReadValueAsync(nestingDepth).ConfigureAwait(false);
				await SkipWhitespaceAsync().ConfigureAwait(false);

				if (await PeekByteAsync().ConfigureAwait(false) == ']')
				{
					await ReadByteAsync().ConfigureAwait(false);
					return;
				}

				firstValue = false;
			}
		}

		private async Task ReadNumberAsync()
		{
			int nextByte = await PeekByteAsync().ConfigureAwait(false);

			if (nextByte == '-')
			{
				await ReadByteAsync().ConfigureAwait(false);
				nextByte = await PeekByteAsync().ConfigureAwait(false);
			}

			if (nextByte == '0')
			{
				await ReadByteAsync().ConfigureAwait(false);

				if (IsDigit(await PeekByteAsync().ConfigureAwait(false)))
				{
					throw new JsonException("A JSON number cannot contain a leading zero.");
				}
			}
			else if (nextByte is >= '1' and <= '9')
			{
				do
				{
					await ReadByteAsync().ConfigureAwait(false);
					nextByte = await PeekByteAsync().ConfigureAwait(false);
				}
				while (IsDigit(nextByte));
			}
			else
			{
				throw new JsonException("A JSON number must contain an integer component.");
			}

			nextByte = await PeekByteAsync().ConfigureAwait(false);

			if (nextByte == '.')
			{
				await ReadByteAsync().ConfigureAwait(false);
				nextByte = await PeekByteAsync().ConfigureAwait(false);

				if (!IsDigit(nextByte))
				{
					throw new JsonException("A JSON number fraction must contain a digit.");
				}

				do
				{
					await ReadByteAsync().ConfigureAwait(false);
					nextByte = await PeekByteAsync().ConfigureAwait(false);
				}
				while (IsDigit(nextByte));
			}

			if (nextByte is 'e' or 'E')
			{
				await ReadByteAsync().ConfigureAwait(false);
				nextByte = await PeekByteAsync().ConfigureAwait(false);

				if (nextByte is '+' or '-')
				{
					await ReadByteAsync().ConfigureAwait(false);
					nextByte = await PeekByteAsync().ConfigureAwait(false);
				}

				if (!IsDigit(nextByte))
				{
					throw new JsonException("A JSON number exponent must contain a digit.");
				}

				do
				{
					await ReadByteAsync().ConfigureAwait(false);
					nextByte = await PeekByteAsync().ConfigureAwait(false);
				}
				while (IsDigit(nextByte));
			}
		}

		private async Task<bool> ReadBooleanAsync()
		{
			int nextByte = await PeekByteAsync().ConfigureAwait(false);

			if (nextByte == 't')
			{
				await ReadLiteralAsync("true").ConfigureAwait(false);
				return true;
			}

			if (nextByte == 'f')
			{
				await ReadLiteralAsync("false").ConfigureAwait(false);
				return false;
			}

			throw new JsonException("The TorBox JSON envelope success property must be a Boolean.");
		}

		private async Task<string?> ReadNullableStringAsync()
		{
			int nextByte = await PeekByteAsync().ConfigureAwait(false);

			if (nextByte == 'n')
			{
				await ReadLiteralAsync("null").ConfigureAwait(false);
				return null;
			}

			if (nextByte != '"')
			{
				throw new JsonException("The TorBox JSON envelope error and detail properties must be strings or null.");
			}

			DecodedString value = await ReadStringAsync(BoundedDiagnosticReader.MaxByteCount).ConfigureAwait(false);
			return value.Value;
		}

		private async Task<DecodedString> ReadStringAsync(int maximumRetainedByteCount)
		{
			await ExpectByteAsync((byte)'"').ConfigureAwait(false);
			BoundedTextBuilder text = new(maximumRetainedByteCount);

			while (true)
			{
				int nextByte = await ReadRequiredByteAsync().ConfigureAwait(false);

				if (nextByte == '"')
				{
					return text.Complete();
				}

				if (nextByte == '\\')
				{
					await ReadEscapeSequenceAsync(text).ConfigureAwait(false);
					continue;
				}

				if (nextByte < 0x20)
				{
					throw new JsonException("A JSON string cannot contain an unescaped control character.");
				}

				if (nextByte <= 0x7F)
				{
					text.Append((char)nextByte);
					continue;
				}

				int codePoint = await ReadUtf8CodePointAsync(nextByte).ConfigureAwait(false);
				text.AppendCodePoint(codePoint);
			}
		}

		private async Task ReadEscapeSequenceAsync(BoundedTextBuilder text)
		{
			int escapedByte = await ReadRequiredByteAsync().ConfigureAwait(false);

			switch (escapedByte)
			{
				case '"':
				case '\\':
				case '/':
					text.Append((char)escapedByte);
					return;
				case 'b':
					text.Append('\b');
					return;
				case 'f':
					text.Append('\f');
					return;
				case 'n':
					text.Append('\n');
					return;
				case 'r':
					text.Append('\r');
					return;
				case 't':
					text.Append('\t');
					return;
				case 'u':
					text.Append((char)await ReadHexadecimalCodeUnitAsync().ConfigureAwait(false));
					return;
				default:
					throw new JsonException("A JSON string contains an invalid escape sequence.");
			}
		}

		private async Task<int> ReadHexadecimalCodeUnitAsync()
		{
			int codeUnit = 0;

			for (int index = 0; index < 4; index++)
			{
				int hexadecimalByte = await ReadRequiredByteAsync().ConfigureAwait(false);
				int hexadecimalValue = hexadecimalByte switch
				{
					>= '0' and <= '9' => hexadecimalByte - '0',
					>= 'A' and <= 'F' => hexadecimalByte - 'A' + 10,
					>= 'a' and <= 'f' => hexadecimalByte - 'a' + 10,
					_ => throw new JsonException("A JSON Unicode escape must contain four hexadecimal digits."),
				};

				codeUnit = (codeUnit << 4) | hexadecimalValue;
			}

			return codeUnit;
		}

		private async Task<int> ReadUtf8CodePointAsync(int leadingByte)
		{
			int expectedContinuationCount;
			int codePoint;
			int minimumCodePoint;

			if (leadingByte is >= 0xC2 and <= 0xDF)
			{
				expectedContinuationCount = 1;
				codePoint = leadingByte & 0x1F;
				minimumCodePoint = 0x80;
			}
			else if (leadingByte is >= 0xE0 and <= 0xEF)
			{
				expectedContinuationCount = 2;
				codePoint = leadingByte & 0x0F;
				minimumCodePoint = 0x800;
			}
			else if (leadingByte is >= 0xF0 and <= 0xF4)
			{
				expectedContinuationCount = 3;
				codePoint = leadingByte & 0x07;
				minimumCodePoint = 0x10000;
			}
			else
			{
				throw new JsonException("A JSON string contains an invalid UTF-8 leading byte.");
			}

			for (int index = 0; index < expectedContinuationCount; index++)
			{
				int continuationByte = await ReadRequiredByteAsync().ConfigureAwait(false);

				if ((continuationByte & 0xC0) != 0x80)
				{
					throw new JsonException("A JSON string contains an invalid UTF-8 continuation byte.");
				}

				codePoint = (codePoint << 6) | (continuationByte & 0x3F);
			}

			if (codePoint < minimumCodePoint || codePoint is >= 0xD800 and <= 0xDFFF || codePoint > 0x10FFFF)
			{
				throw new JsonException("A JSON string contains an invalid Unicode scalar value.");
			}

			return codePoint;
		}

		private async Task ReadLiteralAsync(string literal)
		{
			foreach (char expectedCharacter in literal)
			{
				int actualByte = await ReadRequiredByteAsync().ConfigureAwait(false);

				if (actualByte != expectedCharacter)
				{
					throw new JsonException("The TorBox JSON envelope contains an invalid literal value.");
				}
			}
		}

		private async Task SkipWhitespaceAsync()
		{
			while (await PeekByteAsync().ConfigureAwait(false) is ' ' or '\t' or '\r' or '\n')
			{
				await ReadByteAsync().ConfigureAwait(false);
			}
		}

		private async Task ExpectByteAsync(byte expectedByte)
		{
			int actualByte = await ReadRequiredByteAsync().ConfigureAwait(false);

			if (actualByte != expectedByte)
			{
				throw new JsonException("The TorBox JSON envelope contains an unexpected character.");
			}
		}

		private async Task<int> ReadRequiredByteAsync()
		{
			int value = await ReadByteAsync().ConfigureAwait(false);

			if (value == -1)
			{
				throw new JsonException("The TorBox JSON envelope ended unexpectedly.");
			}

			return value;
		}

		private async Task<int> ReadByteAsync()
		{
			int nextByte = await PeekByteAsync().ConfigureAwait(false);

			if (nextByte == -1)
			{
				return -1;
			}

			_bufferPosition++;

			if (_diagnosticBytes.Length < BoundedDiagnosticReader.MaxByteCount)
			{
				_diagnosticBytes.WriteByte((byte)nextByte);
			}

			_capture?.WriteByte((byte)nextByte);
			return nextByte;
		}

		private async Task<int> PeekByteAsync()
		{
			_cancellationToken.ThrowIfCancellationRequested();

			if (_bufferPosition < _bufferCount)
			{
				return _buffer[_bufferPosition];
			}

			_bufferCount = await _stream
				.ReadAsync(_buffer, 0, _buffer.Length, _cancellationToken)
				.ConfigureAwait(false);
			_bufferPosition = 0;

			return _bufferCount == 0 ? -1 : _buffer[_bufferPosition];
		}

		private static bool IsDigit(int value) => value is >= '0' and <= '9';
	}

	private readonly struct DecodedString
	{
		internal DecodedString(string value, bool wasTruncated)
		{
			Value = value;
			WasTruncated = wasTruncated;
		}

		internal string Value { get; }

		internal bool WasTruncated { get; }
	}

	private sealed class BoundedTextBuilder
	{
		private readonly StringBuilder _builder = new();
		private readonly int _maximumByteCount;
		private int _retainedByteCount;
		private char? _pendingHighSurrogate;

		internal BoundedTextBuilder(int maximumByteCount)
		{
			_maximumByteCount = maximumByteCount;
		}

		internal bool WasTruncated { get; private set; }

		internal void Append(char value)
		{
			if (_pendingHighSurrogate is char pendingHighSurrogate)
			{
				_pendingHighSurrogate = null;

				if (char.IsLowSurrogate(value))
				{
					AppendScalar(pendingHighSurrogate, value, 4);
					return;
				}

				AppendScalar(pendingHighSurrogate, null, 3);
			}

			if (char.IsHighSurrogate(value))
			{
				_pendingHighSurrogate = value;
				return;
			}

			AppendScalar(value, null, GetUtf8ByteCount(value));
		}

		internal void AppendCodePoint(int codePoint)
		{
			if (codePoint <= 0xFFFF)
			{
				Append((char)codePoint);
				return;
			}

			int adjustedCodePoint = codePoint - 0x10000;
			Append((char)((adjustedCodePoint >> 10) + 0xD800));
			Append((char)((adjustedCodePoint & 0x3FF) + 0xDC00));
		}

		internal DecodedString Complete()
		{
			if (_pendingHighSurrogate is char pendingHighSurrogate)
			{
				_pendingHighSurrogate = null;
				AppendScalar(pendingHighSurrogate, null, 3);
			}

			return new DecodedString(_builder.ToString(), WasTruncated);
		}

		private void AppendScalar(char firstCharacter, char? secondCharacter, int byteCount)
		{
			if (_retainedByteCount + byteCount > _maximumByteCount)
			{
				WasTruncated = true;
				return;
			}

			_builder.Append(firstCharacter);

			if (secondCharacter is char second)
			{
				_builder.Append(second);
			}

			_retainedByteCount += byteCount;
		}

		private static int GetUtf8ByteCount(char value) => value switch
		{
			<= '\u007F' => 1,
			<= '\u07FF' => 2,
			_ => 3,
		};
	}
}
