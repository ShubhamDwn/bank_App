using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages.Fund_Transfer
{
    public partial class FundTransferPage : ContentPage
    {
        public FundTransferPage()
        {
            InitializeComponent();
            BindingContext = new FundTransferViewModel();
        }

        private async void OnAddBeneficiaryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AddBeneficiaryPage");
        }
    }
}
