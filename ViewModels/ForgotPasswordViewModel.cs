using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public ICommand ResetPasswordCommand { get; }

        public ForgotPasswordViewModel()
        {
            ResetPasswordCommand = new Command(async () => await ResetPasswordAsync());
        }

        private async Task ResetPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
            {
                await Shell.Current.DisplayAlert("Invalid", "Please enter a valid email address.", "OK");
                return;
            }

            // Simulate email sending logic
            await Shell.Current.DisplayAlert("Link Sent", $"Password reset link sent to {Email}.", "OK");

            // Navigate back to login
            await Shell.Current.GoToAsync("//LoginPage");

        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
