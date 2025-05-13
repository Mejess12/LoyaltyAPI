using LoyaltyAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

public class COBXService
{
    private readonly COBXDbContext _context;
    private readonly ILogger<COBXService> _logger;

    public COBXService(COBXDbContext context, ILogger<COBXService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<tblTransactionDetails>> GetTransactionDetailsAsync()
    {
        if (_context.tblTransactionDetails == null)
        {
            _logger.LogError("DbSet tblTransactionDetails is null.");
            throw new InvalidOperationException("DbSet tblTransactionDetails is null.");
        }

        try
        {
            return await _context.tblTransactionDetails.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction details");
            throw;
        }
    }

    public async Task<List<tblTransactionType>> GetTransactionTypesAsync()
    {
        if (_context.tblTransactionTypes == null)
        {
            _logger.LogError("DbSet tblTransactionTypes is null.");
            throw new InvalidOperationException("DbSet tblTransactionTypes is null.");
        }

        try
        {
            return await _context.tblTransactionTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction types");
            throw;
        }
    }
}