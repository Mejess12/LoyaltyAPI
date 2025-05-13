using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Common;
using LoyaltyAPI.Models;

namespace LoyaltyAPI.Controllers.LoyaltyController
{
    [ApiController]
    [Route("api/v1/Leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public LeaderboardController(LoyaltyDbContext context)
        {
            _context = context;
        }

        [HttpGet("TopEarners")]
        public async Task<IActionResult> GetTopEarners()
        {
            try
            {
                var results = await _context.Transaction
                    .Where(t => t.ClientId != null && t.Points != null)
                    .GroupBy(t => t.ClientId)
                    .Select(g => new
                    {
                        ClientId = g.Key,
                        ClientName = g.Max(t =>
                            t.ClientName != null &&
                            t.ClientName.ToLower() != "unknown" &&
                            !string.IsNullOrEmpty(t.ClientName)
                            ? t.ClientName
                            : null),
                        Points = g.Sum(t => t.Points),
                        Badge = _context.Tier
                            .Where(tr => g.Sum(t => t.Points) >= tr.min && g.Sum(t => t.Points) <= tr.max)
                            .Select(tr => tr.badge)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(x => x.Points)
                    .ToListAsync();

                if (results == null || !results.Any())
                {
                    return Ok(new { success = false, message = "No current users found.", topEarners = new List<object>() });
                }

                return Ok(new { success = true, topEarners = results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}