using System.Windows.Input;
using System.Text.RegularExpressions;
using bank_demo.Services;
using Microsoft.Maui.Controls;
using bank_demo.Services.API;

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
            // Validate inputs
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

            // Step 1: Verify OTP (on registered mobile)
            // Assuming you already have a way to get the mobile linked to Aadhaar, else call an API to fetch it
            // For demo, we assume OTP sent to mobile and user input handled here
            bool otpVerified = await _otpService.SendAndVerifyOtpAsync("registered-mobile-number");
            if (!otpVerified)
            {
                return; // OTP failed, stop signup
            }

            // Step 2: Call Signup API
            var signupRequest = new
            {
                Aadhaar = this.Aadhaar,
                Username = this.Username,
                Password = this.Password
            };

            var json = System.Text.Json.JsonSerializer.Serialize(signupRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PostAsync("http://192.168.12:5164/api/auth/signup", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var signupResponse = System.Text.Json.JsonSerializer.Deserialize<SignupResponse>(responseContent);

                if (signupResponse != null && signupResponse.Success)
                {
                    await Shell.Current.DisplayAlert("Success", signupResponse.Message, "OK");
                    await Shell.Current.GoToAsync("///LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Failed", signupResponse?.Message ?? "Signup failed", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
            }
        }
    }
}
