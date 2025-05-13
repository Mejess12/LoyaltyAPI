using LoyaltyAPI.Models;
using LoyaltyAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoyaltyAPI.Controllers;

[ApiController]
[Route("api/v1/activity")]
public class ActivityController : ControllerBase
{
    private readonly ActivityService _activityService;
    private readonly COBXService _cobxService;

    public ActivityController(ActivityService activityService, COBXService cobxService)
    {
        _activityService = activityService;
        _cobxService = cobxService;
    }

    // Get data from tblTransactionTypes
    [HttpGet("types")]
    public async Task<ActionResult<List<tblTransactionType>>> GetTransactionTypes()
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

    [HttpPost("sync-from-cobx")]
    public async Task<ActionResult<bool>> TransferFromCOBX()
    {
        try
        {
            var transactionTypes = await _cobxService.GetTransactionTypesAsync();

            foreach (var transactionType in transactionTypes)
            {
                var activity = new Activity
                {
                    description = transactionType.TransTypeDesc,
                    points = 0,
                    date_time = DateTime.Now,
                    system_source_id = 1,
                    created_at = DateTime.Now,
                    transaction_type = transactionType.TransTypeID.ToString()
                };

                await _activityService.InsertActivityAsync(activity);
            }

            return Ok(true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error transferring data.", details = ex.Message });
        }
    }
}