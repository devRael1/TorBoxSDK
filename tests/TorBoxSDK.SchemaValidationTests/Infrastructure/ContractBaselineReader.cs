using System.Security.Cryptography;
using System.Text.Json;

namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

internal static class ContractBaselineReader
{
	private const string ManifestFileName = "manifest.json";

	internal static ContractBaseline Load() => Load(LocateBaselineDirectory());

	internal static ContractBaseline Load(string baselineDirectory)
	{
		if (string.IsNullOrWhiteSpace(baselineDirectory))
		{
			throw new ArgumentException("The baseline directory must not be null, empty, or whitespace.", nameof(baselineDirectory));
		}

		string fullBaselineDirectory = Path.GetFullPath(baselineDirectory);
		string manifestPath = Path.Combine(fullBaselineDirectory, ManifestFileName);
		if (!File.Exists(manifestPath))
		{
			throw new FileNotFoundException("The contract baseline manifest was not found.", manifestPath);
		}

		using JsonDocument manifestDocument = JsonDocument.Parse(File.ReadAllText(manifestPath));
		JsonElement root = manifestDocument.RootElement;
		int schemaVersion = ReadRequiredInt(root, "schemaVersion", "manifest");
		if (schemaVersion != 1)
		{
			throw new InvalidDataException($"Unsupported contract baseline schema version '{schemaVersion}'.");
		}

		if (!root.TryGetProperty("sources", out JsonElement sources) ||
			sources.ValueKind != JsonValueKind.Array)
		{
			throw new InvalidDataException("The contract baseline manifest must contain a sources array.");
		}

		Dictionary<string, ContractSnapshotDescriptor> descriptors = new(StringComparer.Ordinal);
		Dictionary<string, ContractSnapshot> snapshots = new(StringComparer.Ordinal);
		foreach (JsonElement source in sources.EnumerateArray())
		{
			ContractSnapshotDescriptor descriptor = ReadDescriptor(source);
			if (!descriptors.TryAdd(descriptor.Id, descriptor))
			{
				throw new InvalidDataException($"The contract baseline contains duplicate source id '{descriptor.Id}'.");
			}

			if (!descriptor.IsCaptured)
			{
				continue;
			}

			ContractSnapshot snapshot = ReadSnapshot(fullBaselineDirectory, descriptor);
			snapshots.Add(descriptor.Id, snapshot);
		}

		return new ContractBaseline(fullBaselineDirectory, descriptors, snapshots);
	}

	private static ContractSnapshotDescriptor ReadDescriptor(JsonElement source)
	{
		string id = ReadRequiredString(source, "id", "source");
		string family = ReadRequiredString(source, "family", $"source '{id}'");
		string format = ReadRequiredString(source, "format", $"source '{id}'");
		string availability = ReadRequiredString(source, "availability", $"source '{id}'");
		bool isCaptured = availability switch
		{
			"captured" => true,
			"unavailable" => false,
			_ => throw new InvalidDataException($"Source '{id}' has unsupported availability '{availability}'.")
		};

		string? artifactPath = ReadOptionalString(source, "artifactPath");
		string? sourceUrl = ReadOptionalString(source, "sourceUrl");
		long? contentLength = ReadOptionalInt64(source, "contentLength");
		string? sha256 = ReadOptionalString(source, "sha256");
		string? reason = ReadOptionalString(source, "reason");

		if (isCaptured &&
			(string.IsNullOrWhiteSpace(artifactPath) ||
			 string.IsNullOrWhiteSpace(sourceUrl) ||
			 contentLength is null ||
			 string.IsNullOrWhiteSpace(sha256)))
		{
			throw new InvalidDataException($"Captured source '{id}' is missing required provenance or integrity metadata.");
		}

		if (!isCaptured && !string.IsNullOrWhiteSpace(artifactPath))
		{
			throw new InvalidDataException($"Unavailable source '{id}' must not reference an artifact.");
		}

		return new ContractSnapshotDescriptor(
			id,
			family,
			format,
			isCaptured,
			artifactPath,
			sourceUrl,
			contentLength,
			sha256,
			reason);
	}

