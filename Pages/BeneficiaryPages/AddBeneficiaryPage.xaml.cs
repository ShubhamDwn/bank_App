using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages.BeneficiaryPages
{
    public partial class AddBeneficiaryPage : ContentPage
    {     
        public AddBeneficiaryPage()
        {

            InitializeComponent();
            BindingContext = new AddBeneficiaryViewModel();
        }
    }
}
