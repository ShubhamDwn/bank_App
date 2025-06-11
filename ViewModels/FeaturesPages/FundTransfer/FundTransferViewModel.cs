using bank_demo.Services.API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    public class FundTransferViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private int _customerId;
        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadBeneficiariesCommand { get; }
        public ICommand SelectBeneficiaryCommand { get; }

        public FundTransferViewModel(int customerId)
        {
            CustomerId = customerId;
            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            LoadBeneficiariesCommand.Execute(null);
            SelectBeneficiaryCommand = new Command<Beneficiary>(OnBeneficiarySelected);
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                string baseUrl = BaseURL.Url(); // From your BaseURL class
                string url = $"{baseUrl}api/beneficiaries?customerId={CustomerId}";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var beneficiaries = JsonSerializer.Deserialize<List<Beneficiary>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Beneficiaries.Clear();
                    foreach (var b in beneficiaries)
                    {
                        b.CustomerId = CustomerId;
                        Beneficiaries.Add(b);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to load beneficiaries: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load beneficiaries: " + ex.Message, "OK");
            }
        }

        private async void OnBeneficiarySelected(Beneficiary selected)
        {
            if (selected == null) return;

            await Shell.Current.GoToAsync($"EnterAmountPage?account_number={CustomerId}&beneficiary_account_number={selected.AccountNumber}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
