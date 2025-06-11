using bank_demo.Services.API;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Input;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    public class EnterAmountViewModel : BaseViewModel
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        public int AccountNumber { get; set; }
        public int CustomerId { get; set; }

        public string BeneficiaryName => Beneficiaries.FirstOrDefault()?.BeneficiaryName ?? "";
        public string BankName => Beneficiaries.FirstOrDefault()?.BankName ?? "";

        private string _amount;
        public string Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private string _remarks;
        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        private string _selectedTransferOption;
        public string SelectedTransferOption
        {
            get => _selectedTransferOption;
            set => SetProperty(ref _selectedTransferOption, value);
        }

        public List<string> TransferOptions { get; } = new List<string> { "NEFT", "IMPS" };

        public ICommand LoadBeneficiariesCommand { get; }
        public ICommand ProceedCommand { get; }

        public EnterAmountViewModel(int accountNumber, int customerId)
        {
            AccountNumber = accountNumber;
            CustomerId = customerId;

            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            LoadBeneficiariesCommand.Execute(null);

            ProceedCommand = new Command(OnProceed);
            SelectedTransferOption = TransferOptions.First();
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                string endpoint = $"{BaseURL.Url()}api/beneficiaries?customerId={CustomerId}&accountNumber={AccountNumber}";

                using var client = new HttpClient();
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var beneficiaries = JsonSerializer.Deserialize<List<Beneficiary>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Beneficiaries.Clear();
                    foreach (var item in beneficiaries)
                        Beneficiaries.Add(item);

                    OnPropertyChanged(nameof(BeneficiaryName));
                    OnPropertyChanged(nameof(BankName));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to load beneficiaries: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load beneficiary details: " + ex.Message, "OK");
            }
        }


        private async void OnProceed()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter an amount", "OK");
                return;
            }

            var beneficiary = Beneficiaries.FirstOrDefault();
            if (beneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "No beneficiary found", "OK");
                return;
            }

            string dateTime = DateTime.Now.ToString("dd MMM yyyy hh:mm tt");

            string summary =
                $"🧾 Bank Transfer Receipt\n" +
                $"-----------------------------\n" +
                $"👤 Beneficiary: {beneficiary.BeneficiaryName}\n" +
                $"🏦 Bank Name: {beneficiary.BankName}\n" +
                $"💰 Amount: ₹{Amount}\n" +
                $"✏️ Remarks: {Remarks}\n" +
                $"🔄 Transfer Mode: {SelectedTransferOption}\n" +
                $"📅 Date & Time: {dateTime}\n" +
                $"-----------------------------";

            bool confirm = await Shell.Current.DisplayAlert("Confirm Transfer", summary, "Confirm", "Cancel");

            if (confirm)
            {
                bool share = await Shell.Current.DisplayAlert("Success", "Transfer Initiated", "Share", "OK");

                if (share)
                {
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Title = "Fund Transfer Details",
                        Text = summary
                    });
                }
            }
        }
    }
}
