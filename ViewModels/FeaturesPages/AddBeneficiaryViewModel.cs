using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Services.API;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class AddBeneficiaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CustomerId { get; set; }

        private string _beneficiaryName;
        public string BeneficiaryName
        {
            get => _beneficiaryName;
            set { _beneficiaryName = value; OnPropertyChanged(); }
        }

        private string _beneficiaryNickName;
        public string BeneficiaryNickName
        {
            get => _beneficiaryNickName;
            set { _beneficiaryNickName = value; OnPropertyChanged(); }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        private string _confirmAccountNumber;
        public string ConfirmAccountNumber
        {
            get => _confirmAccountNumber;
            set { _confirmAccountNumber = value; OnPropertyChanged(); }
        }

        private string _ifsc;
        public string IFSC
        {
            get => _ifsc;
            set
            {
                _ifsc = value; OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(_ifsc) && _ifsc.Length == 11)
                    _ = FetchBankDetailsByIFSC();

            }
        }

        private string _mobileNo;
        public string MobileNo
        {
            get => _mobileNo;
            set { _mobileNo = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _bankName;
        public string BankName
        {
            get => _bankName;
            set { _bankName = value; OnPropertyChanged(); }
        }

        private string _branchName;
        public string BranchName
        {
            get => _branchName;
            set { _branchName = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private string _city;
        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); }
        }

        private string _district;
        public string District
        {
            get => _district;
            set { _district = value; OnPropertyChanged(); }
        }

        private string _state;
        public string State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(); }
        }


        public ICommand AddCommand { get; }

        public AddBeneficiaryViewModel(int customerId)
        {
            CustomerId = customerId;
            AddCommand = new Command(async () => await AddAsync());
        }


        private async Task FetchBankDetailsByIFSC()
        {
            try
            {
                using var client = new HttpClient();
                var url = $"https://ifsc.razorpay.com/{IFSC.Trim().ToUpper()}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var bankData = JsonSerializer.Deserialize<IFSCBankInfo>(json);

                    if (bankData != null)
                    {
                        BankName = bankData.BANK;
                        BranchName = bankData.BRANCH;
                        Address = bankData.ADDRESS;
                        City = bankData.CITY;
                        District = bankData.DISTRICT;
                        State = bankData.STATE;
                    }
                }
                else
                {
                    BankName = BranchName = Address = City = District = State = string.Empty;
                    await Shell.Current.DisplayAlert("Info", "Bank details not found for this IFSC.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to fetch IFSC details: {ex.Message}", "OK");
            }
        }

        private async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(AccountNumber) || string.IsNullOrWhiteSpace(ConfirmAccountNumber) ||
                AccountNumber != ConfirmAccountNumber)
            {
                await Shell.Current.DisplayAlert("Error", "Account numbers do not match or are empty.", "OK");
                return;
            }

            var beneficiary = new
            {
                CustomerId,
                BeneficiaryName,
                BeneficiaryNickName,
                AccountNumber,
                ConfirmAccountNumber,
                IFSC,
                MobileNo,
                Email,
                BankName,
                BranchName
            };


            try
            {
                string baseurl = BaseURL.Url();
                using var client = new HttpClient();
                client.BaseAddress = new Uri(baseurl); // ⛳ Replace with actual API base URL

                var json = JsonSerializer.Serialize(beneficiary);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/beneficiaries", content);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Success", "Beneficiary added successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    var respContent = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Failed: {respContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
            }
        }



        private void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


    }
}