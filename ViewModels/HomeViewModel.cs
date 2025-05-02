using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Pages;
using MySql.Data.MySqlClient;
using bank_demo.Services; // Assuming DBHelper is here
using System.Data;

namespace bank_demo.ViewModels 
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _customerName;
        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); }
        }

        private decimal _savingsBalance;
        public decimal SavingsBalance
        {
            get => _savingsBalance;
            set { _savingsBalance = value; OnPropertyChanged(); }
        }

        public ICommand AboutCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand QRCommand { get; }
        public ICommand ScanToPayCommand { get; }
        public ICommand StatementCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand AddBeneficiaryCommand { get; }
        public ICommand PaymentsCommand { get; }

        public HomeViewModel(int customerId)
        {
            LoadCustomerData(customerId);

            AboutCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("About");
            });

            HomeCommand = new Command(async () =>
            {
                await Shell.Current.DisplayAlert("Home", "Already on Home page", "OK");
            });

            SettingsCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("Settings");
            });

            QRCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(MyQRCodePage));
            });

            ScanToPayCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ScanToPayPage));
            });

            StatementCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(StatementPage));
            });

            HistoryCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(HistoryPage));
            });

            AddBeneficiaryCommand = new Command(async () =>
            {
                // Pass the account number as a query parameter
                await Shell.Current.GoToAsync($"///AddBeneficiaryPage?AccountNumber={customerId}");
            });

            PaymentsCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(PaymentsPage));
            });
        }

        private async void LoadCustomerData(int AccountNumber)
        {
            try
            {
                using var conn = await DBHelper.GetConnectionAsync();
                string query = "SELECT full_name FROM bankdb.users WHERE account_number = @AccountNumber";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    CustomerName = reader.GetString("full_name");
                    //SavingsBalance = reader.GetDecimal("balance");  
                    SavingsBalance = 50000.00M;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load customer data: " + ex.Message, "OK");
            }
        }

    }
}
