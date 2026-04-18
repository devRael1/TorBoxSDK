namespace TorBoxSDK.Http;

/// <summary>
/// Specifies the query parameter name that a property maps to when
/// building an HTTP query string. This attribute is used on Options
/// record properties to document the API parameter name.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class QueryParameterNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The query parameter name used in the HTTP query string.</param>
    public QueryParameterNameAttribute(string name) => Name = name;

    /// <summary>
    /// Gets the query parameter name used in the HTTP query string.
    /// </summary>
    public string Name { get; }
}
