using TorBoxSDK.SchemaValidationTests.Infrastructure;

namespace TorBoxSDK.SchemaValidationTests.OpenApi;

/// <summary>
/// Verifies deterministic, semantic comparison of the versioned OpenAPI and Postman contracts.
/// </summary>
[Trait("Category", "Contract")]
public sealed class OpenApiContractComparisonTests
{
	/// <summary>
	/// Provides every captured baseline source supported by the offline comparer.
	/// </summary>
	public static TheoryData<string> CapturedSnapshotIds()
	{
		TheoryData<string> data = [];
		data.Add("main-openapi");
		data.Add("relay-openapi");
		data.Add("main-postman");
		return data;
	}

	[Theory]
	[MemberData(nameof(CapturedSnapshotIds))]
	public void Compare_CapturedSnapshotWithItsOwnContent_ReturnsEquivalent(string snapshotId)
	{
		// Arrange
		ContractBaseline baseline = ContractBaselineReader.Load();
		ContractSnapshot snapshot = baseline.GetSnapshot(snapshotId);

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.Compare(
			snapshot,
			snapshot.ReadUtf8Text());

		// Assert
		Assert.True(result.IsEquivalent);
		Assert.Empty(result.Differences);
	}

	[Fact]
	public void CompareOpenApi_ReorderedSemanticJson_ReturnsEquivalent()
	{
		// Arrange
		const string baselineJson = """
			{
			  "components": {
			    "schemas": {
			      "Widget": {
			        "type": "object",
			        "required": ["name"],
			        "properties": {
			          "name": { "type": "string", "enum": ["small", "large"] },
			          "labels": { "type": "array", "items": { "type": "string" } }
			        }
			      }
			    }
			  },
			  "paths": {
			    "/widgets/{id}": {
			      "parameters": [
			        { "name": "id", "in": "path", "required": true, "schema": { "type": "string", "format": "uuid" } }
			      ],
			      "get": {
			        "operationId": "getWidget",
			        "parameters": [
			          { "name": "include", "in": "query", "required": false, "schema": { "type": "string", "nullable": true } }
			        ],
			        "responses": {
			          "200": {
			            "content": {
			              "application/json": { "schema": { "$ref": "#/components/schemas/Widget" } }
			            }
			          }
			        }
			      }
			    }
			  }
			}
			""";
		const string candidateJson = """
			{
			  "paths": {
			    "/widgets/{id}": {
			      "get": {
			        "responses": {
			          "200": {
			            "content": {
			              "application/json": { "schema": { "$ref": "#/components/schemas/Widget" } }
			            }
			          }
			        },
			        "parameters": [
			          { "schema": { "nullable": true, "type": "string" }, "required": false, "in": "query", "name": "include" }
			        ],
			        "operationId": "getWidget"
			      },
			      "parameters": [
			        { "schema": { "format": "uuid", "type": "string" }, "required": true, "in": "path", "name": "id" }
			      ]
			    }
			  },
			  "components": {
			    "schemas": {
			      "Widget": {
			        "properties": {
			          "labels": { "items": { "type": "string" }, "type": "array" },
			          "name": { "enum": ["large", "small"], "type": "string" }
			        },
			        "required": ["name"],
			        "type": "object"
			      }
			    }
			  }
			}
			""";

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);

