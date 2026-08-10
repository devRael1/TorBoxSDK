using System.Security.Cryptography;
using System.Text;
using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.OpenApi;

/// <summary>
/// Verifies the integrity and declared availability of the versioned contract baseline.
/// </summary>
[Trait("Category", "Contract")]
public sealed class OpenApiSnapshotIntegrityTests
{
	/// <summary>
	/// Provides the captured snapshots and their immutable provenance recorded in the baseline manifest.
	/// </summary>
	public static TheoryData<string, string, string, string, long, string> CapturedSnapshots()
	{
		TheoryData<string, string, string, string, long, string> data = [];
		data.Add(
			"main-openapi",
			"main",
			"openapi",
			"main.openapi.json",
			96363,
			"6017d1e0162a24315398eff9378b942a01816c305b9b122ee301182e932a94a4");
		data.Add(
			"relay-openapi",
			"relay",
			"openapi",
			"relay.openapi.json",
			1360,
			"71e5eeb37ab6cf0c8284e477c966367b71ba0fcd7becb8f79fe66ddaa4c4bb1e");
		data.Add(
			"main-postman",
			"main",
			"postman-public-api-collection",
			"main.postman.collection.json",
			2012155,
			"82699519a936b9f8dfd83e0a9871b4a222a9a97a5a56fa5e72f73cfad512c0b6");
		return data;
	}

	/// <summary>
	/// Provides sources that were intentionally recorded as unavailable rather than fabricated.
	/// </summary>
	public static TheoryData<string> UnavailableSnapshots()
	{
		TheoryData<string> data = [];
		data.Add("search-openapi");
		data.Add("search-postman");
		data.Add("relay-postman");
		return data;
	}

	[Theory]
	[MemberData(nameof(CapturedSnapshots))]
	public void Load_CapturedSnapshot_ProvidesVerifiedManifestIntegrity(
		string snapshotId,
		string expectedFamily,
		string expectedFormat,
		string expectedArtifactPath,
		long expectedContentLength,
		string expectedSha256)
	{
		// Arrange
		ContractBaseline baseline = ContractBaselineReader.Load();

		// Act
		ContractSnapshot snapshot = baseline.GetSnapshot(snapshotId);
		byte[] content = File.ReadAllBytes(snapshot.FullPath);
		string actualSha256 = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();

		// Assert
		Assert.True(snapshot.Descriptor.IsCaptured);
		Assert.Equal(expectedFamily, snapshot.Descriptor.Family);
		Assert.Equal(expectedFormat, snapshot.Descriptor.Format);
		Assert.Equal(expectedArtifactPath, snapshot.Descriptor.ArtifactPath);
		Assert.Equal(expectedContentLength, snapshot.Descriptor.ContentLength);
		Assert.Equal(expectedContentLength, content.LongLength);
		Assert.Equal(expectedSha256, snapshot.Descriptor.Sha256);
		Assert.Equal(expectedSha256, actualSha256);
		Assert.NotEmpty(snapshot.ReadUtf8Text());
	}

	[Theory]
	[MemberData(nameof(UnavailableSnapshots))]
	public void Load_UnavailableSnapshot_RetainsReasonWithoutArtifact(string snapshotId)
	{
		// Arrange
		ContractBaseline baseline = ContractBaselineReader.Load();

		// Act
		ContractSnapshotDescriptor descriptor = baseline.Descriptors[snapshotId];
		bool hasSnapshot = baseline.Snapshots.ContainsKey(snapshotId);

		// Assert
		Assert.False(descriptor.IsCaptured);
		Assert.False(hasSnapshot);
		Assert.Null(descriptor.ArtifactPath);
		Assert.False(string.IsNullOrWhiteSpace(descriptor.Reason));
	}

	[Fact]
	public void Load_TamperedCapturedArtifact_ThrowsIntegrityError()
	{
		// Arrange
		using TemporaryBaselineDirectory temporaryDirectory = new();
		const string artifactContent = """{"openapi":"3.0.0"}""";
		long contentLength = Encoding.UTF8.GetByteCount(artifactContent);
		string manifest = $$"""
			{
			  "schemaVersion": 1,
			  "sources": [
			    {
			      "id": "main-openapi",
			      "family": "main",
			      "format": "openapi",
			      "availability": "captured",
			      "artifactPath": "main.openapi.json",
			      "sourceUrl": "https://api.example.test/openapi.json",
			      "contentLength": {{contentLength}},
			      "sha256": "0000000000000000000000000000000000000000000000000000000000000000"
			    }
			  ]
			}
			""";
		temporaryDirectory.WriteFile("manifest.json", manifest);
		temporaryDirectory.WriteFile("main.openapi.json", artifactContent);
		Action action = () => ContractBaselineReader.Load(temporaryDirectory.Path);

		// Act
		InvalidDataException exception = Assert.Throws<InvalidDataException>(action);

		// Assert
		Assert.Contains("SHA-256", exception.Message);
	}

	private sealed class TemporaryBaselineDirectory : IDisposable
	{
		public TemporaryBaselineDirectory()
		{
			Path = System.IO.Path.Combine(
				System.IO.Path.GetTempPath(),
				$"TorBoxSDK.SchemaValidationTests.{Guid.NewGuid():N}");
			Directory.CreateDirectory(Path);
		}

		public string Path { get; }

		public void WriteFile(string relativePath, string content)
		{
			string fullPath = System.IO.Path.Combine(Path, relativePath);
			string? directory = System.IO.Path.GetDirectoryName(fullPath);
			if (directory is not null)
			{
				Directory.CreateDirectory(directory);
			}

			File.WriteAllText(fullPath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		}

		public void Dispose()
		{
			if (Directory.Exists(Path))
			{
				Directory.Delete(Path, recursive: true);
			}
		}
	}
}
