using bank_demo.ViewModels.FeaturesPages;
using bank_demo.Services;
using bank_demo.ViewModels.FeaturesPages.FundTransfer;
namespace bank_demo.Pages.Fund_Transfer
{

    [QueryProperty(nameof(AccountNumber), "account_number")]
    public partial class FundTransferPage : ContentPage
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
                    var viewModel = new FundTransferViewModel(_accountNumber);
                    BindingContext = viewModel;
                }
            }
        }
        private async void OnAddBeneficiaryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"AddBeneficiaryPage?account_number={AccountNumber}");
        }

        public FundTransferPage()
        {
            InitializeComponent();
        }
    }
}
