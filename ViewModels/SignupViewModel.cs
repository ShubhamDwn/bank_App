using bank_demo.Services;
using bank_demo.Services.API;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace bank_demo.ViewModels
{
    public class SignupViewModel : BaseViewModel
    {
        private string _aadhaar;
        public string Aadhaar
        {
            get => _aadhaar;
            set => SetProperty(ref _aadhaar, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand SignupCommand { get; }

        private readonly OtpService _otpService;

        public SignupViewModel()
        {
            SignupCommand = new Command(async () => await ExecuteSignup());
            _otpService = new OtpService();
        }

        private async Task ExecuteSignup()
        {
            if (string.IsNullOrWhiteSpace(Aadhaar) || string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "All fields are required", "OK");
                return;
            }

            if (!Regex.IsMatch(Aadhaar, @"^\d{12}$"))
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid 12-digit Aadhaar number", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            if (Password.Length < 8)
            {
                await Shell.Current.DisplayAlert("Error", "Password must be at least 8 characters", "OK");
                return;
            }

            var hashedPassword = SecurityHelper.HashPassword(Password);

            var signupRequest = new SignupRequest
            {
                Aadhaar = Aadhaar,
                Username = Username,
                Password = hashedPassword
            };

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var client = new HttpClient(handler);
            var json = JsonSerializer.Serialize(signupRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string apiUrl = "http://192.168.1.6:5164/api/auth/signup"; // Replace with actual

            try
            {
                var response = await client.PostAsync(apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Error", "Server error. Try again.", "OK");
                    return;
                }

                var result = JsonSerializer.Deserialize<SignupResponse>(responseBody);

                if (result.Success)
                {
                    await Shell.Current.DisplayAlert("Success", result.Message, "OK");
                    await Shell.Current.GoToAsync("///LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Signup Failed", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }
}
