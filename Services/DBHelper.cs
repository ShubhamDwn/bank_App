using bank_demo.Services; // Adjust namespace for your models
using bank_demo.Services.API; // Adjust namespace for your API models
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

        public static async Task<List<AccountModel>> GetCustomerAccountsAsync(int customerId, string accountType, string deviceId = "083ea3911295b82d", int closed = 0)
        {
            // Append optional query parameters
            string url = $"{baseUrl}/accounts?customerId={customerId}&accountType={Uri.EscapeDataString(accountType)}&deviceId={Uri.EscapeDataString(deviceId)}&closed={closed}";
            return await client.GetFromJsonAsync<List<AccountModel>>(url);
        }

        public static async Task<List<TransactionModel>> GetTransactionsAsync(
            int customerId,
            int subSchemeId,
            int accountNumber,
            int pigmyAgentId,
            DateTime fromDate,
            DateTime toDate,
            string deviceId = "083ea3911295b82d")
            {

            // Format for API (safe format)
                string fromDateApi = fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                string toDateApi = toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                string url = $"{baseUrl}/transactions" +
                             $"?customerId={customerId}" +
                             $"&subSchemeId={subSchemeId}" +
                             $"&accountNumber={accountNumber}" +
                             $"&pigmyAgentId={pigmyAgentId}" +
                             $"&deviceId={Uri.EscapeDataString(deviceId)}" +
                             $"&fromDate={fromDateApi}" +
                             $"&toDate={toDateApi}";

                try
                {
                    var result = await client.GetFromJsonAsync<List<TransactionModel>>(url);
                    return result ?? new List<TransactionModel>();
                }
                catch (Exception ex)
                {
                    // Optional: log error or show message
                    Console.WriteLine($"Error fetching transactions: {ex.Message}");
                    return new List<TransactionModel>();
                }
           }

    }
}
