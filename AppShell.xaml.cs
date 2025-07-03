using bank_demo.Pages;
using bank_demo.Pages.AuthenticationPage;
using bank_demo.Pages.BeneficiaryPages;
using bank_demo.Pages.Fund_Transfer;
using bank_demo.Pages.HomePages;
using bank_demo.Pages.StatementPages;
using bank_demo.Services.API;
using System.Text.Json;


namespace bank_demo;
public partial class AppShell : Shell
{
    private static AppShell _instance;
    public AppShell()
    {
        InitializeComponent();
        _instance = this;

        // Register routes
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("Signup", typeof(SignupPage));
        Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("Home", typeof(HomePage));
        Routing.RegisterRoute("About", typeof(About));
        Routing.RegisterRoute("Settings", typeof(Settings));
        Routing.RegisterRoute("ViewStatementPage", typeof(ViewStatementPage));

        Routing.RegisterRoute(nameof(CustomerLedgerPage), typeof(CustomerLedgerPage));
        Routing.RegisterRoute(nameof(MyQRCodePage), typeof(MyQRCodePage));
        Routing.RegisterRoute(nameof(ScanToPayPage), typeof(ScanToPayPage));
        Routing.RegisterRoute(nameof(StatementPage), typeof(StatementPage));
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
        Routing.RegisterRoute(nameof(AddBeneficiaryPage), typeof(AddBeneficiaryPage));
        Routing.RegisterRoute(nameof(PaymentsPage), typeof(PaymentsPage));
        Routing.RegisterRoute(nameof(BeneficiaryDetailPage), typeof(BeneficiaryDetailPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));
        Routing.RegisterRoute(nameof(BeneficiaryStatusPage), typeof(BeneficiaryStatusPage));
        Routing.RegisterRoute(nameof(ContactSupportPage), typeof(ContactSupportPage));
        Routing.RegisterRoute(nameof(TermsPage), typeof(TermsPage));
        Routing.RegisterRoute(nameof(FundTransferPage), typeof(FundTransferPage));
        Routing.RegisterRoute(nameof(EnterAmountPage), typeof(EnterAmountPage));

        // 🔹 Device check logic
        Dispatcher.Dispatch(async () => await HandleDeviceCheckAsync());
    }

    public static async Task RecheckDeviceAsync()
    {
        if (_instance != null)
        {
            await _instance.HandleDeviceCheckAsync();
        }
    }


    private async Task HandleDeviceCheckAsync()
    {
        try
        {
            string deviceId = Preferences.Get("DeviceId", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Preferences.Set("DeviceId", deviceId);
            }

            var payload = new { DeviceId = deviceId };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync($"{BaseURL.Url()}api/auth/check-device", content);
            var body = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"Device check response: {body}");

            var result = System.Text.Json.JsonSerializer.Deserialize<DeviceCheckResponse>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Success == true)
            {
                Preferences.Set("CustomerId", result.Id);
                await GoToAsync($"//LoginPage");
            }
            else
            {
                await GoToAsync("//Signup");
            }
        }
        catch (Exception ex)
        {
            // Show error on screen instead of silently redirecting
            await Shell.Current.DisplayAlert("Startup Error", ex.Message, "OK");

            // Optionally still navigate to signup
            await GoToAsync("//Signup");
        }
    }


    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await AppShell.RecheckDeviceAsync();
    }
}

