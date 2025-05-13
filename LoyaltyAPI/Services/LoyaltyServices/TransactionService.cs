using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LoyaltyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TransactionService
{
    private readonly WebLoanAppDbContext _webLoanAppDbContext;
    private readonly COBXDbContext _cobxDbContext;
    private readonly LoyaltyDbContext _loyaltyDbContext;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(WebLoanAppDbContext webLoanAppDbContext,
                             COBXDbContext cobxDbContext,
                             LoyaltyDbContext loyaltyDbContext,
                             ILogger<TransactionService> logger)
    {
        _webLoanAppDbContext = webLoanAppDbContext;
        _cobxDbContext = cobxDbContext;
        _loyaltyDbContext = loyaltyDbContext;
        _logger = logger;
    }

    public async Task IntegrateDataForClientAsync(int clientId)
    {
        try
        {
            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);

            var cobxTransactions = await _cobxDbContext.tblTransactionDetails
                .Where(t => t.ClientID == clientId)
                .ToListAsync();

            _logger.LogInformation($"[Client {clientId}] Total COBX transactions found: {cobxTransactions.Count}");

            if (!cobxTransactions.Any())
            {
                _logger.LogInformation($"[Client {clientId}] No transactions found in COBX.");
                return;
            }

            int countMissingActivity = 0;
            int countInserted = 0;
            int countDuplicateTransDetailID = 0;

            HashSet<string> missingActivityCodes = new HashSet<string>();
            HashSet<long> processedTransDetailIDs = new HashSet<long>();

            const int batchSize = 100;
            for (int i = 0; i < cobxTransactions.Count; i += batchSize)
            {
                var batch = cobxTransactions.Skip(i).Take(batchSize).ToList();

                await _loyaltyDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await _loyaltyDbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var batchLoyaltyTransactions = new List<Transaction>();

                            foreach (var cobxTransaction in batch)
                            {
                                if (!cobxTransaction.ClientID.HasValue)
                                {
                                    _logger.LogWarning($"[Client {clientId}] Skipped transaction (null ClientID): COBX TransactionCode: {cobxTransaction.TransactionCode}");
                                    continue;
                                }

                                int loyaltyClientId = (int)cobxTransaction.ClientID.Value;

                                var activity = await _loyaltyDbContext.Activity
                                .Where(a => (a.transaction_type ?? "") == cobxTransaction.TransactionCode.ToString())
                                .Select(a => new { a.activity_id, a.description, a.points })
                                .FirstOrDefaultAsync();

                                if (activity != null)
                                {
                                    if (processedTransDetailIDs.Contains(cobxTransaction.TransDetailID))
                                    {
                                        _logger.LogWarning($"[Client {clientId}] Duplicate TransDetailID skipped: {cobxTransaction.TransDetailID}");
                                        countDuplicateTransDetailID++;
                                        continue;
                                    }

                                    processedTransDetailIDs.Add(cobxTransaction.TransDetailID);

                                    string formattedId = string.Format("{0:D2}-{1:D16}",
                                        cobxTransaction.BR_ID, cobxTransaction.TransDetailID);


                                    bool exists = await _loyaltyDbContext.Transaction
                                        .AnyAsync(t => t.ReferenceId == formattedId);

                                    if (exists)
                                    {
                                        _logger.LogWarning($"[Client {clientId}] Skipped duplicate ReferenceId: {formattedId}");
                                        countDuplicateTransDetailID++;
                                        continue;
                                    }

                                    var loyaltyTransaction = new Transaction
                                    {
                                        ClientId = loyaltyClientId,
                                        Description = activity.description,
                                        Points = activity.points,
                                        CreatedAt = cobxTransaction.SLDate,
                                        DateTime = now,
                                        ReferenceId = formattedId,
                                        ReferralId = 0,
                                        ActivityId = activity.activity_id,
                                        ClientName = cobxTransaction.ClientName ?? "Unknown",
                                        BrId = (int)cobxTransaction.BR_ID,


                                    };

                                    batchLoyaltyTransactions.Add(loyaltyTransaction);
                                }
                                else
                                {
                                    _logger.LogWarning($"[Client {clientId}] Missing activity mapping for COBX TransactionCode: {cobxTransaction.TransactionCode}");
                                    countMissingActivity++;
                                    missingActivityCodes.Add(cobxTransaction.TransactionCode.ToString());
                                }
                            }

                            if (batchLoyaltyTransactions.Any())
                            {
                                await _loyaltyDbContext.Transaction.AddRangeAsync(batchLoyaltyTransactions);
                                await _loyaltyDbContext.SaveChangesAsync();
                                countInserted += batchLoyaltyTransactions.Count;
                            }

                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, $"[Client {clientId}] Error occurred during batch insert.");
                            throw;
                        }
                    }
                });
            }

            _logger.LogInformation($"[Client {clientId}] ✅ Insertion Summary:");
            _logger.LogInformation($"- Total COBX transactions fetched: {cobxTransactions.Count}");
            _logger.LogInformation($"- Inserted into Loyalty: {countInserted}");
            _logger.LogInformation($"- Skipped (missing Activity mapping): {countMissingActivity}");
            _logger.LogInformation($"- Skipped (duplicate TransDetailID or ReferenceId): {countDuplicateTransDetailID}");

            if (missingActivityCodes.Any())
            {
                _logger.LogInformation($"[Client {clientId}] 🚫 Missing TransactionCode mappings in Activity table:");
                foreach (var code in missingActivityCodes)
                {
                    _logger.LogInformation($"- TransactionCode: {code}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error occurred while integrating data for client ID {clientId}");
        }
    }

    public async Task<List<Transaction>> GetTransactionsByClientIdAsync(int clientId)
    {
        try
        {
            var transactions = await _loyaltyDbContext.Transaction
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return transactions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching transactions for Client ID {clientId}.", clientId);
            throw;
        }
    }



}
