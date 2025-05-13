using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/Transactions")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _dataIntegrationService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(TransactionService dataIntegrationService, ILogger<TransactionController> logger)
    {
        _dataIntegrationService = dataIntegrationService;
        _logger = logger;
    }

    [HttpPost("sync/{clientId}")]
    public async Task<IActionResult> IntegrateData(int clientId)
    {
        try
        {
            await _dataIntegrationService.IntegrateDataForClientAsync(clientId);
            return Ok(new { message = "Data integration completed successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data integration.");
            return StatusCode(500, new { error = "An error occurred during data integration.", details = ex.Message });
        }
    }


    [HttpGet("transactions/{clientId}")]
    public async Task<IActionResult> GetTransactionsByClientId(int clientId)
    {
        try
        {
            var transactions = await _dataIntegrationService
                .GetTransactionsByClientIdAsync(clientId);

            if (!transactions.Any())
            {
                return NotFound(new { message = $"No transactions found for Client ID {clientId}" });
            }

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching transactions for Client ID {clientId}.", clientId);
            return StatusCode(500, new { error = "An error occurred while fetching transactions.", details = ex.Message });
        }
    }



}