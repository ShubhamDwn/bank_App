using bank_demo.Pages; // Adjust this if AppShell is under a different namespace

namespace bank_demo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set Shell as MainPage
        MainPage = new AppShell();

    }
}
