# Security Policy

## Supported Versions

The following versions of TorBoxSDK are currently supported with security updates:

| Target Framework | Supported          |
| ---------------- | ------------------ |
| net10.0          | :white_check_mark: |
| net9.0           | :white_check_mark: |
| net8.0           | :white_check_mark: |
| net7.0           | :white_check_mark: |
| net6.0           | :white_check_mark: |

## Reporting a Vulnerability

**Please do NOT report security vulnerabilities through public GitHub issues.**

If you discover a security vulnerability in TorBoxSDK, please report it
responsibly using GitHub's private security advisory feature:

**[Report a vulnerability](https://github.com/devRael1/TorBoxSDK/security/advisories/new)**

When reporting, please include:

- A description of the vulnerability
- Steps to reproduce the issue
- The potential impact of the vulnerability
- Any suggested fixes, if applicable

You should receive an initial response within 72 hours acknowledging your
report. We will work with you to understand the issue and coordinate a fix
before any public disclosure.

## Security Best Practices

When using TorBoxSDK, please follow these security practices:

- **Never commit API keys** or tokens to source control. Use environment
  variables or a secrets manager to store your TorBox API key.
- **Never hardcode credentials** in your application code, configuration files,
  or samples.
- Use `.gitignore` and tools like `git-secrets` to prevent accidental commits of
  sensitive data.
- Rotate your API key immediately if you suspect it has been exposed.

## Disclosure Policy

When a security vulnerability is reported, we will:

1. Confirm the vulnerability and determine its impact.
2. Develop and test a fix.
3. Release a patched version.
4. Publicly disclose the vulnerability after the fix is available.

We appreciate your help in keeping TorBoxSDK and its users safe.
