using System.Windows.Input;
using bank_demo.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bank_demo.ViewModels.FeaturesPages.PaymentsVM
{
    public class SendMoneyViewModel : INotifyPropertyChanged
    {
        private string contactName;
        private string mobileNumber;
        private string amount;
        private string note;

        public string ContactName
        {
            get => contactName;
            set { contactName = value; OnPropertyChanged(); }
        }

        public string MobileNumber
        {
            get => mobileNumber;
            set { mobileNumber = value; OnPropertyChanged(); }
        }

        public string Amount
        {
            get => amount;
            set { amount = value; OnPropertyChanged(); }
        }

        public string Note
        {
            get => note;
            set { note = value; OnPropertyChanged(); }
        }

        public ICommand ProceedCommand { get; }

        public SendMoneyViewModel()
        {
            ProceedCommand = new Command(OnProceed);
        }

        private async void OnProceed()
        {
            if (!string.IsNullOrWhiteSpace(Amount) && !string.IsNullOrWhiteSpace(MobileNumber))
            {
                // Implement actual payment service logic here.
                await App.Current.MainPage.DisplayAlert("Transaction", $"Sending ₹{Amount} to {ContactName}", "OK");

                await Shell.Current.GoToAsync(".."); // or navigate to success screen
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please fill in all fields", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
