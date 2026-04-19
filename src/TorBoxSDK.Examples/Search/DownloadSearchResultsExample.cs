using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.Search;

namespace TorBoxSDK.Examples.Search;

/// <summary>
/// Demonstrates how to download search results and retrieve details by ID:
/// Usenet NZB downloads, Usenet result by ID, and meta result by ID.
/// </summary>
public static class DownloadSearchResultsExample
{
	public static async Task RunAsync()
	{
		ExampleHelper.PrintHeader("Search — Download Results & Get by ID");

		ITorBoxClient client = ExampleHelper.CreateClient();
		using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

		try
		{
			// ──────────────────────────────────────────────────────
			// Search Usenet and get a result by ID.
			// ──────────────────────────────────────────────────────
			string query = "ubuntu"; // Replace with your search query

			Console.WriteLine($"Searching Usenet for: \"{query}\"...");

			TorBoxResponse<UsenetSearchResponse> searchResponse =
				await client.Search.SearchUsenetAsync(query, cancellationToken: cts.Token);

			if (searchResponse.Data is not null && searchResponse.Data.Nzbs.Count > 0)
			{
				UsenetSearchResult firstResult = searchResponse.Data.Nzbs[0];
				Console.WriteLine($"  Found {searchResponse.Data.Nzbs.Count} result(s)");
				Console.WriteLine($"  First: {firstResult.Name}");

				// Get full details for a specific Usenet result by ID
				if (firstResult.Id is { } resultId)
				{
					Console.WriteLine();
					Console.WriteLine($"Getting Usenet result details for ID: {resultId}...");

					TorBoxResponse<UsenetSearchResult> detailResponse =
						await client.Search.GetUsenetByIdAsync(resultId, cancellationToken: cts.Token);

					if (detailResponse.Data is not null)
					{
						Console.WriteLine($"  Name: {detailResponse.Data.Name}");
						Console.WriteLine($"  Size: {ExampleHelper.FormatBytes(detailResponse.Data.Size)}");
						Console.WriteLine($"  Source: {detailResponse.Data.Source ?? "N/A"}");
					}

					// ──────────────────────────────────────────────
					// Download the NZB file for a Usenet result.
					// Returns a URL to the NZB file.
					// ──────────────────────────────────────────────
					string guid = resultId; // The NZB GUID from the search result (often matches the result ID)

					Console.WriteLine();
					Console.WriteLine($"Downloading NZB for result: {resultId}...");

					TorBoxResponse<string> downloadResponse =
						await client.Search.DownloadUsenetAsync(resultId, guid, cts.Token);

					if (downloadResponse.Data is not null)
					{
						Console.WriteLine($"  NZB download URL/data length: {downloadResponse.Data.Length} chars");
					}
				}
			}
			else
			{
				Console.WriteLine("  No Usenet results found.");
			}

			// ──────────────────────────────────────────────────────
			// Search meta (movies/TV) and get a result by ID.
			// ──────────────────────────────────────────────────────
			string metaQuery = "Inception"; // Replace with your search query

			Console.WriteLine();
			Console.WriteLine($"Searching meta for: \"{metaQuery}\"...");

			TorBoxResponse<IReadOnlyList<MetaSearchResult>> metaResponse =
				await client.Search.SearchMetaAsync(metaQuery, cancellationToken: cts.Token);

			if (metaResponse.Data is not null && metaResponse.Data.Count > 0)
			{
				MetaSearchResult firstMeta = metaResponse.Data[0];
				Console.WriteLine($"  Found {metaResponse.Data.Count} result(s)");
				Console.WriteLine($"  First: {firstMeta.Title} ({firstMeta.ReleaseYears?.ToString() ?? "N/A"})");

				// Get full details for a specific meta result by ID
				if (firstMeta.Id is { } metaId)
				{
					Console.WriteLine();
					Console.WriteLine($"Getting meta result details for ID: {metaId}...");

					TorBoxResponse<MetaSearchResult> metaDetailResponse =
						await client.Search.GetMetaByIdAsync(metaId, cts.Token);

					if (metaDetailResponse.Data is not null)
					{
						Console.WriteLine($"  Title: {metaDetailResponse.Data.Title}");
						Console.WriteLine($"  Type: {metaDetailResponse.Data.MediaType ?? "N/A"}");
						Console.WriteLine($"  Year: {metaDetailResponse.Data.ReleaseYears?.ToString() ?? "N/A"}");
						Console.WriteLine($"  IMDB: {metaDetailResponse.Data.ImdbId ?? "N/A"}");

						if (metaDetailResponse.Data.Description is { } description)
						{
							Console.WriteLine($"  Description: {description[..Math.Min(100, description.Length)]}...");
						}
					}
				}
			}
			else
			{
				Console.WriteLine("  No meta results found.");
			}
		}
		catch (TorBoxException ex)
		{
			Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
		}

		Console.WriteLine();
		Console.WriteLine("Download search results example completed.");
	}
}
