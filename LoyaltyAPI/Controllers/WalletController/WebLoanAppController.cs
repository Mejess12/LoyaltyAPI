using Microsoft.AspNetCore.Mvc;
using LoyaltyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[Route("api/v1/webloanapp")]
[ApiController]
public class WebLoanAppController : ControllerBase
{
    private readonly WebLoanAppService _webLoanAppService;
    private readonly ILogger<WebLoanAppController> _logger;

    public WebLoanAppController(WebLoanAppService webLoanAppService, ILogger<WebLoanAppController> logger)
    {
        _webLoanAppService = webLoanAppService;
        _logger = logger;
    }

    [HttpGet("clientwallets")]
    public async Task<ActionResult<List<ClientWallet>>> GetClientWallets()
    {
        try
        {
            return await _webLoanAppService.GetClientWalletsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching client wallets.");
            return StatusCode(500, new { error = "An error occurred.", details = ex.Message });
        }
    }

    [HttpGet("clientwallet/{clientId}")]
    public async Task<ActionResult<ClientWallet>> GetClientWalletById(int clientId)
    {
        try
        {
            if (clientId <= 0)
            {
                _logger.LogWarning($"Invalid client ID: {clientId}");
                return BadRequest(new { error = "Invalid client ID." });
            }
            var wallet = await _webLoanAppService.GetClientWalletByIdAsync(clientId);
            if (wallet == null)
            {
                _logger.LogWarning($"Client wallet not found for client ID: {clientId}");
                return NotFound();
            }
            return wallet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching client wallet for client ID: {clientId}");
            return StatusCode(500, new { error = "An error occurred.", details = ex.Message });
        }
    }
}