using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages.StatementPages;

public partial class CustomerLedgerPage : ContentPage
{

    private bool _customerSet;

    public CustomerLedgerPage()
	{
		InitializeComponent();
        BindingContext = new CustomerLedgerViewModel();
    }
}