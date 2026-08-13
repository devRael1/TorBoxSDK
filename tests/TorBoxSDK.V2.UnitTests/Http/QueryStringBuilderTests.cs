using TorBoxSDK.Http;

namespace TorBoxSDK.V2.UnitTests.Http;

public sealed class QueryStringBuilderTests
{
	[Fact]
	public void Build_WithEscapedNamesAndValues_OmitsNullValues()
	{
		// Arrange
		(string Name, string? Value)[] parameters =
		[
			("search term", "a/b?c=d&é"),
			("ignored", null),
			("empty", string.Empty),
		];

		// Act
		string query = QueryStringBuilder.Build(parameters);

		// Assert
		Assert.Equal("?search%20term=a%2Fb%3Fc%3Dd%26%C3%A9&empty=", query);
	}

	[Fact]
	public void Build_WithOnlyNullValues_ReturnsAnEmptyQuery()
	{
		// Arrange
		(string Name, string? Value)[] parameters =
		[
			("first", null),
			("second", null),
		];

		// Act
		string query = QueryStringBuilder.Build(parameters);

		// Assert
		Assert.Equal(string.Empty, query);
	}
}
