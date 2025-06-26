using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using bank_demo.Services.API;
using Microsoft.Maui.Controls;


namespace bank_demo.ViewModels.FeaturesPages
{
    public class CustomerLedgerViewModel : BaseViewModel
    {
        public ObservableCollection<CustomerAccountLedgerModel> LedgerData { get; set; } = new();

        public ICommand RefreshLedgerCommand { get; }

        private int _customerId;
        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private bool _isLoading;
        private bool _isVisible;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public CustomerLedgerViewModel(int customerId)
        {
            _customerId = customerId;
            SelectedDate = DateTime.Today;

            RefreshLedgerCommand = new Command(async () => await LoadLedgerAsync());

            // auto-load on start
            _ = LoadLedgerAsync();
        }

        private static readonly HttpClient _httpClient = new(); // reuse

        private async Task LoadLedgerAsync()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;
                IsVisible = false;
                string dateParam = SelectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                string url = $"{BaseURL.Url()}api/customerledger/account-ledger" +
                             $"?customerId={_customerId}" +
                             $"&transactionDate={dateParam}" +
                             $"&isClosed=true";

                var result = await _httpClient.GetFromJsonAsync<List<CustomerAccountLedgerModel>>(url);

                if (result == null || result.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Info", "No ledger records found.", "OK");
                    LedgerData.Clear();
                    return;
                }

                LedgerData = new ObservableCollection<CustomerAccountLedgerModel>(result);
                OnPropertyChanged(nameof(LedgerData));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to fetch ledger: {ex.Message}", "OK");
                IsVisible = false;
            }
            finally
            {
                IsLoading = false;
                IsVisible = true;
            }
        }

    }
}

