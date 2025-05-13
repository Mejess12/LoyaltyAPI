using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoyaltyAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LoyaltyAPI.Controllers
{
    [ApiController]
    [Route("api/v1/tier")]
    public class TierController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;
        private readonly ILogger<TierController> _logger;

        public TierController(LoyaltyDbContext context, ILogger<TierController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{clientId}")]
        public async Task<ActionResult<object>> GetClientTier(int clientId, DateTime? currentDate = null)
        {
            try
            {
                _logger.LogInformation($"GetClientTier called for clientId: {clientId}, currentDate: {currentDate}");


                long totalPoints = await _context.Transaction
                    .Where(t => t.ClientId == clientId)
                    .SumAsync(t => (long)(t.Points ?? 0));

                _logger.LogInformation($"Client {clientId} has {totalPoints} points.");


                var tiers = await _context.Tier.ToListAsync();

                Tier? clientTier = null;
                foreach (var tier in tiers)
                {
                    _logger.LogInformation($"Checking tier {tier.tier_id}: min={tier.min}, max={tier.max}");


                    if (tier.min <= totalPoints && totalPoints < tier.max)
                    {
                        clientTier = tier;
                        break;
                    }
                }

                // If no tier was found and points exceed 1000, assign the "Champion" tier (id = 4)
                if (clientTier == null && totalPoints > 1000)
                {
                    clientTier = tiers.FirstOrDefault(t => t.tier_id == 4);
                }


                if (clientTier == null)
                {
                    _logger.LogInformation($"No tier found for client {clientId}.");
                    return NotFound(new { Status = 0, Message = "Tier not found for the given points." });
                }

                _logger.LogInformation($"Tier found for client {clientId}: {clientTier.tier_id}");

                return Ok(new
                {
                    Status = 1,
                    Tier = new
                    {
                        Badge = clientTier.badge,
                        Description = clientTier.description,
                        MinPoints = clientTier.min,
                        MaxPoints = clientTier.max,
                        Color = clientTier.color
                    },
                    Points = totalPoints,
                    Message = "Client tier retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetClientTier for clientId: {clientId}");
                return StatusCode(500, new { Status = 0, Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<object>> GetAllTiers()
        {
            var tiers = await _context.Tier.ToListAsync();
            return Ok(new
            {
                Status = 1,
                Tiers = tiers.Select(t => new
                {
                    Badge = t.badge,
                    MinPoints = t.min,
                    MaxPoints = t.max,
                    Description = t.description,
                    DateAdded = t.date_added,
                    UpdateDate = t.update_date,
                    Color = t.color
                }),
                Message = "All tiers retrieved successfully."
            });
        }
    }
}
