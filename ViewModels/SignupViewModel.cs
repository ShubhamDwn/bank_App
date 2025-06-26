using System.Text.RegularExpressions;
using System.Windows.Input;
using bank_demo.Services;
using bank_demo.Services.API;
using Microsoft.Maui.Controls;



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
            // 1. Validate fields
            if (string.IsNullOrWhiteSpace(Aadhaar) || string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (!Regex.IsMatch(Aadhaar, @"^\d{12}$"))
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid 12-digit Aadhaar number.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (Password.Length < 8)
            {
                await Shell.Current.DisplayAlert("Error", "Password must be at least 8 characters long.", "OK");
                return;
            }

            // 2. OTP verification
            bool otpVerified = await _otpService.SendAndVerifyOtpAsync("registered-mobile-number");
            if (!otpVerified)
                return;

            // 3. Send signup request
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
                var response = await httpClient.PostAsync($"{BaseURL.Url()}api/auth/signup", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Try to parse server JSON response even if it's a 400/500
                SignupResponse signupResponse = null;

                try
                {
                    signupResponse = System.Text.Json.JsonSerializer.Deserialize<SignupResponse>(responseContent,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    // Optional fallback: show raw response
                    await Shell.Current.DisplayAlert("Server Error", $"Unexpected response: {responseContent}", "OK");
                    return;
                }

                if (signupResponse != null)
                {
                    if (signupResponse.Success)
                    {
                        await Shell.Current.DisplayAlert("Success", signupResponse.Message, "OK");
                        await Shell.Current.GoToAsync("///LoginPage");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Signup Failed", signupResponse.Message, "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await Shell.Current.DisplayAlert("Network Error", $"Server unreachable: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Unexpected Error", $"Error: {ex.Message}", "OK");
            }


        }
    }
}
