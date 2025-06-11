using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Services.API;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class BeneficiaryStatusViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new ObservableCollection<Beneficiary>();
        public ICommand SelectBeneficiaryCommand { get; }

        private readonly int _customerId;
        private readonly HttpClient _httpClient;

        public BeneficiaryStatusViewModel(int customerId)
        {
            _customerId = customerId;
            _httpClient = new HttpClient(); // Ideally injected or reused via DI/singleton
            SelectBeneficiaryCommand = new Command<Beneficiary>(OnSelectBeneficiary);
            LoadBeneficiaries();
        }

        private async void LoadBeneficiaries()
        {
            try
            {
                string url = $"{BaseURL.Url()}api/beneficiaries?customerId={_customerId}";
                var result = await _httpClient.GetFromJsonAsync<List<Beneficiary>>(url);

                Beneficiaries.Clear();
                if (result != null)
                {
                    foreach (var beneficiary in result)
                        Beneficiaries.Add(beneficiary);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load beneficiaries: {ex.Message}", "OK");
            }
        }

        private async void OnSelectBeneficiary(Beneficiary selected)
        {
            if (selected == null) return;

            string customerId = selected.CustomerId.ToString();
            string accountNumber = selected.AccountNumber.ToString();

            await Shell.Current.GoToAsync($"BeneficiaryDetailPage?CustomerId={selected.CustomerId}&AccountNumber={selected.AccountNumber}");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
