using bank_demo.Services;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    public class FundTransferViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private int _accountNumber;
        public int AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
                OnPropertyChanged();
            }
        }

        public Command LoadBeneficiariesCommand { get; }

        public FundTransferViewModel(int accountNumber)
        {
            AccountNumber = accountNumber;
            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            LoadBeneficiariesCommand.Execute(null);
            SelectBeneficiaryCommand = new Command<Beneficiary>(OnBeneficiarySelected);
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                using var conn = await DBHelper.GetConnectionAsync();
                string query = @"SELECT BeneficiaryName, BeneficiaryBankName, BeneficiaryIFSCCode, BeneficiaryAccountNumber, BeneficiaryBankBranch, BeneficiaryNickname
                                 FROM beneficiaries 
                                 WHERE LoginedAccountNumber = @AccountNumber";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);

                using var reader = await cmd.ExecuteReaderAsync();

                Beneficiaries.Clear();
                while (await reader.ReadAsync())
                {
                    Beneficiaries.Add(new Beneficiary
                    {
                        Name = reader.GetString("BeneficiaryName"),
                        BankName = reader.GetString("BeneficiaryBankName"),
                        IFSCCode = reader.GetString("BeneficiaryIFSCCode"),
                        BeneficiaryAccountNumber = reader.GetInt32("BeneficiaryAccountNumber"),
                        Branch = reader.GetString("BeneficiaryBankBranch"),
                        Nickname = reader.IsDBNull(reader.GetOrdinal("BeneficiaryNickname")) ? "" : reader.GetString("BeneficiaryNickname")
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load beneficiaries: " + ex.Message, "OK");
            }
        }

        public ICommand SelectBeneficiaryCommand { get; }

        private async void OnBeneficiarySelected(Beneficiary selected)
        {
            if (selected == null) return;

            await Shell.Current.GoToAsync($"BeneficiaryDetailPage?account_number={AccountNumber}&beneficiary_account_number={selected.BeneficiaryAccountNumber}");
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
