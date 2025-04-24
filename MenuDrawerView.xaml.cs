using System;
using bank_demo.Pages;
using bank_demo.ViewModels.FeaturesPages;
using Microsoft.Maui.Controls;

namespace bank_demo
{
    public partial class MenuDrawerView : ContentView
    {
        public MenuDrawerView()
        {
            InitializeComponent();
            BindingContext = new MenuDrawerViewModel();
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }

        private async void OnTransactionHistoryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TransactionHistoryPage));
        }

        private async void OnBeneficiaryStatusClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(BeneficiaryStatusPage));
        }

        private async void OnContactSupportClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ContactSupportPage));
        }

        private async void OnSecuritySettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SecuritySettingsPage));
        }

        private async void OnTermsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TermsPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
