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
			"Added:response-content:GET /widgets/{id}:200:text/plain",
			"Modified:schema:Widget"
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
	public void CompareOpenApi_ContentlessResponsesChanged_ReturnsStableResponseDifferences()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets": {
			      "get": {
			        "responses": {
			          "202": { "description": "Accepted" },
			          "204": { "description": "No content" },
			          "404": { "description": "Widget was not found" }
			        }
			      }
			    }
			  }
			}
			""";
		const string candidateJson = """
			{
			  "paths": {
			    "/widgets": {
			      "get": {
			        "responses": {
			          "201": { "description": "Created" },
			          "202": { "description": "Accepted" },
			          "404": { "description": "No matching widget was found" }
			        }
			      }
			    }
			  }
			}
			""";
		string[] expectedDifferences =
		[
			"Added:response:GET /widgets:201",
			"Removed:response:GET /widgets:204",
			"Modified:response:GET /widgets:404"
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
	public void CompareOpenApi_RequestBodyRequiredChanged_ReturnsStableRequestBodyDifference()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets": {
			      "post": {
			        "requestBody": {
			          "required": false,
			          "content": {
			            "application/json": { "schema": { "type": "object" } }
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
			    "/widgets": {
			      "post": {
			        "requestBody": {
			          "required": true,
			          "content": {
			            "application/json": { "schema": { "type": "object" } }
			          }
			        }
			      }
			    }
			  }
			}
			""";

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractComparisonResult repeatedResult = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractDifference difference = Assert.Single(result.Differences);

		// Assert
		Assert.False(result.IsEquivalent);
		Assert.Equal("Modified:request-body:POST /widgets", DescribeDifference(difference));
		Assert.Equal("required=false;$ref=absent", difference.BaselineValue);
		Assert.Equal("required=true;$ref=absent", difference.CandidateValue);
		Assert.Equal(result.Differences, repeatedResult.Differences);
	}

	[Fact]
	public void CompareOpenApi_RequestBodyReferenceChanged_ReturnsStableRequestBodyDifference()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets": {
			      "post": {
			        "requestBody": { "$ref": "#/components/requestBodies/CreateWidget" }
			      }
			    }
			  }
			}
			""";
		const string candidateJson = """
			{
			  "paths": {
			    "/widgets": {
			      "post": {
			        "requestBody": { "$ref": "#/components/requestBodies/UpdateWidget" }
			      }
			    }
			  }
			}
			""";

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractComparisonResult repeatedResult = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractDifference difference = Assert.Single(result.Differences);

		// Assert
		Assert.False(result.IsEquivalent);
		Assert.Equal("Modified:request-body:POST /widgets", DescribeDifference(difference));
		Assert.Contains("#/components/requestBodies/CreateWidget", difference.BaselineValue);
		Assert.Contains("#/components/requestBodies/UpdateWidget", difference.CandidateValue);
		Assert.Equal(result.Differences, repeatedResult.Differences);
	}

	[Fact]
	public void CompareOpenApi_ParameterSerializationChanged_ReturnsStableParameterDifference()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets": {
			      "get": {
			        "parameters": [
			          {
			            "name": "ids",
			            "in": "query",
			            "style": "form",
			            "explode": true,
			            "allowReserved": false,
			            "schema": { "type": "array", "items": { "type": "string" } }
			          }
			        ]
			      }
			    }
			  }
			}
			""";
		const string candidateJson = """
			{
			  "paths": {
			    "/widgets": {
			      "get": {
			        "parameters": [
			          {
			            "name": "ids",
			            "in": "query",
			            "style": "pipeDelimited",
			            "explode": false,
			            "allowReserved": true,
			            "schema": { "type": "array", "items": { "type": "string" } }
			          }
			        ]
			      }
			    }
			  }
			}
			""";

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractComparisonResult repeatedResult = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractDifference difference = Assert.Single(result.Differences);

		// Assert
		Assert.False(result.IsEquivalent);
		Assert.Equal("Modified:parameter:GET /widgets:query:ids", DescribeDifference(difference));
		Assert.Contains("style=\"form\"", difference.BaselineValue);
		Assert.Contains("explode=true", difference.BaselineValue);
		Assert.Contains("allowReserved=false", difference.BaselineValue);
		Assert.Contains("style=\"pipeDelimited\"", difference.CandidateValue);
		Assert.Contains("explode=false", difference.CandidateValue);
		Assert.Contains("allowReserved=true", difference.CandidateValue);
		Assert.Equal(result.Differences, repeatedResult.Differences);
	}

	[Fact]
	public void CompareOpenApi_ReorderedInlineObjectSchema_ReturnsEquivalent()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets": {
			      "post": {
			        "requestBody": {
			          "content": {
			            "application/json": {
			              "schema": {
			                "type": "object",
			                "required": ["name", "metadata"],
			                "properties": {
			                  "name": { "type": "string" },
			                  "metadata": {
			                    "type": "object",
			                    "required": ["createdAt", "tags"],
			                    "properties": {
			                      "createdAt": { "type": "string", "format": "date-time" },
			                      "tags": { "type": "array", "items": { "type": "string" } }
			                    }
			                  }
			                }
			              }
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
			    "/widgets": {
			      "post": {
			        "requestBody": {
			          "content": {
			            "application/json": {
			              "schema": {
			                "properties": {
			                  "metadata": {
			                    "properties": {
			                      "tags": { "items": { "type": "string" }, "type": "array" },
			                      "createdAt": { "format": "date-time", "type": "string" }
			                    },
			                    "required": ["tags", "createdAt"],
			                    "type": "object"
			                  },
			                  "name": { "type": "string" }
			                },
			                "required": ["metadata", "name"],
			                "type": "object"
			              }
			            }
			          }
			        }
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
	public void CompareOpenApi_InlineNestedObjectPropertiesAndRequiredChanged_ReturnsContentDifference()
	{
		// Arrange
		const string baselineJson = """
			{
			  "paths": {
			    "/widgets": {
			      "post": {
			        "requestBody": {
			          "content": {
			            "application/json": {
			              "schema": {
			                "type": "object",
			                "properties": {
			                  "metadata": {
			                    "type": "object",
			                    "required": ["name"],
			                    "properties": { "name": { "type": "string" } }
			                  }
			                }
			              }
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
			    "/widgets": {
			      "post": {
			        "requestBody": {
			          "content": {
			            "application/json": {
			              "schema": {
			                "type": "object",
			                "properties": {
			                  "metadata": {
			                    "type": "object",
			                    "required": ["kind", "name"],
			                    "properties": {
			                      "kind": { "type": "string" },
			                      "name": { "type": "string" }
			                    }
			                  }
			                }
			              }
			            }
			          }
			        }
			      }
			    }
			  }
			}
			""";

		// Act
		ContractComparisonResult result = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractComparisonResult repeatedResult = ContractSnapshotComparer.CompareOpenApi(
			baselineJson,
			candidateJson);
		ContractDifference difference = Assert.Single(result.Differences);

		// Assert
		Assert.False(result.IsEquivalent);
		Assert.Equal("Modified:request-content:POST /widgets:application/json", DescribeDifference(difference));
		Assert.Equal(result.Differences, repeatedResult.Differences);
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
