using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using bank_demo.Services;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace bank_demo.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private bool _isRememberMe;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public bool IsRememberMe
        {
            get => _isRememberMe;
            set { _isRememberMe = value; OnPropertyChanged(nameof(IsRememberMe)); }
        }

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
            SignUpCommand = new Command(async () => await SignUpAsync());
            ForgotPasswordCommand = new Command(async () => await ForgotPasswordAsync());
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Username and password cannot be empty.", "OK");
                    return;
                }

                using var conn = await DBHelper.GetConnectionAsync();
                var cmd = new SqlCommand("SELECT * FROM users WHERE Username = @username", conn);
                cmd.Parameters.AddWithValue("@username", Username);

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", "No user found with this username.", "OK");
                    return;
                }

                string storedHashedPassword = reader["Password"].ToString();
                int accountNumber = Convert.ToInt32(reader["LoginedAccountNumber"]);

                string enteredHashedPassword = HashPassword(Password);

                if (storedHashedPassword == enteredHashedPassword)
                {
                    await Shell.Current.GoToAsync($"///HomePage?CustomerId={accountNumber}");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid password.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task SignUpAsync()
        {
            await Shell.Current.GoToAsync("Signup");
        }

        private async Task ForgotPasswordAsync()
        {
            string email = await Application.Current.MainPage.DisplayPromptAsync(
                "Forgot Password",
                "Enter your email to reset your password:",
                "Send",
                "Cancel",
                placeholder: "example@email.com");

            if (string.IsNullOrWhiteSpace(email))
                return;

            if (email.ToLower().Contains("@") && email.ToLower().Contains("."))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Reset Link Sent",
                    $"A password reset link has been sent to {email}.",
                    "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Invalid Email",
                    "Please enter a valid email address.",
                    "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
