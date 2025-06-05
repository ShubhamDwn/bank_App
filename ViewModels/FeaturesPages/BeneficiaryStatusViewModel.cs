using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using bank_demo.Services;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class BeneficiaryStatusViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new ObservableCollection<Beneficiary>();
        public ICommand SelectBeneficiaryCommand { get; }

        private int _customerId;

        public BeneficiaryStatusViewModel(int customerId)
        {
            _customerId = customerId;
            SelectBeneficiaryCommand = new Command<Beneficiary>(OnSelectBeneficiary);
            LoadBeneficiaries();
        }

        private async void LoadBeneficiaries()
        {
            try
            {
                using var conn = await DBHelper.GetConnectionAsync();
                string query = @"SELECT BeneficiaryName, BankName, IFSC, AccountNumber, BranchName, BeneficiaryNickName 
                                 FROM BeneficiaryDetail 
                                 WHERE CustomerId = @CustomerId AND IsRegister = 1 AND Status = 1";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerId", _customerId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var beneficiary = new Beneficiary
                    {
                        BeneficiaryName = reader["BeneficiaryName"].ToString() ?? "",
                        BankName = reader["BankName"].ToString() ?? "",
                        IFSCCode = reader["IFSC"].ToString() ?? "",
                        BranchName = reader["BranchName"].ToString() ?? "",
                        BeneficiaryNickName = reader["BeneficiaryNickName"].ToString() ?? "",
                        CustomerId = _customerId, // current user
                        AccountNumber = Convert.ToInt32(reader["AccountNumber"])
                    };

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

            await Shell.Current.GoToAsync($"BeneficiaryDetailPage?CustomerId={selected.CustomerId}&AccountNumber={selected.AccountNumber}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
