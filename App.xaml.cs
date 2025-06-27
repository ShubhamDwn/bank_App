using bank_demo.Pages; // Adjust this if AppShell is under a different namespace

namespace bank_demo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Application.Current.UserAppTheme = AppTheme.Light;

        // Set Shell as MainPage
        MainPage = new AppShell();

    }
}
