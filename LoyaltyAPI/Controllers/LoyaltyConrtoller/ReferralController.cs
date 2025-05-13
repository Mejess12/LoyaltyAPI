using LoyaltyAPI.Models;
using LoyaltyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyAPI.Controllers
{
    [Route("api/v1/referral")]
    [ApiController]
    public class ReferralController : ControllerBase
    {
        private readonly ReferralService _referralService;

        public ReferralController(ReferralService referralService)
        {
            _referralService = referralService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReferral([FromBody] ReferralRequest request)
        {
            Console.WriteLine($"Received Referral Create Request: ClientId={request.ClientId}, BrId={request.BrId}");

            if (request == null || request.ClientId <= 0 || request.BrId <= 0)
            {
                return BadRequest("Invalid clientId or brId.");
            }

            var referral = await _referralService.CreateReferralAsync(request.ClientId, request.BrId);

            if (referral != null && referral.ReferralId > 0)
            {

                return Ok(referral);
            }

            return Conflict("Referral already exists.");
        }


        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetReferral(int clientId)
        {
            if (clientId <= 0)
            {
                return BadRequest("Invalid clientId.");
            }

            var referral = await _referralService.GetReferralAsync(clientId);

            if (referral == null)
            {
                return NotFound("Referral not found.");
            }

            return Ok(referral);
        }
    }
}
