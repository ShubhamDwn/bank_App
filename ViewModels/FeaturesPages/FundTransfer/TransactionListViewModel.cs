using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using bank_demo.Services.API;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    [QueryProperty(nameof(CustomerId), "CustomerId")]
    [QueryProperty(nameof(AccountNumber), "AccountNumber")]
    [QueryProperty(nameof(BeneficiaryCode), "BeneficiaryCode")]
    [QueryProperty(nameof(BeneficiaryName), "BeneficiaryName")]
    [QueryProperty(nameof(BankName), "BankName")]
    public class TransactionListViewModel : INotifyPropertyChanged
    {
        private long _customerId; // ✅ changed to long
        private string _accountNumber;
        private string _beneficiaryCode;
        private string _beneficiaryName;
        private string _bankName;

        public long CustomerId
        {
            get => _customerId;
            set { _customerId = value; OnPropertyChanged(); }
        }

        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        public string BeneficiaryCode
        {
            get => _beneficiaryCode;
            set { _beneficiaryCode = value; OnPropertyChanged(); }
        }

        public string BeneficiaryName
        {
            get => _beneficiaryName;
            set { _beneficiaryName = value; OnPropertyChanged(); }
        }

        public string BankName
        {
            get => _bankName;
            set { _bankName = value; OnPropertyChanged(); }
        }

        public ICommand SendMoneyCommand { get; }

        public TransactionListViewModel()
        {
            SendMoneyCommand = new Command(OnSendMoney);
        }

        private async void OnSendMoney()
        {
            // This gets overwritten anyway
            CustomerId = Preferences.Get("CustomerId", 0L); // ✅ changed to long

            await Shell.Current.GoToAsync(
                $"EnterAmountPage?CustomerId={CustomerId}" + 
                $"&AccountNumber={AccountNumber}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
