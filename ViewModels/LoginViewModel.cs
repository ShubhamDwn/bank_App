using bank_demo.Services;
using bank_demo.Services.API;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace bank_demo.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _customerName;
        private string _pin;
        private int _customerId;

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }
        public int CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }

        public string Pin
        {
            get => _pin;
            set => SetProperty(ref _pin, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand LoadCustomerNameCommand { get; }


        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
            SignUpCommand = new Command(async () => await SignUpAsync());
            LoadCustomerNameCommand = new Command(async () => await LoadCustomerNameAsync());


           // _ = LoadCustomerNameAsync(); // Load customer name immediately
        }

        

        private async Task LoadCustomerNameAsync()
        {
            try
            {
                int customerId = Preferences.Get("CustomerId", 0);
                CustomerId = customerId;
                if (customerId == 0)
                {
                    CustomerName = "Customer";             
                    return;
                }
                using var client = new HttpClient();
                var response = await client.GetAsync($"{BaseURL.Url()}api/home/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<HomeResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result != null && !string.IsNullOrWhiteSpace(result.CustomerName))
                    {
                        CustomerName = result.CustomerName;
                        Preferences.Set("CustomerName", result.CustomerName);
                    }
                    else
                    {
                        CustomerName = "Customer";
                        await Application.Current.MainPage.DisplayAlert("Info", "Customer name not found in response.", "OK");
                    }
                }
                else
                {
                    CustomerName = "Customer";
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch customer name. Status: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                CustomerName = "Customer";
                await Application.Current.MainPage.DisplayAlert("Error", $"Exception in name fetch: {ex.Message}", "OK");
            }
        }

        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Pin))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "PIN is required.", "OK");
                    return;
                }

                string deviceId = Preferences.Get("DeviceId", string.Empty);
                if (string.IsNullOrWhiteSpace(deviceId))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Device ID missing. Please re-install the app.", "OK");
                    return;
                }

                var loginRequest = new
                {
                    DeviceId = deviceId,
                    Pin = Pin
                };

                var apiUrl = $"{BaseURL.Url()}api/auth/login";
                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                var response = await client.PostAsync(apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid response from server.", "OK");
                    return;
                }

                if (loginResponse.Success)
                {
                    
                    await Shell.Current.GoToAsync($"///HomePage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", loginResponse.Message ?? "Unknown error", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Login Error", ex.Message, "OK");
            }
        }

        private async Task SignUpAsync()
        {
            await Shell.Current.GoToAsync("//Signup");
        }
    }
}
