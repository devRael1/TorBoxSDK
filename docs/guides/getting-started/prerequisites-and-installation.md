---
uid: getting-started-prerequisites-installation
title: Prerequisites and Installation
description: Environment prerequisites, package installation, and secure API key setup for TorBoxSDK.
---

# Prerequisites and Installation

This guide covers everything needed before your first TorBoxSDK call.

## Prerequisites

- .NET 6.0 or newer
- a valid TorBox API key
- outbound HTTPS access to TorBox endpoints

## Install package

```bash
dotnet add package TorBoxSDK
```

Or with NuGet Package Manager:

```powershell
Install-Package TorBoxSDK
```

## Verify installation quickly

```bash
dotnet list package | findstr TorBoxSDK
```

Expected outcome: `TorBoxSDK` appears in package list.

## Store your API key safely

Avoid hardcoding the key in source code. Prefer environment variables or secret stores.

### Local development (PowerShell)

```powershell
$env:TORBOX_API_KEY = "your-api-key"
```

### Production recommendations

- ASP.NET Core user-secrets or KeyVault/Secrets Manager
- CI/CD secret variables
- no plain text key in `appsettings.json` committed to Git

## Minimal sanity check

```csharp
using TorBoxSDK;

string apiKey = Environment.GetEnvironmentVariable("TORBOX_API_KEY")
    ?? throw new InvalidOperationException("Missing TORBOX_API_KEY.");

using TorBoxClient client = new(apiKey);
```

If this runs without throwing, installation and key retrieval are ready.
