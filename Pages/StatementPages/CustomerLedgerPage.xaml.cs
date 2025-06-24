using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages.StatementPages;

[QueryProperty(nameof(CustomerId), "CustomerId")]
public partial class CustomerLedgerPage : ContentPage
{
    private int _customerId;
    private bool _customerSet;

    public CustomerLedgerPage()
	{
		InitializeComponent();
	}

    public int CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            _customerSet = true;
            TryInitializeViewModel();
        }
    }

    private void TryInitializeViewModel()
    {
        if (_customerSet)
        {
                BindingContext = new CustomerLedgerViewModel(_customerId);

                //Shell.Current.DisplayAlert("subscheme Name", _subSchemeName, "OK");
                //Shell.Current.DisplayAlert("Success", "Data Loaded", "OK");
        }
        else
        {
            Shell.Current.DisplayAlert("Error", "Unable to Load Data", "OK");
        }
    }
}