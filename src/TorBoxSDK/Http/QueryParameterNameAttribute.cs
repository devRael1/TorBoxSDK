namespace TorBoxSDK.Http;

/// <summary>
/// Specifies the query parameter name that a property maps to.
/// This attribute is used on options model properties as metadata
/// for API documentation and discoverability.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QueryParameterNameAttribute"/> class.
/// </remarks>
/// <param name="name">The query parameter name used in the HTTP query string.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class QueryParameterNameAttribute(string name) : Attribute
{

	/// <summary>
	/// Gets the query parameter name used in the HTTP query string.
	/// </summary>
	public string Name { get; } = name;
}
