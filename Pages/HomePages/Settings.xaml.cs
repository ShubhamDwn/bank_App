using bank_demo.Services.API;
using System.Text;
using System.Text.Json;

namespace bank_demo.Pages.HomePages;
[QueryProperty(nameof(CustomerId), "CustomerId")]
public partial class Settings : ContentPage
{
    private int _customerId;

    public int CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            Application.Current.MainPage.DisplayAlert("Debug", $"Received CustomerId: {_customerId}", "OK");
        }
    }
    public Settings()
	{
		InitializeComponent();
	}
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        Preferences.Set("IsLoggedIn", false);
        await AppShell.RecheckDeviceAsync();
    }

    private async void OnLogoutAllDevicesClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Confirm Logout",
            "Are you sure you want to logout from all devices?",
            "Yes", "Cancel");

        if (!confirm) return;

        try
        {
            var customerIdStr = Preferences.Get("CustomerId", string.Empty);
            if (!int.TryParse(customerIdStr, out int customerId))
            {
                await DisplayAlert("Error", "Customer ID not found.", "OK");
                return;
            }

            var payload = new { CustomerId = customerId };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync($"{BaseURL.Url()}api/auth/logout-all", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<LogoutAllResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result != null && result.Success)
            {
                await DisplayAlert("Success", result.Message, "OK");
            }
            else
            {
                await DisplayAlert("Failed", result?.Message ?? "Logout failed.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }



}