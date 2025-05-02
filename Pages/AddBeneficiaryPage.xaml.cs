using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages
{
    [QueryProperty(nameof(AccountNumber), "account_number")]
    public partial class AddBeneficiaryPage : ContentPage
    {
        private int _accountNumber;

        public int AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
                BindingContext = new AddBeneficiaryViewModel(_accountNumber); // Set the BindingContext when AccountNumber is set
            }
        }

        public AddBeneficiaryPage()
        {

            InitializeComponent();
        }
    }
}
