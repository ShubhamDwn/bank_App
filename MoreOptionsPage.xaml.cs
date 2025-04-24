using System;
using Microsoft.Maui.Controls;
using bank_demo.Pages;

namespace bank_demo
{
    public partial class MoreOptionsPage : ContentPage
    {
        public MoreOptionsPage()
        {
            InitializeComponent();
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }

        private async void OnTransactionHistoryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TransactionHistoryPage");
        }

        private async void OnBeneficiaryStatusClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("BeneficiaryStatusPage");
        }

        private async void OnContactSupportClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ContactSupportPage");
        }

        private async void OnSecuritySettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SecuritySettingsPage");
        }

        private async void OnTermsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TermsPage");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Clear any session info if needed here
            await Shell.Current.GoToAsync("//LoginPage"); // double slash navigates to root
        }
    }
}
