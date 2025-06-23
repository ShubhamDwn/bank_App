using System;
using Microsoft.Maui.Controls;
using bank_demo.ViewModels;
using System.Threading.Tasks;

namespace bank_demo.Pages.HomePages
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

            MenuDrawer.TranslationX = -DrawerWidth;
            MenuDrawer.Opacity = 0;
            MenuDrawer.InputTransparent = true;
            MenuOverlay.IsVisible = false;
        }

        private async void OnHamburgerClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = true;
            MenuDrawer.InputTransparent = false;
            MenuDrawer.IsVisible = true;

            await MenuDrawer.TranslateTo(0, 0, DrawerAnimationDuration, Easing.CubicOut);
            await MenuDrawer.FadeTo(1, DrawerAnimationDuration, Easing.CubicOut);

            if (BindingContext is ViewModels.HomeViewModel vm)
            {
                vm.IsMenuVisible = true;
            }
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await MenuDrawer.TranslateTo(-DrawerWidth, 0, DrawerAnimationDuration, Easing.CubicIn);
            await MenuDrawer.FadeTo(0, DrawerAnimationDuration, Easing.CubicIn);

            MenuDrawer.InputTransparent = true;
            MenuOverlay.IsVisible = false;

            if (BindingContext is ViewModels.HomeViewModel vm)
            {
                vm.IsMenuVisible = false;
            }
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is HomeViewModel vm)
                vm.StartCarousel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is HomeViewModel vm)
                vm.StopCarousel();
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

        private async void OnBenfeciaryClicked(object sender, EventArgs e)
        {
            // Access the instance's _customerId
            int account_number = this._customerId;

            // Navigate to the BeneficiaryStatusPage and pass the account_number
            await Shell.Current.GoToAsync($"BeneficiaryStatusPage?account_number={account_number}");
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
