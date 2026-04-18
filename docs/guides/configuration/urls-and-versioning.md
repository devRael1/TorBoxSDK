---
uid: configuration-urls-versioning
title: Base URLs and Versioning
description: Understand endpoint host configuration, version composition, and trailing slash behavior.
---

# Base URLs and Versioning

Correct base URLs are critical because SDK endpoints use relative paths.

## URL composition model

- Main host + API version produces Main versioned URL
- Relay host + API version produces Relay versioned URL
- Search uses its own host directly

## Trailing slash guidance

Keep trailing slashes in base URLs.

This prevents accidental path concatenation issues with relative endpoint paths in `HttpClient` requests.

## Example

```csharp
services.AddTorBox(options =>
{
    options.MainApiBaseUrl = "https://api.torbox.app/";
    options.RelayApiBaseUrl = "https://relay.torbox.app/";
    options.SearchApiBaseUrl = "https://search-api.torbox.app/";
    options.ApiVersion = "v1";
});
```

## When to customize

Customize only if:

- using a test/staging gateway
- routing through an internal proxy
- validating future API versions in controlled environments

For most users, official defaults are best.
