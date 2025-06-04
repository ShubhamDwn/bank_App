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
                var cmd = new SqlCommand("SELECT UserPassword,CustomerId FROM Customer WHERE UserName = @username", conn);
                cmd.Parameters.AddWithValue("@username", Username);

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", "No user found with this username.", "OK");
                    return;
                }

                string storedHashedPassword = reader["UserPassword"].ToString();
                int CustomerId = Convert.ToInt32(reader["CustomerID"]);

                string enteredHashedPassword = SecurityHelper.HashPassword(Password);

                await Application.Current.MainPage.DisplayAlert("Debug", $"Entered: {enteredHashedPassword}\nStored: {storedHashedPassword}", "OK");


                if (storedHashedPassword == enteredHashedPassword)
                {
                    await Shell.Current.GoToAsync($"///HomePage?CustomerId={CustomerId}");
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
            await Shell.Current.GoToAsync("ForgotPasswordPage");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
