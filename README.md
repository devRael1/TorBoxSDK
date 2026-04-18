# TorBoxSDK

[![NuGet](https://img.shields.io/badge/NuGet-TorBoxSDK-blue?logo=nuget)](https://www.nuget.org/packages/TorBoxSDK)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/devRael1/TorBoxSDK/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-net6.0%20%7C%20net7.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-purple)]()

**Important:** This SDK is unofficial and is not affiliated with or endorsed by TorBox.

TorBoxSDK is an open-source, MIT-licensed C# SDK for the [TorBox API](https://api-docs.torbox.app/). It gives .NET applications typed access to the TorBox Main, Search, and Relay APIs with dependency injection support and consistent response handling.

## Key Features

- Covers the TorBox Main, Search, and Relay APIs through a single root client
- Organizes the Main API into focused resource clients (Torrents, Usenet, Web Downloads, User, Notifications, RSS, Integrations)
- Multi-targets .NET 6 through .NET 10
- Integrates with `IServiceCollection`, `IHttpClientFactory`, and configuration binding
- Uses the standard `TorBoxResponse` envelope and surfaces API failures through `TorBoxException`
- Full XML documentation and SourceLink support

## Installation

```bash
dotnet add package TorBoxSDK
```

## Quick Start

```csharp
using Microsoft.Extensions.DependencyInjection;
using TorBoxSDK;
using TorBoxSDK.DependencyInjection;

ServiceCollection services = new();
services.AddTorBox(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
        ?? throw new InvalidOperationException("Set TORBOX_API_KEY.");
});

using ServiceProvider provider = services.BuildServiceProvider();
ITorBoxClient client = provider.GetRequiredService<ITorBoxClient>();

var torrents = await client.Main.Torrents.GetMyTorrentListAsync();
```

Or standalone mode:

```csharp
using TorBoxClient client = new("your-api-key");
var result = await client.Main.User.GetMeAsync();
```

## Documentation

- [Getting Started Overview](docs/guides/getting-started.md) - Install and make your first request
- [Architecture Overview](docs/guides/architecture.md) - Understand the client hierarchy
- [Configuration Overview](docs/guides/configuration.md) - Configure options and base URLs
- [Error Handling Overview](docs/guides/error-handling.md) - Handle API errors
- [API Reference](docs/guides/api-reference.md) - Full API documentation
- [Examples](src/TorBoxSDK.Examples/) - 37 runnable scenarios

## Contributing

Contributions are welcome! Read the [Contributing Guide](CONTRIBUTING.md) for development workflow and review the [Code of Conduct](CODE_OF_CONDUCT.md).

- [Report vulnerabilities](SECURITY.md) privately
- [File issues and feature requests](https://github.com/devRael1/TorBoxSDK/issues)

## License

MIT License. See [LICENSE](LICENSE) for details.
