using TorBoxSDK.Examples.Helpers;
using TorBoxSDK.Models.Common;
using TorBoxSDK.Models.User;

namespace TorBoxSDK.Examples.Main.User;

/// <summary>
/// Demonstrates how to retrieve transaction history and download transaction PDFs.
/// </summary>
public static class TransactionsExample
{
    public static async Task RunAsync()
    {
        ExampleHelper.PrintHeader("User — Transactions & PDF Export");

        ITorBoxClient client = ExampleHelper.CreateClient();
        using CancellationTokenSource cts = ExampleHelper.CreateTimeout();

        try
        {
            // ──────────────────────────────────────────────────────
            // Get transaction history.
            // ──────────────────────────────────────────────────────
            Console.WriteLine("Fetching transaction history...");

            TorBoxResponse<IReadOnlyList<Transaction>> transactionsResponse =
                await client.Main.User.GetTransactionsAsync(cts.Token);

            if (transactionsResponse.Data is not null && transactionsResponse.Data.Count > 0)
            {
                Console.WriteLine($"Found {transactionsResponse.Data.Count} transaction(s):");

                foreach (Transaction transaction in transactionsResponse.Data)
                {
                    Console.WriteLine($"  [{transaction.TransactionId ?? "N/A"}] {transaction.Type ?? "N/A"} — {transaction.Amount} ({transaction.At?.ToString("o") ?? "N/A"})");
                }

                // ──────────────────────────────────────────────────
                // Download a specific transaction as PDF.
                // ──────────────────────────────────────────────────
                string? transactionId = transactionsResponse.Data[0].TransactionId;

                if (transactionId is not null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Downloading PDF for transaction {transactionId}...");

                    TorBoxResponse<string> pdfResponse =
                        await client.Main.User.GetTransactionPdfAsync(transactionId, cts.Token);

                    if (pdfResponse.Data is not null)
                    {
                        Console.WriteLine($"  PDF URL/data length: {pdfResponse.Data.Length} chars");
                        Console.WriteLine("  Use this URL to download or display the transaction receipt.");
                    }
                }
            }
            else
            {
                Console.WriteLine("  No transactions found.");

                // Demo with a sample transaction ID
                string sampleTransactionId = "12345"; // Replace with actual transaction ID

                Console.WriteLine();
                Console.WriteLine($"Downloading PDF for transaction {sampleTransactionId}...");

                TorBoxResponse<string> pdfResponse =
                    await client.Main.User.GetTransactionPdfAsync(sampleTransactionId, cts.Token);

                if (pdfResponse.Data is not null)
                {
                    Console.WriteLine($"  PDF URL/data length: {pdfResponse.Data.Length} chars");
                }
            }
        }
        catch (TorBoxException ex)
        {
            Console.Error.WriteLine($"API error [{ex.ErrorCode}]: {ex.Detail ?? ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Transactions example completed.");
    }
}
