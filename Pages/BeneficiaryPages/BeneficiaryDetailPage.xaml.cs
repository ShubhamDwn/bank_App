using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages.BeneficiaryPages;

[QueryProperty(nameof(CustomerId), "CustomerId")]
[QueryProperty(nameof(AccountNumber), "AccountNumber")]
public partial class BeneficiaryDetailPage : ContentPage
{
    private string _accountNumber;
    private long _customerId;
    private bool _isCustomerIdSet = false;
    private bool _isAccountNumberSet = false;

    public BeneficiaryDetailPage()
    {
        InitializeComponent();
    }

    public string AccountNumber
    {
        get => _accountNumber;
        set
        {
            _accountNumber = value;
            _isAccountNumberSet = true;
            Console.WriteLine($"[DEBUG] AccountNumber received: {_accountNumber}");
            TryInitializeViewModel();
        }
    }

    public long CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            _isCustomerIdSet = true;
            Console.WriteLine($"[DEBUG] CustomerId received: {_customerId}");
            TryInitializeViewModel();
        }
    }

    private void TryInitializeViewModel()
    {
        if (_isCustomerIdSet && _isAccountNumberSet && BindingContext == null)
        {
            BindingContext = new BeneficiaryDetailPageViewModel(_accountNumber);
            Console.WriteLine("[DEBUG] ViewModel initialized successfully.");
        }
    }
}
