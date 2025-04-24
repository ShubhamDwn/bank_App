using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages;

public partial class AddBeneficiaryPage : ContentPage
{
    public AddBeneficiaryPage()
    {
        InitializeComponent();
        BindingContext = new AddBeneficiaryViewModel();
    }
}
