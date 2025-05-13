using LoyaltyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyAPI.Services
{
    public class ReferralService
    {
        private readonly LoyaltyDbContext _context;

        public ReferralService(LoyaltyDbContext context)
        {
            _context = context;
        }

        public async Task<Referral?> CreateReferralAsync(int clientId, int brId)
        {
            var existingReferral = await _context.Referral
                .FirstOrDefaultAsync(r => r.ClientId == clientId);

            if (existingReferral != null)
            {
                return existingReferral;
            }


            var clientIdStr = clientId.ToString("D6");
            var referralCode = $"REF{clientIdStr[^3..]}{brId:D2}";


            var referral = new Referral
            {
                ClientId = clientId,
                BrId = brId,
                ReferralCode = referralCode,
                Points = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Referral.AddAsync(referral);
            await _context.SaveChangesAsync();

            return referral;
        }


        public async Task<Referral?> GetReferralAsync(int clientId)
        {
            return await _context.Referral.FirstOrDefaultAsync(r => r.ClientId == clientId);
        }
    }
}
