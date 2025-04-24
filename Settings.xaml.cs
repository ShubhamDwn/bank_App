namespace bank_demo;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
	}
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        Preferences.Set("IsLoggedIn", false);
        await Shell.Current.GoToAsync("//LoginPage");
    }

}