using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages
{
    [QueryProperty(nameof(AccountNumber), "account_number")]
    public partial class BeneficiaryStatusPage : ContentPage
    {
        private int _accountNumber;

        public int AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
                Console.WriteLine($"[DEBUG] Account number received via QueryProperty: {_accountNumber}");

                if (_accountNumber > 0)
                {
                    var viewModel = new BeneficiaryStatusPageViewModel(_accountNumber);
                    BindingContext = viewModel;
                }
            }
        }
        private async void OnAddBeneficiaryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"AddBeneficiaryPage?account_number={AccountNumber}"); 
        }

        public BeneficiaryStatusPage()
        {
            InitializeComponent();
        }
    }
}
