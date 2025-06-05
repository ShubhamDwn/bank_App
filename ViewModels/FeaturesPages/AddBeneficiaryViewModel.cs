using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using bank_demo.Services;

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
            set { _ifsc = value; OnPropertyChanged(); }
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

        public ICommand AddCommand { get;}

        public AddBeneficiaryViewModel(int customerId)
        {
            CustomerId = customerId;
            AddCommand = new Command(async () => await AddAsync());
        }

        private async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(AccountNumber) || string.IsNullOrWhiteSpace(ConfirmAccountNumber) ||
                AccountNumber != ConfirmAccountNumber)
            {
                await Shell.Current.DisplayAlert("Error", "Account numbers do not match or are empty.", "OK");
                return;
            }

            try
            {
                using var conn = await DBHelper.GetConnectionAsync();

                string checkQuery = @"SELECT COUNT(*) FROM BeneficiaryDetail 
                                      WHERE CustomerId = @CustomerId AND AccountNumber = @AccountNumber";
                using var checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                checkCmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);

                var exists = (int)(await checkCmd.ExecuteScalarAsync() ?? 0);
                if (exists > 0)
                {
                    await Shell.Current.DisplayAlert("Duplicate", "This beneficiary is already added.", "OK");
                    return;
                }

                string insertQuery = @"
                    INSERT INTO BeneficiaryDetail (CustomerId, BenificiaryCode, BeneficiaryName, BeneficiaryNickName, AccountNumber, IFSC, MobileNo, Email, BankName, BranchName, IsRegister, RegistrationDate, RegistrationStatus, Status, SysDate)
                    VALUES (@CustomerId, @BeneficiaryCode, @BeneficiaryName, @BeneficiaryNickName, @AccountNumber, @IFSC, @MobileNo, @Email, @BankName, @BranchName, 1, GETDATE(), 'Registered', 1, GETDATE())";

                using var insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                insertCmd.Parameters.AddWithValue("@BeneficiaryCode", AccountNumber+IFSC);
                insertCmd.Parameters.AddWithValue("@BeneficiaryName", BeneficiaryName);
                insertCmd.Parameters.AddWithValue("@BeneficiaryNickName", BeneficiaryNickName ?? "");
                insertCmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                insertCmd.Parameters.AddWithValue("@IFSC", IFSC);
                insertCmd.Parameters.AddWithValue("@MobileNo", MobileNo);
                insertCmd.Parameters.AddWithValue("@Email", Email);
                insertCmd.Parameters.AddWithValue("@BankName", BankName);
                insertCmd.Parameters.AddWithValue("@BranchName", BranchName);

                await insertCmd.ExecuteNonQueryAsync();

                await Shell.Current.DisplayAlert("Success", "Beneficiary added successfully!", "OK");
                await Shell.Current.GoToAsync(".."); // or a detail page
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed: " + ex.Message, "OK");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
