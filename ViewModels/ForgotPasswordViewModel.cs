using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Services;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace bank_demo.ViewModels
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private string _aadhaar;
        private string _newPassword;
        private string _confirmPassword;
        private bool _isOtpVerified;
        private string _mobile; // store mobile after fetching from API

        public string Aadhaar
        {
            get => _aadhaar;
            set => SetProperty(ref _aadhaar, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsOtpVerified
        {
            get => _isOtpVerified;
            set => SetProperty(ref _isOtpVerified, value);
        }

        public ICommand SendOtpCommand { get; }
        public ICommand ResetPasswordCommand { get; }

        private readonly OtpService _otpService;

        public ForgotPasswordViewModel()
        {
            SendOtpCommand = new Command(async () => await SendOtpAsync());
            ResetPasswordCommand = new Command(async () => await ResetPasswordAsync());
            _otpService = new OtpService();
        }

        private async Task SendOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(Aadhaar))
            {
                await Shell.Current.DisplayAlert("Error", "Aadhaar is required.", "OK");
                return;
            }

            if (Aadhaar.Length != 12)
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid 12-digit Aadhaar number.", "OK");
                return;
            }

            try
            {
                // Call your API to get mobile number by Aadhaar
                var httpClient = new HttpClient();
                var response = await httpClient.PostAsync(
                    "http://your-api-url/api/auth/getmobile",
                    new StringContent(JsonSerializer.Serialize(new { Aadhaar }), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to retrieve mobile number.", "OK");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var mobileResponse = JsonSerializer.Deserialize<GetMobileResponse>(json);

                if (mobileResponse == null || string.IsNullOrWhiteSpace(mobileResponse.Mobile))
                {
                    await Shell.Current.DisplayAlert("Error", "Mobile number not found for Aadhaar.", "OK");
                    return;
                }

                _mobile = mobileResponse.Mobile;

                // Send OTP and verify
                IsOtpVerified = await _otpService.SendAndVerifyOtpAsync(_mobile);

                if (!IsOtpVerified)
                {
                    await Shell.Current.DisplayAlert("Error", "OTP verification failed.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
            }
        }

        private async Task ResetPasswordAsync()
        {
            if (!IsOtpVerified)
            {
                await Shell.Current.DisplayAlert("Error", "Please verify OTP before resetting password.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "Password fields cannot be empty.", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (NewPassword.Length < 8)
            {
                await Shell.Current.DisplayAlert("Error", "Password must be at least 8 characters.", "OK");
                return;
            }

            try
            {
                var forgotPasswordRequest = new
                {
                    Aadhaar = this.Aadhaar,
                    NewPassword = this.NewPassword
                };

                var httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(forgotPasswordRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://your-api-url/api/auth/forgotpassword", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var forgotPasswordResponse = JsonSerializer.Deserialize<ForgotPasswordResponse>(responseContent);

                if (forgotPasswordResponse != null && forgotPasswordResponse.Success)
                {
                    await Shell.Current.DisplayAlert("Success", forgotPasswordResponse.Message, "OK");
                    await Shell.Current.GoToAsync("///LoginPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Failed", forgotPasswordResponse?.Message ?? "Password reset failed.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(backingStore, value))
                return;

            backingStore = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GetMobileResponse
    {
        public string Mobile { get; set; }
    }

    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
