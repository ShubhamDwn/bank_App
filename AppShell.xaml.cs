using bank_demo.Pages;

namespace bank_demo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register all your routes
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("Signup", typeof(SignupPage));
        Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("Home", typeof(HomePage));
        Routing.RegisterRoute("About", typeof(About));
        Routing.RegisterRoute("Settings", typeof(Settings));

        Routing.RegisterRoute(nameof(MyQRCodePage), typeof(bank_demo.Pages.MyQRCodePage));
        Routing.RegisterRoute(nameof(ScanToPayPage), typeof(ScanToPayPage));
        Routing.RegisterRoute(nameof(StatementPage), typeof(Pages.StatementPage));
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
        Routing.RegisterRoute(nameof(AddBeneficiaryPage), typeof(AddBeneficiaryPage));
        Routing.RegisterRoute(nameof(PaymentsPage), typeof(PaymentsPage));
        Routing.RegisterRoute(nameof(BeneficiaryDetailPage), typeof(BeneficiaryDetailPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));
        Routing.RegisterRoute(nameof(BeneficiaryStatusPage), typeof(BeneficiaryStatusPage));
        Routing.RegisterRoute(nameof(ContactSupportPage), typeof(ContactSupportPage));
        Routing.RegisterRoute(nameof(SecuritySettingsPage), typeof(SecuritySettingsPage));
        Routing.RegisterRoute(nameof(TermsPage), typeof(TermsPage));



        // Navigate to LoginPage after AppShell is loaded
        Dispatcher.Dispatch(async () =>
        {
            await GoToAsync("//LoginPage");
        });
    }
    // Logout handler
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
    
  


}
