using Dapper;
using MySql.Data.MySqlClient; // Keep this line
using System.Collections.Generic;
using System.Threading.Tasks;
using LoyaltyAPI.Models;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI;

public class WebLoanAppService
{
    private readonly string _connectionString;

    public WebLoanAppService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("WebLoanAppDB")
            ?? throw new InvalidOperationException("Connection string 'WebLoanAppDB' not found.");
    }

    // Fetch all client wallets
    public async Task<List<ClientWallet>> GetClientWalletsAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        var wallets = await connection.QueryAsync<ClientWallet>("SELECT * FROM tblclientwallet");
        return wallets.AsList();
    }

    // Fetch a client wallet by client ID
    public async Task<ClientWallet?> GetClientWalletByIdAsync(int clientId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<ClientWallet>(
            "SELECT * FROM tblclientwallet WHERE ClientID = @ClientID",
            new { ClientID = clientId }
        );
    }

    // Fetch a client wallet by username
    public async Task<ClientWallet?> GetClientWalletByUsernameAsync(string username)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<ClientWallet>(
            "SELECT * FROM tblclientwallet WHERE Username = @Username",
            new { Username = username }
        );
    }

    // Fetch all clients
    public async Task<List<Client>> GetAllClientsAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        var clients = await connection.QueryAsync<Client>("SELECT * FROM tblclient");
        return clients.AsList();
    }

    // Fetch a client by ID
    public async Task<Client?> GetClientByIdAsync(int clientId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Client>(
            "SELECT * FROM tblclient WHERE ClientID = @ClientID",
            new { ClientID = clientId }
        );
    }

    // Fetch transaction details for a client (based on TransactionDetails model)
    public async Task<List<TransactionDetails>> GetTransactionDetailsAsync(long clientId)
    {
        using var connection = new MySqlConnection(_connectionString);
        var transactionDetails = await connection.QueryAsync<TransactionDetails>(
            "SELECT * FROM tblTransactionDetails WHERE ClientID = @ClientID",
            new { ClientID = clientId }
        );
        return transactionDetails.AsList();
    }
}
