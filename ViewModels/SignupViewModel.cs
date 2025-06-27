using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;
using bank_demo.Services;
using bank_demo.Services.API;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace bank_demo.ViewModels
{
    public class SignupViewModel : BaseViewModel
    {
        private string _customerId;
        public string CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }

        private string _pin;
        public string Pin
        {
            get => _pin;
            set => SetProperty(ref _pin, value);
        }

        private string _confirmPin;
        public string ConfirmPin
        {
            get => _confirmPin;
            set => SetProperty(ref _confirmPin, value);
        }

        public ICommand SignupCommand { get; }
        private readonly OtpService _otpService;

        public SignupViewModel()
        {
            SignupCommand = new Command(async () => await ExecuteSignup(false));
            _otpService = new OtpService();
        }

        private async Task ExecuteSignup(bool forceOverride)
        {
            if (string.IsNullOrWhiteSpace(CustomerId) || string.IsNullOrWhiteSpace(Pin))
            {
                await Shell.Current.DisplayAlert("Error", "Customer ID and 4-digit PIN are required.", "OK");
                return;
            }

            if (!Regex.IsMatch(Pin, @"^\d{4}$"))
            {
                await Shell.Current.DisplayAlert("Error", "PIN must be a 4-digit number.", "OK");
                return;
            }

            if (Pin != ConfirmPin)
            {
                await Shell.Current.DisplayAlert("Error", "PINs do not match.", "OK");
                return;
            }

            bool otpVerified = await _otpService.SendAndVerifyOtpAsync("registered-mobile-number");
            if (!otpVerified)
                return;

            string deviceId = Preferences.Get("DeviceId", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Preferences.Set("DeviceId", deviceId);
            }

            var signupRequest = new SignupRequest
            {
                CustomerId = this.CustomerId,
                Pin = this.Pin,
                DeviceId = deviceId,
                ForceOverride = forceOverride
            };

            var json = JsonSerializer.Serialize(signupRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PostAsync($"{BaseURL.Url()}api/auth/signup", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                SignupResponse? signupResponse = null;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Try to parse known failure response
                    try
                    {
                        signupResponse = JsonSerializer.Deserialize<SignupResponse>(responseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (signupResponse?.Message?.Contains("another device", StringComparison.OrdinalIgnoreCase) == true && !forceOverride)
                        {
                            bool confirm = await Shell.Current.DisplayAlert("Device Conflict",
                                "Account is already registered on another device.\nDo you want to log out the previous device and register this one?",
                                "Yes", "Cancel");

                            if (confirm)
                            {
                                await ExecuteSignup(true); // Retry with override
                                return;
                            }
                        }

                        await Shell.Current.DisplayAlert("Signup Failed", signupResponse?.Message ?? "Signup failed.", "OK");
                        return;
                    }
                    catch
                    {
                        await Shell.Current.DisplayAlert("Signup Failed", $"Server said:\n{responseContent}", "OK");
                        return;
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Error", $"Unexpected error: {response.StatusCode}", "OK");
                    return;
                }

                // Success
                signupResponse = JsonSerializer.Deserialize<SignupResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (signupResponse?.Success == true)
                {
                    await Shell.Current.DisplayAlert("Success", signupResponse.Message, "OK");
                    await AppShell.RecheckDeviceAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Signup Failed", signupResponse?.Message ?? "Unknown failure.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Unexpected exception: {ex.Message}", "OK");
            }
        }
    }

  
}
