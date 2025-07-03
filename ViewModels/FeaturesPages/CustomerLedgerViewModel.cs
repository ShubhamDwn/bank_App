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
        public ObservableCollection<CustomerAccountLedgerModel> LedgerData { get; private set; } = new();
        private List<CustomerAccountLedgerModel> _allLedgerData = new();

        private int _pageSize = 20;
        private int _currentPage = 0;

        public ICommand RefreshLedgerCommand { get; }
        public ICommand LoadMoreCommand { get; }

        private int _customerId;
        private DateTime _selectedDate;
        private bool _isLoading;
        private bool _isVisible;
        private bool _showClosedAccounts;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

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

        public bool ShowClosedAccounts
        {
            get => _showClosedAccounts;
            set
            {
                if (SetProperty(ref _showClosedAccounts, value))
                {
                    _currentPage = 0;
                    ApplyClosedFilter(reset: true);
                }
            }
        }

        public CustomerLedgerViewModel()
        {
            _customerId = Preferences.Get("CustomerId", 0); ;
            SelectedDate = DateTime.Today;

            RefreshLedgerCommand = new Command(async () => await LoadLedgerAsync());
            LoadMoreCommand = new Command(() => ApplyClosedFilter(reset: false));

            _ = LoadLedgerAsync();
        }

        private static readonly HttpClient _httpClient = new();

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
                             $"&transactionDate={dateParam}";

                var result = await _httpClient.GetFromJsonAsync<List<CustomerAccountLedgerModel>>(url);

                if (result == null || result.Count == 0)
                {
                    LedgerData.Clear();
                    OnPropertyChanged(nameof(LedgerData));
                    await Shell.Current.DisplayAlert("Info", "No ledger records found.", "OK");
                    return;
                }

                _allLedgerData = result;
                _currentPage = 0;
                ApplyClosedFilter(reset: true);
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

        private void ApplyClosedFilter(bool reset)
        {
            if (reset)
            {
                LedgerData.Clear();
                _currentPage = 0;
            }

            var filtered = ShowClosedAccounts
                ? _allLedgerData
                : _allLedgerData.Where(x => !x.Closed);

            var nextPage = filtered
                .Skip(_currentPage * _pageSize)
                .Take(_pageSize)
                .ToList();

            foreach (var item in nextPage)
                LedgerData.Add(item);

            _currentPage++;
            OnPropertyChanged(nameof(LedgerData));
        }
    }
}
