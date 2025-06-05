using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages
{
    [QueryProperty(nameof(CustomerId), "CustomerId")]
    public partial class AddBeneficiaryPage : ContentPage
    {
        private int _customerId;

        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                BindingContext = new AddBeneficiaryViewModel(_customerId); // Set the BindingContext when AccountNumber is set
            }
        }

        public AddBeneficiaryPage()
        {

            InitializeComponent();
        }
    }
}
