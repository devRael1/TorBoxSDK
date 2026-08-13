using System.Reflection;

namespace TorBoxSDK.V2.UnitTests.Configuration;

public sealed class TorBoxClientOptionsTests
{
	public static TheoryData<string?> BlankApiKeys => new()
	{
		null,
		string.Empty,
		"\u2003",
	};

	public static TheoryData<Action<TorBoxClientOptions>> MissingBaseUrlConfigurations => new()
	{
		static options => options.MainApiBaseUrl = null,
		static options => options.SearchApiBaseUrl = string.Empty,
		static options => options.RelayApiBaseUrl = "\u2003",
	};

	public static TheoryData<Action<TorBoxClientOptions>> NonAbsoluteBaseUrlConfigurations => new()
	{
		static options => options.MainApiBaseUrl = "/v1/api/",
		static options => options.SearchApiBaseUrl = "search/",
		static options => options.RelayApiBaseUrl = "v1/",
	};

	public static TheoryData<Action<TorBoxClientOptions>> BaseUrlConfigurationsWithoutTrailingSlash => new()
	{
		static options => options.MainApiBaseUrl = "https://main.example.test/v1/api",
		static options => options.SearchApiBaseUrl = "https://search.example.test",
		static options => options.RelayApiBaseUrl = "https://relay.example.test/v1",
	};

	[Fact]
	public void Options_ExposeMutableConfigurationBindableProperties()
	{
		// Arrange
		string[] propertyNames =
		[
			nameof(TorBoxClientOptions.ApiKey),
			nameof(TorBoxClientOptions.MainApiBaseUrl),
			nameof(TorBoxClientOptions.SearchApiBaseUrl),
			nameof(TorBoxClientOptions.RelayApiBaseUrl),
			nameof(TorBoxClientOptions.Timeout),
		];

		// Act
		PropertyInfo?[] properties = propertyNames
			.Select(static propertyName => typeof(TorBoxClientOptions).GetProperty(propertyName))
			.ToArray();

		// Assert
		Assert.All(properties, static property =>
		{
			Assert.NotNull(property);
			Assert.True(property.CanWrite);
		});
	}

	[Fact]
	public void Constructor_WithDefaultOptions_UsesTheReviewedResolvedFamilyUrls()
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();

		// Act
		using TorBoxClient client = new(options);

		// Assert
		Assert.Equal("https://api.torbox.app/v1/api/", options.MainApiBaseUrl);
		Assert.Equal("https://search-api.torbox.app/", options.SearchApiBaseUrl);
		Assert.Equal("https://relay.torbox.app/v1/", options.RelayApiBaseUrl);
		Assert.Equal(TimeSpan.FromSeconds(30), options.Timeout);
	}

	[Theory]
	[MemberData(nameof(BlankApiKeys))]
	public void Constructor_WithBlankApiKey_ThrowsArgumentException(string? apiKey)
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();
		options.ApiKey = apiKey;

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TorBoxClient(options));

		// Assert
		Assert.Equal(nameof(TorBoxClientOptions.ApiKey), exception.ParamName);
	}

	[Theory]
	[MemberData(nameof(MissingBaseUrlConfigurations))]
	public void Constructor_WithMissingFamilyBaseUrl_ThrowsArgumentException(Action<TorBoxClientOptions> configure)
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();
		configure(options);

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TorBoxClient(options));

		// Assert
		Assert.NotNull(exception.ParamName);
	}

	[Theory]
	[MemberData(nameof(NonAbsoluteBaseUrlConfigurations))]
	public void Constructor_WithNonAbsoluteFamilyBaseUrl_ThrowsArgumentException(Action<TorBoxClientOptions> configure)
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();
		configure(options);

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TorBoxClient(options));

		// Assert
		Assert.NotNull(exception.ParamName);
	}

	[Theory]
	[MemberData(nameof(BaseUrlConfigurationsWithoutTrailingSlash))]
	public void Constructor_WithFamilyBaseUrlWithoutTrailingSlash_ThrowsArgumentException(Action<TorBoxClientOptions> configure)
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();
		configure(options);

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TorBoxClient(options));

		// Assert
		Assert.NotNull(exception.ParamName);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void Constructor_WithNonPositiveTimeout_ThrowsArgumentException(int seconds)
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();
		options.Timeout = TimeSpan.FromSeconds(seconds);

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TorBoxClient(options));

		// Assert
		Assert.Equal(nameof(TorBoxClientOptions.Timeout), exception.ParamName);
	}

	[Fact]
	public void ValidateAndNormalize_WithTimeoutAboveHttpClientMaximum_ThrowsArgumentException()
	{
		// Arrange
		TorBoxClientOptions options = CreateValidOptions();
		options.Timeout = TimeSpan.FromDays(25);

		// Act
		ArgumentException exception = Assert.Throws<ArgumentException>(() => options.ValidateAndNormalize());

		// Assert
		Assert.Equal(nameof(TorBoxClientOptions.Timeout), exception.ParamName);
	}

	[Fact]
	public void ValidateAndNormalize_WithMaximumHttpClientTimeout_AcceptsTimeout()
	{
		// Arrange
		TimeSpan maximumHttpClientTimeout = TimeSpan.FromMilliseconds(int.MaxValue);
		TorBoxClientOptions options = CreateValidOptions();
		options.Timeout = maximumHttpClientTimeout;

		// Act
		ValidatedTorBoxClientOptions validatedOptions = options.ValidateAndNormalize();

		// Assert
		Assert.Equal(maximumHttpClientTimeout, validatedOptions.Timeout);
	}

	private static TorBoxClientOptions CreateValidOptions() => new()
	{
		ApiKey = "test-api-key",
	};
}
