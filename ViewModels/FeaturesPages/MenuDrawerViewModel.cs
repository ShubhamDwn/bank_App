namespace bank_demo.ViewModels.FeaturesPages
{
    public class MenuDrawerViewModel : BaseViewModel
    {
        public Command ProfileCommand { get; }
        public Command HistoryCommand { get; }
        public Command SecuritySettingsCommand { get; }
        public Command LogoutCommand { get; }

        public MenuDrawerViewModel()
        {
            ProfileCommand = new Command(OnProfile);
            HistoryCommand = new Command(OnHistory);
            SecuritySettingsCommand = new Command(OnSecuritySettings);
            LogoutCommand = new Command(OnLogout);
        }

        private async void OnProfile()
        {
            await Shell.Current.GoToAsync("ProfilePage");
        }

        private async void OnHistory()
        {
            await Shell.Current.GoToAsync("TransactionHistoryPage");
        }

        private async void OnSecuritySettings()
        {
            await Shell.Current.GoToAsync("SecuritySettingsPage");
        }

        private async void OnLogout()
        {
            // Example: Clear session and go to login
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
