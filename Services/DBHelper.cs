using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

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
    }
}
