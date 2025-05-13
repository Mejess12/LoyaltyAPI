using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LoyaltyAPI.Models;
using MySqlConnector;

public class DataService
{
    private readonly string _webLoanAppConnection;
    private readonly string _cobxDavaoConnection;

    public DataService(string webLoanAppConnection, string cobxDavaoConnection)
    {
        _webLoanAppConnection = webLoanAppConnection;
        _cobxDavaoConnection = cobxDavaoConnection;
    }

    public async Task<ClientWallet> GetClientWalletAsync(string clientId)
    {
        using var connection = new MySqlConnection(_webLoanAppConnection);
#pragma warning disable CS8603 
        return await connection.QueryFirstOrDefaultAsync<ClientWallet>(
            "SELECT * FROM clientwallet WHERE ClientID = @ClientID",
            new { ClientID = clientId }
        );
#pragma warning restore CS8603 
    }

    [Obsolete]
    public async Task<List<TransactionDetails>> GetTransactionsAsync()
    {
        using var connection = new SqlConnection(_cobxDavaoConnection);
        return (await connection.QueryAsync<TransactionDetails>(
            "SELECT * FROM tblTransactionDetails"
        )).ToList();
    }
}