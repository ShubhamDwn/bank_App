using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using bank_demo.Services;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class BeneficiaryDetailPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private int _customerId;
        private int _accountNumber;

        public int CustomerId
        {
            get => _customerId;
            set { _customerId = value; OnPropertyChanged(); }
        }

        public int AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        public Command LoadBeneficiariesCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PaymentCommand { get; }

        private Beneficiary _selectedBeneficiary;
        public Beneficiary SelectedBeneficiary
        {
            get => _selectedBeneficiary;
            set
            {
                _selectedBeneficiary = value;
                OnPropertyChanged();
            }
        }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public BeneficiaryDetailPageViewModel(int customerId, int accountNumber)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            CustomerId = customerId;
            AccountNumber = accountNumber;

            DeleteCommand = new Command(async () => await DeleteBeneficiaryAsync());
            PaymentCommand = new Command(async () => await NavigateToPaymentPageAsync());
            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            LoadBeneficiariesCommand.Execute(null);
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                using var conn = await DBHelper.GetConnectionAsync();

                string query = @"
                    SELECT 
                        BeneficiaryName, 
                        BankName, 
                        IFSC, 
                        AccountNumber, 
                        BranchName, 
                        BeneficiaryNickName,
                        CustomerId
                    FROM BeneficiaryDetail
                    WHERE CustomerId = @CustomerId AND AccountNumber = @AccountNumber";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);

                using var reader = await cmd.ExecuteReaderAsync();

                Beneficiaries.Clear();

                while (await reader.ReadAsync())
                {
                    Beneficiaries.Add(new Beneficiary
                    {
                        BeneficiaryName = reader["BeneficiaryName"].ToString()!,
                        BankName = reader["BankName"].ToString()!,
                        IFSCCode = reader["IFSC"].ToString()!,
                        AccountNumber = Convert.ToInt32(reader["AccountNumber"]),
                        BranchName = reader["BranchName"].ToString()!,
                        BeneficiaryNickName = reader["BeneficiaryNickName"]?.ToString() ?? "",
                        CustomerId = Convert.ToInt32(reader["CustomerId"])
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Unable to load beneficiary details: {ex.Message}", "OK");
            }

        }

        private async Task NavigateToPaymentPageAsync()
        {
            var beneficiary = Beneficiaries.FirstOrDefault();
            if (beneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "No beneficiary found", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"EnterAmountPage?account_number={beneficiary.AccountNumber}&customer_id={beneficiary.CustomerId}");
        }



        private async Task DeleteBeneficiaryAsync()
        {
            var beneficiary = Beneficiaries.FirstOrDefault();
            if (beneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "No beneficiary found", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete {beneficiary.BeneficiaryName}?",
                "Yes", "No");

            if (!confirm) return;

            try
            {
                using var conn = await DBHelper.GetConnectionAsync();
                string query = @"DELETE FROM BeneficiaryDetail 
                         WHERE CustomerId = @CustomerId 
                           AND AccountNumber = @AccountNumber 
                           AND BeneficiaryAccountNumber = @BeneficiaryAccountNumber";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerId", beneficiary.CustomerId);
                cmd.Parameters.AddWithValue("@AccountNumber", beneficiary.AccountNumber);

                int rows = await cmd.ExecuteNonQueryAsync();

                if (rows > 0)
                {
                    Beneficiaries.Clear();
                    await Shell.Current.DisplayAlert("Success", "Beneficiary deleted", "OK");
                    await Shell.Current.GoToAsync(".."); // Navigate back
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Deletion failed", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not delete: {ex.Message}", "OK");
            }
        }









        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
