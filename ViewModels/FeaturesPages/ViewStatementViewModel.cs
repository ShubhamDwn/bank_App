using bank_demo.Services;
using bank_demo.Services.API;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class ViewStatementViewModel : BaseViewModel
    {
        public ObservableCollection<TransactionModel> Transactions { get; set; } = new();

        public ICommand LoadStatementCommand { get; }
        public ICommand ExportPdfCommand { get; }

        private int _customerId;
        private int _subSchemeId;
        private string _subSchemeName;
        private int _accountNumber;
        private int _pigmyAgentId;

        // ✅ These are the actual bindable properties used in UI
        private DateTime _from;
        public DateTime FromDate
        {
            get => _from;
            set => SetProperty(ref _from, value);
        }

        private DateTime _to;
        public DateTime ToDate
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isStatementVisible;
        public bool IsStatementVisible
        {
            get => _isStatementVisible;
            set => SetProperty(ref _isStatementVisible, value);
        }

        public bool IsCustomDateRange => true;
        public bool IsViewStatementVisible => true;

        // ✅ Constructor now assigns dates to bindable properties FromDate and ToDate
        public ViewStatementViewModel(int customerId, int subSchemeId, string subSchemeName, int accountNumber, int pigmyAgentId, DateTime fromDate, DateTime toDate)
        {
            _customerId = customerId;
            _subSchemeId = subSchemeId;
            _subSchemeName = subSchemeName;
            _accountNumber = accountNumber;
            _pigmyAgentId = pigmyAgentId;

            FromDate = fromDate;  // ✅ show correct value in DatePicker
            ToDate = toDate;

            LoadStatementCommand = new Command(async () => await LoadStatementAsync());
            ExportPdfCommand = new Command(ExportToPdf);

            // ✅ Load using these properties (you can test with log alert too)
            _ = LoadStatementAsync();
        }

        private async Task LoadStatementAsync()
        {
            try
            {
                IsLoading = true;
                Transactions.Clear();

                var data = await DBHelper.GetTransactionsAsync(
                    _customerId,
                    _subSchemeId,
                    _accountNumber,
                    _pigmyAgentId,
                    FromDate,     // ✅ using bound property, not private field
                    ToDate
                );

                foreach (var txn in data)
                    Transactions.Add(txn);


                if (Transactions.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Info", "No transactions found for selected period.", "OK");
                }

                IsStatementVisible = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load statement: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ExportToPdf()
        {
            try
            {
                string customerName;
                string SubSchemeName = _subSchemeName; 
                string accountNumber = _accountNumber.ToString();

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{BaseURL.Url()}api/home/{_customerId}");

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to fetch customer details.", "OK");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<HomeResponse>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid customer data received.", "OK");
                    return;
                }

                customerName = data.CustomerName;


                var filePath = await StatementPdfExporter.ExportToDownloadAsync(
                    Transactions.ToList(),
                    customerName,
                    accountNumber,
                    SubSchemeName,
                    FromDate,
                    ToDate
                );

                await Shell.Current.DisplayAlert("Success", $"PDF saved to: {filePath}", "OK");

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }


    }
}
