using bank_demo.ViewModels.FeaturesPages;
using bank_demo.Services;
namespace bank_demo.Pages.BeneficiaryPages
{
    public partial class BeneficiaryStatusPage : ContentPage
    {
        private async void OnAddBeneficiaryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"AddBeneficiaryPage"); 
        }

        public BeneficiaryStatusPage()
        {
            InitializeComponent();
            BindingContext = new BeneficiaryStatusViewModel(); // ? Set the ViewModel

        }
    }
}
