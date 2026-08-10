# Severity Guide — Code Review Findings

Every finding must be rated with one of four levels. Use this guide to select the correct level.

---

## 🔴 CRITICAL — Must fix before merge

The code is broken, insecure, or introduces a runtime failure. Merging as-is causes:
- A crash, deadlock, or data corruption in production
- A security vulnerability (OWASP Top 10)
- A silent data loss or incorrect API response

**Automatic CRITICAL triggers:**
| Finding | Reason |
|---------|--------|
| `.Result` or `.Wait()` on a `Task` in library code | Deadlock in ASP.NET / synchronization context |
| `async void` on a non-event method | Exceptions swallowed, GC pressure |
| Hardcoded API key, token, or password | Secret exposure in repository |
| `NullReferenceException` risk on guaranteed-to-be-called path | Runtime crash |
| HTTP client base URL set from unvalidated user input | SSRF vulnerability |
| Exception with stack trace returned to external caller | Information disclosure |
| Missing `await` on an async method that has side effects | Fire-and-forget data loss |

**Example:**
```csharp
// 🔴 CRITICAL — L42: Synchronous block on Task causes deadlock in ASP.NET context.
// Replace with: string json = await _httpClient.GetStringAsync(url, ct).ConfigureAwait(false);
string json = _httpClient.GetStringAsync(url).Result;
```

---

## 🟠 MAJOR — Strong recommendation to fix

The code compiles and runs but violates a core project convention, degrades reliability, or creates a significant maintenance risk. Should not merge without discussion.

**Automatic MAJOR triggers:**
| Finding | Reason |
|---------|--------|
| Missing `ConfigureAwait(false)` in `src/` | Not a library-safe async pattern |
| `CancellationToken` not propagated to inner async calls | Cancellation silently ignored |
| Endpoint method implemented in the wrong resource client | Architecture violation |
| `TorBoxResponse<T>` not used — raw deserialization | Bypasses error handling contract |
| `new JsonSerializerOptions()` inside a hot method | High allocation, perf regression |
| Public `class` property with `set` on a response model | Breaks immutability contract |
| Missing `ArgumentNullException.ThrowIfNull` on required public params | Silent null bug |
| No XML documentation on a `public` method/class | Public API not self-documenting |
| Test makes a real HTTP call to `api.torbox.app` | Unit test non-isolable, CI failure |
| `async void` test method | Exceptions swallowed in xUnit |
| No `[JsonPropertyName]` on a JSON-serialized property | Silent deserialization failure |

**Example:**
```csharp
// 🟠 MAJOR — L17: CancellationToken `ct` is accepted but never passed to the inner HTTP call.
// All inner awaits must forward ct: await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
```

---

## 🟡 MINOR — Should fix

The code works and follows most conventions but has a quality issue that makes it harder to maintain, read, or extend.

**Common MINOR triggers:**
| Finding | Reason |
|---------|--------|
| `var` used where type is not obvious | Reduces readability |
| Block-body method that should be expression-bodied | Verbosity |
| Block-body namespace instead of file-scoped | File-scope is mandatory |
| Missing `Async` suffix on an async method | Naming violation |
| Private field missing `_` prefix | Naming violation |
| `List<T>` property on response model (should be `IReadOnlyList<T>`) | Mutability leaked |
| `string?` on a field that should never be null (per API docs) | Wrong nullability |
| `DateTime` instead of `DateTimeOffset` for API timestamps | Timezone ambiguity |
| Blank lines missing between AAA sections in a test | Readability |
| `[Fact]` duplicated instead of `[Theory]` | Unnecessary duplication |
| `ToString()` called on value type in log message string | Minor boxing allocation |
| `using` directives out of order | Style violation |

**Example:**
```csharp
// 🟡 MINOR — L8: Block-body namespace. Use file-scoped namespace: `namespace TorBoxSDK.Clients;`
namespace TorBoxSDK.Clients
{
    ...
}
```

---

## 🔵 NITPICK — Polish

Stylistic or subjective observation that does not affect correctness or maintainability. Nice to have.

**Common NITPICK triggers:**
| Observation | |
|-------------|--|
| Variable name could be more descriptive | `data` → `torrentData` |
| Comment is slightly redundant or obvious | Comment mirrors the code exactly |
| Order of properties in a model could match API documentation order | Consistency |
| Implicit empty collection could use `[]` instead of `new List<T>()` | Modern idiom |
| Minor whitespace inconsistency | Extra blank line |
| Parameter names in XML doc don't quite describe the intent | Docs precision |
| Method could return `IReadOnlyList<T>` instead of `IList<T>` | Preferred interface |

**Example:**
```csharp
// 🔵 NITPICK — L31: Variable name `d` is ambiguous. Consider `download` or `torrentDownload`.
foreach (var d in downloads) { ... }
```

---

## Verdict Rules

Select the verdict based on the highest severity finding present:

| Findings present | Verdict |
|-----------------|---------|
| No findings at all | `APPROVED` |
| Only NITPICK findings | `APPROVED` |
| Only MINOR and/or NITPICK | `APPROVED WITH MINOR ISSUES` |
| At least one MAJOR (no CRITICAL) | `CHANGES REQUESTED` |
| At least one CRITICAL | `BLOCKED` |

**Additional rules:**
- `BLOCKED` is non-negotiable — the file must not merge until all CRITICALs are resolved.
- `CHANGES REQUESTED` should clearly list all MAJORs that require resolution.
- `APPROVED WITH MINOR ISSUES` must list the MINORs so the author can address them in a follow-up.
- `APPROVED` still lists NITPICK observations when present.

---

## Severity Override Cases

Some contexts change the default severity:

| Context | Adjustment |
|---------|------------|
| MAJOR in a `samples/` file | Upgrade to CRITICAL — samples are teaching material; wrong patterns will be adopted |
| MINOR in a public `interface` | Upgrade to MAJOR — interface violations lock into public API |
| NITPICK in a name that appears in 10+ places | Upgrade to MINOR — fix cost is low; inconsistency spreads |
| CRITICAL in a `tests/` file only (not `src/`) | Keep CRITICAL but note it does not affect production code |
