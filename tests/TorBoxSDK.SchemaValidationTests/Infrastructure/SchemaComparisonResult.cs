namespace TorBoxSDK.SchemaValidationTests.Infrastructure;

/// <summary>
/// Holds the results of comparing an OpenAPI schema definition with its corresponding SDK model type.
/// </summary>
/// <param name="SchemaName">The OpenAPI schema name (e.g., <c>"ControlTorrent"</c>).</param>
/// <param name="ModelType">The SDK C# type mapped to this schema.</param>
/// <param name="FieldsMissingInSdk">
/// JSON property names present in the OpenAPI schema but absent in the SDK model.
/// A non-empty list indicates the SDK model is out of date with the API specification.
/// </param>
/// <param name="FieldsExtraInSdk">
/// JSON property names declared in the SDK model (via <c>[JsonPropertyName]</c>) but absent
/// from the OpenAPI schema. These may be undocumented API parameters or SDK-only extensions.
/// </param>
internal sealed record SchemaComparisonResult(
	string SchemaName,
	Type ModelType,
	IReadOnlyList<string> FieldsMissingInSdk,
	IReadOnlyList<string> FieldsExtraInSdk);
