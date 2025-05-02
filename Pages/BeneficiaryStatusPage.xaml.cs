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
                if (_accountNumber > 0)
                {
                    BindingContext = new BeneficiaryStatusPageViewModel(_accountNumber);
                }
            }
        }

        public BeneficiaryStatusPage()
        {
            InitializeComponent();
        }
    }
}
