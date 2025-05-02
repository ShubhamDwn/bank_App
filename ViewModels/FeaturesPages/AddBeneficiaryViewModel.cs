using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using bank_demo.Services; // Assuming DBHelper is here

namespace bank_demo.ViewModels.FeaturesPages
{
    public class AddBeneficiaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        

        private int _accountNumber;
        public int AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
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

        public AddBeneficiaryViewModel(int accountNumber)
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
                string checkQuery = @"SELECT COUNT(*) FROM bankdb.beneficiaries 
                                      WHERE account_number = @AccountNumber 
                                      AND BeneficiaryAccountNumber = @BeneficiaryAccountNumber";

                using var checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                checkCmd.Parameters.AddWithValue("@BeneficiaryAccountNumber", beneficiaryAccountNumber);

                var result = (long)await checkCmd.ExecuteScalarAsync();
                if (result > 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Beneficiary already exists for this account.", "OK");
                    return;
                }

                // Insert beneficiary
                string insertQuery = @"INSERT INTO bankdb.beneficiaries
                    (account_number, BankName, IFSCCode, BeneficiaryAccountNumber, Bank_Branch, BeneficiaryNickname)
                    VALUES
                    (@AccountNumber, @BankName, @IFSCCode, @BeneficiaryAccountNumber, @Branch, @Nickname)";

                using var insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                insertCmd.Parameters.AddWithValue("@BankName", BankName);
                insertCmd.Parameters.AddWithValue("@IFSCCode", IFSCCode);
                insertCmd.Parameters.AddWithValue("@BeneficiaryAccountNumber", beneficiaryAccountNumber);
                insertCmd.Parameters.AddWithValue("@Branch", Branch);
                insertCmd.Parameters.AddWithValue("@Nickname", Nickname ?? ""); // Allow NULL or empty nickname

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
