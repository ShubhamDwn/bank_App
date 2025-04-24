/*using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace bank_demo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Check login status when the page appears
            CheckLoginStatus();
        }

        private async void CheckLoginStatus()
        {
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (!isLoggedIn)
            {
                // Navigate using Shell routing (MVVM-friendly)
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                // Optional: go to homepage if needed
                await Shell.Current.GoToAsync("//Home");
            }
        }
    }
}*/