		// Assert
		Assert.True(result.IsEquivalent);
		Assert.Empty(result.Differences);
	}

	[Fact]
	public void CompareOpenApi_ChangedContract_ReturnsStableSemanticDifferences()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets/{id}": {
			      "get": {
			        "operationId": "getWidget",
			        "parameters": [
			          { "name": "id", "in": "query", "required": false, "schema": { "type": "string", "format": "uuid", "nullable": true, "default": "widget-1" } }
			        ],
			        "requestBody": {
			          "content": {
			            "application/json": { "schema": { "type": "object" } }
			          }
			        },
			        "responses": {
			          "200": {
			            "content": {
			              "application/json": { "schema": { "$ref": "#/components/schemas/Widget" } }
			            }
			          }
			        }
			      },
			      "post": { "operationId": "createWidget" }
			    }
			  },
			  "components": {
			    "schemas": {
			      "Widget": {
			        "type": "object",
			        "required": ["name", "labels"],
			        "properties": {
			          "name": { "type": "string", "enum": ["small", "large"] },
			          "labels": { "type": "array", "items": { "type": "string" } }
			        }
			      }
			    }
			  }
			}
			""";
		const string candidateJson = """
			{
			  "paths": {
			    "/widgets/{id}": {
			      "delete": { "operationId": "deleteWidget" },
			      "get": {
			        "operationId": "getWidgetV2",
			        "parameters": [
			          { "name": "id", "in": "query", "required": true, "schema": { "type": "integer", "format": "int32", "nullable": false, "default": 2 } }
			        ],
			        "requestBody": {
			          "content": {
			            "application/xml": { "schema": { "type": "object" } }
			          }
			        },
			        "responses": {
			          "200": {
			            "content": {
			              "text/plain": { "schema": { "type": "string" } }
			            }
			          }
			        }
			      }
			    }
			  },
			  "components": {
			    "schemas": {
			      "Widget": {
			        "type": "object",
			        "required": [],
			        "properties": {
			          "name": { "type": "string", "enum": ["large", "medium"] }
			        }
			      }
			    }
			  }
			}
			""";
		string[] expectedDifferences =
		[
			"Removed:field:Widget.labels",
			"Modified:field:Widget.name",
			"Added:operation:DELETE /widgets/{id}",
			"Modified:operation:GET /widgets/{id}",
			"Removed:operation:POST /widgets/{id}",
			"Modified:parameter:GET /widgets/{id}:query:id",
			"Removed:request-content:GET /widgets/{id}:application/json",
			"Added:request-content:GET /widgets/{id}:application/xml",
			"Removed:response-content:GET /widgets/{id}:200:application/json",
			"Added:response-content:GET /widgets/{id}:200:text/plain"
		];

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractComparisonResult repeatedResult = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		string[] differences = result.Differences.Select(DescribeDifference).ToArray();
		string[] repeatedDifferences = repeatedResult.Differences.Select(DescribeDifference).ToArray();

		// Assert
		Assert.False(result.IsEquivalent);
		Assert.Equal(expectedDifferences, differences);
		Assert.Equal(expectedDifferences, repeatedDifferences);
	}

	[Fact]
	public void ComparePostmanCollection_ChangedLegacyRequest_ReturnsOperationAndContentDifferences()
	{
		// Arrange
		const string baselineJson = """
			{
			  "data": {
			    "requests": [
			      {
			        "method": "GET",
			        "url": "https://api.example.test/widgets",
			        "queryParams": [ { "key": "include", "value": "files" } ],
			        "headerData": [ { "key": "Content-Type", "value": "application/json" } ],
			        "dataMode": "raw"
			      }
			    ]
			  }
			}
			""";
		const string candidateJson = """
			{
			  "data": {
			    "requests": [
			      {
			        "method": "POST",
			        "url": "https://api.example.test/widgets",
			        "queryParams": [ { "key": "include", "value": "files", "disabled": true } ],
			        "headerData": [ { "key": "Content-Type", "value": "application/x-www-form-urlencoded" } ],
			        "dataMode": "urlencoded"
			      }
			    ]
			  }
			}
			""";

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.ComparePostmanCollection(
			baselineJson,
			candidateJson);

		// Assert
		Assert.False(result.IsEquivalent);
		Assert.Equal(8, result.Differences.Count);
		Assert.Contains(result.Differences, difference =>
			difference is { Kind: ContractDifferenceKind.Removed, Category: "operation", Subject: "GET https://api.example.test/widgets" });
		Assert.Contains(result.Differences, difference =>
			difference is { Kind: ContractDifferenceKind.Added, Category: "operation", Subject: "POST https://api.example.test/widgets" });
		Assert.Contains(result.Differences, difference =>
			difference is { Kind: ContractDifferenceKind.Removed, Category: "request-content", Subject: "GET https://api.example.test/widgets:application/json" });
		Assert.Contains(result.Differences, difference =>
			difference is { Kind: ContractDifferenceKind.Added, Category: "request-content", Subject: "POST https://api.example.test/widgets:application/x-www-form-urlencoded" });
	}

	private static string DescribeDifference(ContractDifference difference) =>
		$"{difference.Kind}:{difference.Category}:{difference.Subject}";
}
