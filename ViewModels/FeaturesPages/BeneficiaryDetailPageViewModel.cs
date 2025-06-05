using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using bank_demo.Services;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class BeneficiaryDetailPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private int _accountNumber;
        private int _beneficiaryAccountNumber;

        public int AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        public int BeneficiaryAccountNumber
        {
            get => _beneficiaryAccountNumber;
            set { _beneficiaryAccountNumber = value; OnPropertyChanged(); }
        }

        public Command LoadBeneficiariesCommand { get; }

        public BeneficiaryDetailPageViewModel(int accountNumber, int beneficiaryAccountNumber)
        {
            AccountNumber = accountNumber;
            BeneficiaryAccountNumber = beneficiaryAccountNumber;

            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            LoadBeneficiariesCommand.Execute(null);
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                using var conn = await DBHelper.GetConnectionAsync();
                string query = @"SELECT BeneficiaryName, BeneficiaryBankName, BeneficiaryIFSCCode, 
                                        BeneficiaryAccountNumber, BeneficiaryBankBranch, BeneficiaryNickname
                                 FROM beneficiaries 
                                 WHERE LoginedAccountNumber = @AccountNumber AND BeneficiaryAccountNumber = @BeneficiaryAccountNumber";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                cmd.Parameters.AddWithValue("@BeneficiaryAccountNumber", BeneficiaryAccountNumber);

                using var reader = await cmd.ExecuteReaderAsync();

                Beneficiaries.Clear();
                while (await reader.ReadAsync())
                {
                    Beneficiaries.Add(new Beneficiary
                    {
                        BeneficiaryName = reader["BeneficiaryName"].ToString(),
                        BankName = reader["BeneficiaryBankName"].ToString(),
                        IFSCCode = reader["BeneficiaryIFSCCode"].ToString(),
                        BeneficiaryAccountNumber = Convert.ToInt32(reader["BeneficiaryAccountNumber"]),
                        BranchName = reader["BeneficiaryBankBranch"].ToString(),
                        BeneficiaryNickName = reader["BeneficiaryNickname"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load beneficiary details: " + ex.Message, "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
