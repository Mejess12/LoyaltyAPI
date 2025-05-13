using LoyaltyAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LoyaltyAPI.Controllers
{
    [Route("api/v1/nfctransactions")]
    [ApiController]
    public class NfcTransactionController : ControllerBase
    {
        private readonly NfcTransactionService _nfcTransactionService;

        public NfcTransactionController(NfcTransactionService nfcTransactionService)
        {
            _nfcTransactionService = nfcTransactionService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertTransactionFromNfc(
        [FromQuery] string brId,
        [FromQuery] string clientId,
        [FromQuery] string clientChkId)
        {
            if (string.IsNullOrEmpty(brId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientChkId))
            {
                return BadRequest("Invalid input. Please provide brId, clientId, and clientChkId.");
            }

            var (success, reason) = await _nfcTransactionService.InsertReferralFromNfcAsync(brId, clientId, clientChkId);

            if (success)
            {
                return Ok("Referral successfully inserted.");
            }

            if (reason == "Duplicate")
            {
                return Ok("Referral already exists for this NFC card.");
            }

            if (reason == "CardNotFound")
            {
                return Ok(); // Skip silently if no NFC
            }

            // For any other unexpected errors
            return StatusCode(500, "An error occurred while processing the referral.");
        }



    }
}
