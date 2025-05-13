using LoyaltyAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyAPI.Services
{
    public class NfcTransactionService
    {
        private readonly NFCContext _nfcContext;
        private readonly LoyaltyDbContext _loyaltyContext;
        private readonly ILogger<NfcTransactionService> _logger;

        public NfcTransactionService(NFCContext nfcContext, LoyaltyDbContext loyaltyContext, ILogger<NfcTransactionService> logger)
        {
            _nfcContext = nfcContext;
            _loyaltyContext = loyaltyContext;
            _logger = logger;
        }
        public async Task<(bool Success, string? Reason)> InsertReferralFromNfcAsync(string brId, string clientId, string clientChkId, string? cardRefNo = null)

        {
            string formattedBrId = brId.PadLeft(2, '0');
            string formattedClientId = clientId.PadLeft(7, '0');
            string formattedClientChkId = clientChkId.PadLeft(1, '0');
            string memberUid = $"{formattedBrId}-{formattedClientId}-{formattedClientChkId}";

            var cards = await _nfcContext.OfficialCards
                .Where(c => c.MemberUID.StartsWith($"{formattedBrId}-{formattedClientId}"))
                .ToListAsync();

            if (!cards.Any())
            {
                return (false, "CardNotFound");
            }

            var selectedCard = !string.IsNullOrEmpty(cardRefNo)
                ? cards.FirstOrDefault(c => c.CardRefNo?.Trim() == cardRefNo.Trim())
                : cards.FirstOrDefault();

            if (selectedCard == null)
            {
                return (false, "CardNotFound");
            }

            if (!int.TryParse(clientId.TrimStart('0'), out int parsedClientId))
            {
                return (false, "Error");
            }

            var existingReferral = await _loyaltyContext.Referral
                .FirstOrDefaultAsync(r =>
                    r.ClientId == parsedClientId &&
                    r.ReferralCode != null &&
                    r.ReferralCode.Trim() == (selectedCard.CardRefNo != null ? selectedCard.CardRefNo.Trim() : string.Empty));

            if (existingReferral != null)
            {
                return (false, "Duplicate");
            }

            var newReferral = new Referral
            {
                ClientId = parsedClientId,
                ReferralCode = selectedCard.CardRefNo ?? "N/A",
                BrId = int.Parse(brId),
                Points = 0,
                CreatedAt = selectedCard.DateAdded,
                UpdatedAt = DateTime.UtcNow
            };

            _loyaltyContext.Referral.Add(newReferral);

            try
            {
                await _loyaltyContext.SaveChangesAsync();
                return (true, null);
            }
            catch
            {
                return (false, "Error");
            }
        }

    }
}
