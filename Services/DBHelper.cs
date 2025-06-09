using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using bank_demo.Services;

namespace bank_demo.Services
{
    public static class DBHelper
    {
        private const string connectionString = "Server=192.168.1.14,1433;Database=WarnaMahilaDB;User=sa;Password=sa;TrustServerCertificate=True;";

        public static async Task<SqlConnection> GetConnectionAsync()
        {
            var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }

        public static async Task ExecuteQueryAsync(string query)
        {
            using var conn = await GetConnectionAsync();
            using var cmd = new SqlCommand(query, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        // New: Get account types by calling your stored procedure
        public static async Task<List<string>> GetAccountTypesAsync(int customerId)
        {
            var results = new List<string>();

            using (var conn = await GetConnectionAsync())
            using (var cmd = new SqlCommand("App_AccountList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 200;
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        results.Add(reader.GetString(0));
                }
            }

            return results;
        }

        // New: Get transactions list for an account type and date range
        public static async Task<List<TransactionModel>> GetTransactionsAsync(int customerId, string accountType, DateTime fromDate, DateTime toDate)
        {
            var transactions = new List<TransactionModel>();

            using var conn = await GetConnectionAsync();

            // Assuming you have a stored procedure for transactions, e.g. "dbo.GetAccountStatement"
            using var cmd = new SqlCommand("dbo.App_GetNewStatement", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@CustomerId", customerId));
            cmd.Parameters.Add(new SqlParameter("@AccountType", accountType));
            cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
            cmd.Parameters.Add(new SqlParameter("@ToDate", toDate));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                transactions.Add(new TransactionModel
                {
                    Description = reader["Description"]?.ToString(),
                    Amount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0,
                    Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue
                });
            }

            return transactions;
        }

        public static async Task<List<string>> GetDistinctAccountTypesAsync(int customerId)
        {
            var list = new List<string>();

            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("App_GetCustomerAccount", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 200;
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(reader.GetString(0)); // assuming first column is AccountType (string)
            }

            return list;
        }

        public static async Task<List<AccountModel>> GetCustomerAccountsAsync(int customerId, string accountType, string deviceId = "d7620a1b553407a7", int closed = 0)
        {
            var accounts = new List<AccountModel>();

            // Handle null or whitespace accountType
            if (string.IsNullOrWhiteSpace(accountType))
                throw new ArgumentException("Account type cannot be null or empty.", nameof(accountType));

            // Mapping dictionary
            var accountTypeMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "SHARE", 1 },
        { "SAVING", 2 },
        { "FIXED", 3 },
        { "LOAN", 4 },
        { "RECCURING", 5 },
        { "PIGMYAGENT", 6 },
        { "PIGMY", 7 }
    };

            var normalizedKey = accountType.Trim().ToUpperInvariant();

            if (!accountTypeMap.TryGetValue(normalizedKey, out int accountTypeInt))
                throw new ArgumentException($"Invalid account type: '{accountType}'", nameof(accountType));

            using var conn = await GetConnectionAsync();
            using var cmd = new SqlCommand("App_GetCustomerAccount", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 200
            };

            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@AcType", accountTypeInt); // Converted to INT
            cmd.Parameters.AddWithValue("@DeviceId", deviceId);
            cmd.Parameters.AddWithValue("@Closed", closed);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var account = new AccountModel
                {
                    AccountNumber = reader["AccountNumber"]?.ToString(),
                    AccountName = reader["AccountName"]?.ToString(),
                    AccountType = reader["AccountType"]?.ToString(),
                    Balance = reader["Balance"] != DBNull.Value ? Convert.ToDecimal(reader["Balance"]) : 0,
                    OpeningDate = reader["OpeningDate"] != DBNull.Value ? Convert.ToDateTime(reader["OpeningDate"]) : DateTime.MinValue,
                    BranchName = reader["BranchName"]?.ToString(),
                    Status = reader["Status"]?.ToString()
                };
                accounts.Add(account);
            }

            return accounts;
        }


    }
}
