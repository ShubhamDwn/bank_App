using bank_demo.ViewModels.FeaturesPages;  // Ensure this is added to the top

namespace bank_demo.Pages
{
    [QueryProperty(nameof(AccountNumber), "account_number")]
    public partial class AddBeneficiaryPage : ContentPage
    {
        // Declare the AccountNumber property
        private int _accountNumber;

        public int AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
                BindingContext = new AddBeneficiaryViewModel(_accountNumber); // Pass AccountNumber to ViewModel
            }
        }

        public AddBeneficiaryPage()
        {
            InitializeComponent();
        }
    }

}
