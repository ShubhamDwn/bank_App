using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using bank_demo.Services.API;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class FundTransferViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        public ICommand SelectBeneficiaryCommand { get; }
        public ICommand LoadBeneficiariesCommand { get; }

        private readonly int _customerId;
        private readonly HttpClient _httpClient;

        public FundTransferViewModel()
        {
            _customerId = Preferences.Get("CustomerId", 0);
            _httpClient = new HttpClient();

            SelectBeneficiaryCommand = new Command<Beneficiary>(OnSelectBeneficiary);
            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiaries());

            LoadBeneficiariesCommand.Execute(null); // Auto-load on initialization
        }

        private async Task LoadBeneficiaries()
        {
            try
            {
                string url = $"{BaseURL.Url()}api/beneficiaries/list";

                var payload = new { CustomerId = _customerId };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<Beneficiary>>();

                    Beneficiaries.Clear();
                    if (result != null)
                    {
                        foreach (var b in result)
                            Beneficiaries.Add(b);
                    }
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Failed to load beneficiaries: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Exception occurred: {ex.Message}", "OK");
            }
        }

        private async void OnSelectBeneficiary(Beneficiary selected)
        {
            if (selected == null) return;

            // Navigate to TransactionListPage with required params
            await Shell.Current.GoToAsync(
                    $"TransactionListPage?" +
                    $"CustomerId={selected.CustomerId}" +
                    $"&AccountNumber={selected.AccountNumber}" +
                    $"&BeneficiaryCode={selected.BeneficiaryCode}" +
                    $"&BeneficiaryName={Uri.EscapeDataString(selected.BeneficiaryName)}" +
                    $"&BankName={Uri.EscapeDataString(selected.BankName)}");

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
