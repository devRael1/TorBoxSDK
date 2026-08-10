# Report Template

Use this exact template for every code review. Do not invent a different format.

---

## Code Review — `<filename>`  

> **Scope:** `<full file path>`  
> **Instructions loaded:** `<list of instruction files loaded before review>`  
> **Reviewed:** `<date>`

---

### Summary

`<2–4 sentences: overall assessment of quality. State what the file does, whether conventions are followed overall, and what the main issues are. Be direct and specific.>`

---

### Dimension Check

For every dimension below, mark ✅ if no issues were found and it was explicitly verified, or omit it if it has findings listed under Findings.

| Dimension | Status |
|-----------|--------|
| 1. Correctness | ✅ / ⚠️ see findings |
| 2. Naming | ✅ / ⚠️ see findings |
| 3. Nullability | ✅ / ⚠️ see findings |
| 4. Type inference | ✅ / ⚠️ see findings |
| 5. Modern C# | ✅ / ⚠️ see findings |
| 6. Async/Await | ✅ / ⚠️ see findings |
| 7. Architecture | ✅ / ⚠️ see findings |
| 8. Response handling | ✅ / ⚠️ see findings |
| 9. Security | ✅ / ⚠️ see findings |
| 10. Performance | ✅ / ⚠️ see findings |
| 11. File organization | ✅ / ⚠️ see findings |
| 12. XML documentation | ✅ / ⚠️ see findings |

*For test files, also include:* Test naming / AAA structure / Assertions / HttpClient mocking / Test isolation  
*For sample files, also include:* API key handling / DI pattern / Error handling / Self-explanatory code

---

### Findings

#### 🔴 CRITICAL — Must fix before merge
`<Remove this section if there are no CRITICAL findings>`

- **[L{line}]** `<Exact description of what is wrong.>` **Impact:** `<Why this matters — what breaks, fails, or leaks.>` **Fix:** `<Concise description of the expected replacement, no full code blocks unless necessary.>`

#### 🟠 MAJOR — Strong recommendation to fix
`<Remove this section if there are no MAJOR findings>`

- **[L{line}]** `<Description.>` **Impact:** `<Why.>` **Fix:** `<What to do.>`

#### 🟡 MINOR — Should fix
`<Remove this section if there are no MINOR findings>`

- **[L{line}]** `<Description.>` **Fix:** `<Concise suggestion.>`

#### 🔵 NITPICK — Polish
`<Remove this section if there are no NITPICK findings>`

- **[L{line}]** `<Observation.>` `<Optional suggestion.>`

---

### Verdict

> **`<APPROVED | APPROVED WITH MINOR ISSUES | CHANGES REQUESTED | BLOCKED>`**  
> `<One sentence justification referencing the finding(s) that drive the verdict, or confirming all dimensions passed.>`

---

## Multi-file Review Template

When reviewing more than one file, use one section per file, then close with:

---

### Global Summary — `<folder or feature name>`

| File | Verdict | Highest Severity |
|------|---------|-----------------|
| `path/to/File1.cs` | APPROVED | — |
| `path/to/File2.cs` | CHANGES REQUESTED | 🟠 MAJOR |
| `path/to/File3.cs` | BLOCKED | 🔴 CRITICAL |

**Overall verdict:** `<BLOCKED if any file is BLOCKED; CHANGES REQUESTED if any MAJOR; otherwise aggregate>`

`<2–3 sentences: cross-cutting observations that span multiple files.>`

---

## Example Report (Single File)

---

## Code Review — `TorrentsClient.cs`

> **Scope:** `src/TorBoxSDK/Clients/TorrentsClient.cs`  
> **Instructions loaded:** `csharp-conventions.instructions.md`, `review-sdk-core.instructions.md`  
> **Reviewed:** 2026-04-04

---

### Summary

`TorrentsClient` implements the torrent resource client and covers the main list and create endpoints. General structure follows the primary constructor pattern and file-scoped namespace correctly. However, two blocking issues were found: synchronous task blocking on line 42 and missing `CancellationToken` propagation on four inner calls. XML documentation is absent on all public methods.

---

### Dimension Check

| Dimension | Status |
|-----------|--------|
| 1. Correctness | ✅ |
| 2. Naming | ✅ |
| 3. Nullability | ✅ |
| 4. Type inference | ✅ |
| 5. Modern C# | ✅ |
| 6. Async/Await | ⚠️ see findings |
| 7. Architecture | ✅ |
| 8. Response handling | ✅ |
| 9. Security | ✅ |
| 10. Performance | ⚠️ see findings |
| 11. File organization | ✅ |
| 12. XML documentation | ⚠️ see findings |

---

### Findings

#### 🔴 CRITICAL — Must fix before merge

- **[L42]** `.Result` on `GetStringAsync` blocks the thread synchronously. **Impact:** Deadlock in ASP.NET context where a synchronization context is present; also prevents cancellation from working. **Fix:** `string json = await _httpClient.GetStringAsync(url, ct).ConfigureAwait(false);`

#### 🟠 MAJOR — Strong recommendation to fix

- **[L55]** `ct` is accepted as a parameter but not forwarded to `_httpClient.GetAsync(url)`. **Impact:** Cancellation signals are silently ignored — endpoint calls cannot be cancelled. **Fix:** Pass `ct` to all inner `await` calls: `await _httpClient.GetAsync(url, ct).ConfigureAwait(false);`
- **[L18]** `GetMyListAsync`, `CreateTorrentAsync`, and `ControlTorrentAsync` have no XML documentation. **Impact:** Public API surface is undiscoverable — consumers cannot understand parameters or thrown exceptions. **Fix:** Add `<summary>`, `<param>`, `<returns>`, and `<exception>` on all three methods.

#### 🔵 NITPICK — Polish

- **[L29]** `var items` could be `IReadOnlyList<Torrent> items` for explicit type visibility in a teaching context.

---

### Verdict

> **`BLOCKED`**  
> Synchronous task blocking on L42 is a CRITICAL deadlock risk that must be resolved before this file is merged.
