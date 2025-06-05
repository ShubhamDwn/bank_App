using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace bank_demo.Services
{
    public static class DBHelper
    {
        // Local SQL Server connection string
        private const string connectionString = "Server=192.168.251.99,1433;Database=bankdb;User=appuser;Password=root;TrustServerCertificate=True;";

        // This method gets the connection asynchronously
        public static async Task<SqlConnection> GetConnectionAsync()
        {
            var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(); // Open the connection asynchronously
            return conn;
        }

        // Method to execute a query
        public static async Task ExecuteQueryAsync(string query)
        {
            using (var conn = await GetConnectionAsync())
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    await cmd.ExecuteNonQueryAsync(); // Execute the query asynchronously
                }
            }
        }
    }
}
