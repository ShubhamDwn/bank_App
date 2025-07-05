using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using bank_demo.Services;
using bank_demo.Services.API;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    public class EnterAmountViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private readonly OtpService _otpService;

        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private Beneficiary _selectedBeneficiary;
        public Beneficiary SelectedBeneficiary
        {
            get => _selectedBeneficiary;
            set { _selectedBeneficiary = value; OnPropertyChanged(); }
        }

        private string _accountNumber;
        private int _customerId;
        private string _amount;
        private string _remarks;
        private bool _isLoading;

        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        public int CustomerId
        {
            get => _customerId;
            set { _customerId = value; OnPropertyChanged(); }
        }

        public string Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        public string Remarks
        {
            get => _remarks;
            set { _remarks = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public List<string> TransferOptions { get; set; } = new() { "NEFT", "IMPS", "RTGS" };

        private string _selectedTransferOption;
        public string SelectedTransferOption
        {
            get => _selectedTransferOption;
            set { _selectedTransferOption = value; OnPropertyChanged(); }
        }

        public ICommand LoadBeneficiaryCommand { get; }
        public ICommand ProceedCommand { get; }

        public EnterAmountViewModel(string accountNumber, int customerId)
        {
            _httpClient = new HttpClient();
            _otpService = new OtpService();

            AccountNumber = accountNumber;
            CustomerId = customerId;

            LoadBeneficiaryCommand = new Command(async () => await LoadBeneficiaryAsync());
            ProceedCommand = new Command(async () => await OnProceedAsync());

            LoadBeneficiaryCommand.Execute(null);
        }

        private async Task LoadBeneficiaryAsync()
        {
            IsLoading = true;
            try
            {
                string url = $"{BaseURL.Url()}api/beneficiaries/list";
                var request = new { CustomerId = CustomerId };

                var response = await _httpClient.PostAsJsonAsync(url, request);
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
                        if (beneficiary.AccountNumber?.Trim() == AccountNumber?.Trim())
                        {
                            SelectedBeneficiary = beneficiary;
                            Beneficiaries.Add(beneficiary);
                            break;
                        }
                    }
                }

                if (SelectedBeneficiary == null)
                {
                    await Shell.Current.DisplayAlert("Not Found", "Beneficiary not found.", "OK");
                    await Shell.Current.GoToAsync("..");
                }

                await LoadAvailableTransferModes(SelectedBeneficiary?.IFSC);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load beneficiary: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAvailableTransferModes(string ifsc)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ifsc)) return;

                string url = $"{BaseURL.Url()}api/transaction/availableModes?ifsc={ifsc}";
                var modes = await _httpClient.GetFromJsonAsync<List<string>>(url);

                TransferOptions = modes ?? new List<string> { "NEFT", "IMPS", "RTGS" };
                SelectedTransferOption = TransferOptions.FirstOrDefault();
                OnPropertyChanged(nameof(TransferOptions));
                OnPropertyChanged(nameof(SelectedTransferOption));
            }
            catch
            {
                TransferOptions = new List<string> { "NEFT", "IMPS", "RTGS" };
                SelectedTransferOption = TransferOptions.FirstOrDefault();
                OnPropertyChanged(nameof(TransferOptions));
                OnPropertyChanged(nameof(SelectedTransferOption));
            }
        }

        private async Task OnProceedAsync()
        {
            if (string.IsNullOrWhiteSpace(Amount) || !decimal.TryParse(Amount, out decimal parsedAmount) || parsedAmount <= 0)
            {
                await Shell.Current.DisplayAlert("Validation Error", "Enter a valid amount.", "OK");
                return;
            }

            if (SelectedBeneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "No beneficiary selected.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedTransferOption))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Please select a transfer method.", "OK");
                return;
            }

            var verified = await _otpService.SendAndVerifyOtpAsync("transaction");
            if (!verified) return;

            try
            {
                var transactionRequest = new NeftTransactionRequest
                {
                    SubSchemeId = 2,
                    AccountNumber = long.Parse(AccountNumber),
                    CustomerId = (int)CustomerId,
                    PaymentTypeId = GetPaymentTypeId(SelectedTransferOption),
                    BranchId = 1,
                    TransactionDate = DateTime.Now,
                    NeftAmount = parsedAmount,
                    NeftCharges = 0,
                    BenificiaryRemark = Remarks,
                    BenificiaryIFSCCode = SelectedBeneficiary.IFSC,
                    BenificiaryAccountNumber = SelectedBeneficiary.AccountNumber,
                    BenificiaryAccountHolderName = SelectedBeneficiary.BeneficiaryName,
                    BenificiaryBranchName = SelectedBeneficiary.BranchName,
                    BenificiaryBankName = SelectedBeneficiary.BankName,
                    IsFileGenerate = false,
                    UserName = Preferences.Get("UserName", "AppUser"),
                    BenificiaryMobileNo = SelectedBeneficiary.MobileNo,
                    BenificiaryEmail = SelectedBeneficiary.Email
                };

                var json = JsonSerializer.Serialize(transactionRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseURL.Url()}api/transaction/neft-transaction", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Failed", $"Server error: {responseBody}", "OK");
                    return;
                }

                var result = JsonSerializer.Deserialize<TransactionResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    await Shell.Current.DisplayAlert("Success", result.Message, "OK");
                    await Shell.Current.GoToAsync($"TransactionDetailPage?transactionId={result.TransactionId}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Failed", result?.Message ?? "Unknown error", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
        }

        private int GetPaymentTypeId(string method) =>
            method?.ToUpper() switch
            {
                "NEFT" => 1,
                "IMPS" => 2,
                "RTGS" => 3,
                "UPI" => 4,
                "SWIFT" => 5,
                _ => 1
            };

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
