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

        private int _customerId;
        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged();
            }
        }

        public Command LoadBeneficiariesCommand { get; }

        public FundTransferViewModel(int customerId)
        {
            CustomerId = customerId;
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
                                 FROM BeneficiaryDetail 
                                 WHERE CustomerId = @CustomerId";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);

                using var reader = await cmd.ExecuteReaderAsync();

                Beneficiaries.Clear();
                while (await reader.ReadAsync())
                {
                    Beneficiaries.Add(new Beneficiary
                    {
                        BeneficiaryName = reader.GetString("BeneficiaryName"),
                        BankName = reader.GetString("BeneficiaryBankName"),
                        IFSCCode = reader.GetString("BeneficiaryIFSCCode"),
                        AccountNumber = reader.GetInt32("AccountNumber"),
                        BranchName = reader.GetString("BeneficiaryBankBranch"),
                        BeneficiaryNickName = reader.IsDBNull(reader.GetOrdinal("BeneficiaryNickname")) ? "" : reader.GetString("BeneficiaryNickname"),
                        CustomerId = CustomerId // ✅ This line fixes the error
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

            await Shell.Current.GoToAsync($"EnterAmountPage?account_number={CustomerId}&beneficiary_account_number={selected.AccountNumber}");
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
