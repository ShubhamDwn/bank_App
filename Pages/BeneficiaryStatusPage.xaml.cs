using bank_demo.ViewModels.FeaturesPages;
using bank_demo.Services;
namespace bank_demo.Pages
{
    [QueryProperty(nameof(CustomerId), "CustomerId")]
    public partial class BeneficiaryStatusPage : ContentPage
    {
        private int _customerId;

        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                Console.WriteLine($"[DEBUG] Account number received via QueryProperty: {_customerId}");

                if (_customerId > 0)
                {
                    var viewModel = new BeneficiaryStatusViewModel(_customerId);
                    BindingContext = viewModel;
                }
            }
        }
        private async void OnAddBeneficiaryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"AddBeneficiaryPage?CustomerId={CustomerId}"); 
        }

        public BeneficiaryStatusPage()
        {
            InitializeComponent();
        }
    }
}
