using Microsoft.AspNetCore.Mvc;
using LoyaltyAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/v1/COBX")]
public class COBXController : ControllerBase
{
    private readonly COBXService _cobxService;
    private readonly DbContextOptions<COBXDbContext> _dbContextOptions;
    private readonly ILogger<COBXController> _logger;

    public COBXController(COBXService cobxService, DbContextOptions<COBXDbContext> dbContextOptions, ILogger<COBXController> logger) // Add logger parameter
    {
        _cobxService = cobxService;
        _dbContextOptions = dbContextOptions;
        _logger = logger;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionDetails()
    {
        try
        {
            var transactions = await _cobxService.GetTransactionDetailsAsync();
            return Ok(transactions);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while fetching transactions.");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching transactions.");
            return StatusCode(500, new { error = "An error occurred while fetching transactions.", details = ex.Message });
        }
    }

    [HttpGet("transactiontypes")]
    public async Task<IActionResult> GetTransactionTypes()
    {
        try
        {
            var types = await _cobxService.GetTransactionTypesAsync();
            return Ok(types);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while fetching transaction types.", details = ex.Message });
        }
    }

    // [HttpGet("testTransactions")]
    // public async Task<IActionResult> TestTransactions()
    // {
    //     try
    //     {
    //         using (var context = new COBXDbContext(_dbContextOptions)) // Use stored options
    //         {
    //             var transactions = await context.tblTransactionDetails.ToListAsync();
    //             return Ok(transactions);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { error = "Test endpoint error", details = ex.Message });
    //     }
    // }
}