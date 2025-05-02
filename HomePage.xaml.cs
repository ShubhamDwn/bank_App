using System;
using Microsoft.Maui.Controls;
using bank_demo.ViewModels;
using System.Threading.Tasks;

namespace bank_demo
{
        [QueryProperty(nameof(CustomerId), "CustomerId")]
        public partial class HomePage : ContentPage
        {
            private const uint DrawerAnimationDuration = 250;
            private const double DrawerWidth = 300;

            private int _customerId;

            public int CustomerId
            {
                get => _customerId;
                set
                {
                    _customerId = value;
                    BindingContext = new HomeViewModel(_customerId); // Now ViewModel gets the ID
                }
            }

            public HomePage()
            {
                InitializeComponent();
                // Move drawer off-screen initially
                MenuDrawer.TranslationX = -DrawerWidth;
            }
        


        private async void OnHamburgerClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = true;
            MenuDrawer.IsVisible = true;

            await MenuDrawer.TranslateTo(0, 0, DrawerAnimationDuration, Easing.CubicInOut);
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await HideDrawer();
        }

        private async void OnDrawerArrowClicked(object sender, EventArgs e)
        {
            await HideDrawer();
        }

        private async Task HideDrawer()
        {
            await MenuDrawer.TranslateTo(-DrawerWidth, 0, DrawerAnimationDuration, Easing.CubicInOut);
            MenuDrawer.IsVisible = false;
            MenuOverlay.IsVisible = false;
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Profile", "This is the profile section.", "OK");
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Settings", "Settings page not yet implemented.", "OK");
        }

        private async void OnNewHomePageClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("NewHomePage");
        }
      

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
