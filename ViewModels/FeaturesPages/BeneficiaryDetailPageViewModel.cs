using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Services.API;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class BeneficiaryDetailPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private readonly long _customerId;
        private readonly string _accountNumber;
        private readonly HttpClient _httpClient;

        public Command LoadBeneficiariesCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PaymentCommand { get; }

        private Beneficiary _selectedBeneficiary;
        public Beneficiary SelectedBeneficiary
        {
            get => _selectedBeneficiary;
            set { _selectedBeneficiary = value; OnPropertyChanged(); }
        }

        public BeneficiaryDetailPageViewModel(string accountNumber)
        {
            _customerId = Preferences.Get("CustomerId", 0);
            _accountNumber = accountNumber;
            _httpClient = new HttpClient();

            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            DeleteCommand = new Command(async () => await DeleteBeneficiaryAsync());
            PaymentCommand = new Command(async () => await NavigateToPaymentPageAsync());

            LoadBeneficiariesCommand.Execute(null);
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                string url = $"{BaseURL.Url()}api/beneficiaries/list";

                var payload = new
                {
                    CustomerId = _customerId
                };

                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load beneficiaries.", "OK");
                    return;
                }

                var result = await response.Content.ReadFromJsonAsync<List<Beneficiary>>();
                Beneficiaries.Clear();

                if (result != null)
                {
                    foreach (var beneficiary in result)
                    {
                        if (beneficiary.AccountNumber == _accountNumber)
                        {
                            SelectedBeneficiary = beneficiary;
                            Beneficiaries.Add(beneficiary); // Show only matched one in details page
                            break;
                        }
                    }
                }

                if (SelectedBeneficiary == null)
                {
                    await Shell.Current.DisplayAlert("Not Found", "Beneficiary not found.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Unable to load beneficiary details: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToPaymentPageAsync()
        {
            var beneficiary = SelectedBeneficiary;
            if (beneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "No beneficiary selected.", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"EnterAmountPage?account_number={beneficiary.AccountNumber}&customer_id={beneficiary.CustomerId}");
        }

        private async Task DeleteBeneficiaryAsync()
        {
            var beneficiary = SelectedBeneficiary;
            if (beneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "No beneficiary selected.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete {beneficiary.BeneficiaryName}?",
                "Yes", "No");

            if (!confirm) return;

            try
            {
                string deleteUrl = $"{BaseURL.Url()}api/beneficiaries/delete";

                var payload = new
                {
                    CustomerId = _customerId,
                    AccountNumber = _accountNumber,
                    BeneficiaryAccountNumber = beneficiary.AccountNumber
                };

                var response = await _httpClient.PostAsJsonAsync(deleteUrl, payload);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Success", "Beneficiary deleted.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Deletion failed: {msg}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not delete: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
