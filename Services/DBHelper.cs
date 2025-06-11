using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using bank_demo.Services; // Adjust namespace for your models
using bank_demo.Services.API; // Adjust namespace for your API models

namespace bank_demo.Services
{
    public static class DBHelper
    {
        private static readonly HttpClient client = new();
        private static readonly string baseUrl = $"{BaseURL.Url()}api/statement";

        public static async Task<List<string>> GetAccountTypesAsync(int customerId)
        {
            return await client.GetFromJsonAsync<List<string>>($"{baseUrl}/account-types/{customerId}");
        }

        public static async Task<List<AccountModel>> GetCustomerAccountsAsync(int customerId, string accountType, string deviceId = "d7620a1b553407a7", int closed = 0)
        {
            // Append optional query parameters
            string url = $"{baseUrl}/accounts?customerId={customerId}&accountType={Uri.EscapeDataString(accountType)}&deviceId={Uri.EscapeDataString(deviceId)}&closed={closed}";
            return await client.GetFromJsonAsync<List<AccountModel>>(url);
        }

        public static async Task<List<TransactionModel>> GetTransactionsAsync(int customerId, string accountType, DateTime fromDate, DateTime toDate)
        {
            string url = $"{baseUrl}/transactions?customerId={customerId}&accountType={Uri.EscapeDataString(accountType)}&fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            return await client.GetFromJsonAsync<List<TransactionModel>>(url);
        }
    }
}
