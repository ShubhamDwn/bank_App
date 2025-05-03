using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using bank_demo.Services;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class AddBeneficiaryViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.



        private int _accountNumber;
        public int AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _bankName;
        public string BankName
        {
            get => _bankName;
            set { _bankName = value; OnPropertyChanged(); }
        }

        private string _ifscCode;
        public string IFSCCode
        {
            get => _ifscCode;
            set { _ifscCode = value; OnPropertyChanged(); }
        }

        private string _branch;
        public string Branch
        {
            get => _branch;
            set { _branch = value; OnPropertyChanged(); }
        }

        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set { _nickname = value; OnPropertyChanged(); }
        }

        private string _beneficiaryAccountNumberText;
        public string BeneficiaryAccountNumberText
        {
            get => _beneficiaryAccountNumberText;
            set { _beneficiaryAccountNumberText = value; OnPropertyChanged(); }
        }

        private string _confirmAccountNumberText;
        public string ConfirmAccountNumberText
        {
            get => _confirmAccountNumberText;
            set { _confirmAccountNumberText = value; OnPropertyChanged(); }
        }

        public ICommand AddBeneficiaryCommand { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public AddBeneficiaryViewModel(int accountNumber)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            AccountNumber = accountNumber;
            Console.WriteLine("Function called");
            AddBeneficiaryCommand = new Command(async () => await AddBeneficiaryAsync());
        }


        private async Task AddBeneficiaryAsync()
        {
            // --- VALIDATIONS ---
            if (string.IsNullOrWhiteSpace(BankName) ||
                string.IsNullOrWhiteSpace(IFSCCode) ||
                string.IsNullOrWhiteSpace(Branch) ||
                string.IsNullOrWhiteSpace(BeneficiaryAccountNumberText) ||
                string.IsNullOrWhiteSpace(ConfirmAccountNumberText))
            {
                await Shell.Current.DisplayAlert("Validation Error", "All fields are required except Nickname.", "OK");
                return;
            }

            if (BeneficiaryAccountNumberText != ConfirmAccountNumberText)
            {
                await Shell.Current.DisplayAlert("Validation Error", "Beneficiary Account numbers do not match.", "OK");
                return;
            }

            if (!int.TryParse(BeneficiaryAccountNumberText, out int beneficiaryAccountNumber))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Beneficiary Account Number must be a valid number.", "OK");
                return;
            }

            try
            {
                using var conn = await DBHelper.GetConnectionAsync();

                // Check for duplicates first
                string checkQuery = @"SELECT COUNT(*) FROM beneficiaries 
                                      WHERE LoginedAccountNumber = @AccountNumber 
                                      AND BeneficiaryAccountNumber = @BeneficiaryAccountNumber";

                using var checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                checkCmd.Parameters.AddWithValue("@BeneficiaryAccountNumber", beneficiaryAccountNumber);

#pragma warning disable CS8605 // Unboxing a possibly null value.
                var result = (int)(await checkCmd.ExecuteScalarAsync() ?? 0);
#pragma warning restore CS8605 // Unboxing a possibly null value.
                if (result > 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Beneficiary already exists for this account.", "OK");
                    return;
                }

                // Insert beneficiary
                string insertQuery = @"INSERT INTO beneficiaries
                    (LoginedAccountNumber, BeneficiaryName, BeneficiaryBankName, BeneficiaryIFSCCode, BeneficiaryAccountNumber, BeneficiaryBankBranch, BeneficiaryNickname)
                    VALUES
                    (@AccountNumber,@Name, @BankName, @IFSCCode, @BeneficiaryAccountNumber, @Branch, @Nickname)";

                using var insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                insertCmd.Parameters.AddWithValue("@Name", Name);
                insertCmd.Parameters.AddWithValue("@BankName", BankName);
                insertCmd.Parameters.AddWithValue("@IFSCCode", IFSCCode);
                insertCmd.Parameters.AddWithValue("@BeneficiaryAccountNumber", beneficiaryAccountNumber);
                insertCmd.Parameters.AddWithValue("@Branch", Branch);
                insertCmd.Parameters.AddWithValue("@Nickname", Nickname ?? "");

                await insertCmd.ExecuteNonQueryAsync();

                await Shell.Current.DisplayAlert("Success", "Beneficiary added successfully!", "OK");
                await Shell.Current.GoToAsync($"BeneficiaryStatusPage?account_number={AccountNumber}");  
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to add beneficiary: " + ex.Message, "OK");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