	private static ContractSnapshot ReadSnapshot(string baselineDirectory, ContractSnapshotDescriptor descriptor)
	{
		string artifactPath = descriptor.ArtifactPath
			?? throw new InvalidDataException($"Captured source '{descriptor.Id}' has no artifact path.");
		string expectedHash = descriptor.Sha256
			?? throw new InvalidDataException($"Captured source '{descriptor.Id}' has no SHA-256 hash.");
		long expectedLength = descriptor.ContentLength
			?? throw new InvalidDataException($"Captured source '{descriptor.Id}' has no content length.");
		string fullArtifactPath = ResolveArtifactPath(baselineDirectory, artifactPath);
		if (!File.Exists(fullArtifactPath))
		{
			throw new FileNotFoundException($"The artifact for captured source '{descriptor.Id}' was not found.", fullArtifactPath);
		}

		byte[] content = File.ReadAllBytes(fullArtifactPath);
		if (content.LongLength != expectedLength)
		{
			throw new InvalidDataException(
				$"Artifact '{artifactPath}' has length '{content.LongLength}', expected '{expectedLength}'.");
		}

		string actualHash = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
		if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidDataException(
				$"Artifact '{artifactPath}' has SHA-256 '{actualHash}', expected '{expectedHash}'.");
		}

		using JsonDocument ignored = JsonDocument.Parse(content);
		return new ContractSnapshot(descriptor, fullArtifactPath, content);
	}

	private static string LocateBaselineDirectory()
	{
		string[] startingDirectories = [AppContext.BaseDirectory, Directory.GetCurrentDirectory()];
		foreach (string startingDirectory in startingDirectories)
		{
			DirectoryInfo? directory = new(Path.GetFullPath(startingDirectory));
			while (directory is not null)
			{
				string candidate = Path.Combine(directory.FullName, "contracts", "baseline", ManifestFileName);
				if (File.Exists(candidate))
				{
					return Path.GetDirectoryName(candidate)
						?? throw new InvalidDataException("The contract baseline manifest has no parent directory.");
				}

				directory = directory.Parent;
			}
		}

		throw new DirectoryNotFoundException(
			"Could not locate contracts/baseline/manifest.json from the test execution directory.");
	}

	private static string ResolveArtifactPath(string baselineDirectory, string artifactPath)
	{
		if (Path.IsPathRooted(artifactPath))
		{
			throw new InvalidDataException($"Artifact path '{artifactPath}' must be relative to the baseline directory.");
		}

		string fullBaselineDirectory = Path.GetFullPath(baselineDirectory)
			.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		string fullArtifactPath = Path.GetFullPath(Path.Combine(fullBaselineDirectory, artifactPath));
		string requiredPrefix = fullBaselineDirectory + Path.DirectorySeparatorChar;
		if (!fullArtifactPath.StartsWith(requiredPrefix, StringComparison.Ordinal))
		{
			throw new InvalidDataException($"Artifact path '{artifactPath}' escapes the baseline directory.");
		}

		return fullArtifactPath;
	}

	private static string ReadRequiredString(JsonElement element, string propertyName, string context)
	{
		string? value = ReadOptionalString(element, propertyName);
		return !string.IsNullOrWhiteSpace(value)
			? value
			: throw new InvalidDataException($"{context} is missing required string property '{propertyName}'.");
	}

	private static string? ReadOptionalString(JsonElement element, string propertyName) =>
		element.TryGetProperty(propertyName, out JsonElement property) &&
		property.ValueKind == JsonValueKind.String
			? property.GetString()
			: null;

	private static int ReadRequiredInt(JsonElement element, string propertyName, string context) =>
		element.TryGetProperty(propertyName, out JsonElement property) && property.TryGetInt32(out int value)
			? value
			: throw new InvalidDataException($"{context} is missing required integer property '{propertyName}'.");

	private static long? ReadOptionalInt64(JsonElement element, string propertyName) =>
		element.TryGetProperty(propertyName, out JsonElement property) && property.TryGetInt64(out long value)
			? value
			: null;
}
