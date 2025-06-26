using bank_demo.Services;
using bank_demo.Services.API;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;



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

                var loginRequest = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                var apiUrl = $"{BaseURL.Url()}api/auth/login"; // Use your actual IP and port here

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true; // Ignore SSL errors for dev

                using var client = new HttpClient(handler);
                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                var responseBody = await response.Content.ReadAsStringAsync();


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, options);

                // Step 3.2: Check if deserialization failed
                if (loginResponse == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid response from server.", "OK");
                    return;
                }

                // Step 3.3: Use the success flag properly
                if (loginResponse.Success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Login successful", "OK");
                    await Shell.Current.GoToAsync($"///HomePage?CustomerId={loginResponse.CustomerId}");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", loginResponse.Message, "OK");
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
