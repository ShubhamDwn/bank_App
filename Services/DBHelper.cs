using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace bank_demo.Services
{
    public static class DBHelper
    {
        private const string connectionString = "server=10.0.2.2;port=3306;user=root;password=root;database=bankdb;";

        public static async Task<MySqlConnection> GetConnectionAsync()
        {
            var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }
    }

}
