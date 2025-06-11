using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages;

[QueryProperty(nameof(CustomerId), "CustomerId")]
[QueryProperty(nameof(AccountNumber), "AccountNumber")]
public partial class BeneficiaryDetailPage : ContentPage
{
    private int _customerId;
    private string _accountNumber;
    private bool _isAccountSet = false;
    private bool _isBeneficiarySet = false;

    public BeneficiaryDetailPage()
    {
        InitializeComponent();
    }

    public int CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            _isAccountSet = true;
            Console.WriteLine($"[DEBUG] AccountNumber received: {_customerId}");
            TryInitializeViewModel();
        }
    }

    public string AccountNumber
    {
        get => _accountNumber;
        set
        {
            _accountNumber = value;
            _isBeneficiarySet = true;
            Console.WriteLine($"[DEBUG] BeneficiaryAccountNumber received: {_accountNumber}");
            TryInitializeViewModel();
        }
    }

    private void TryInitializeViewModel()
    {
        if (_isAccountSet && _isBeneficiarySet)
        {
            BindingContext = new BeneficiaryDetailPageViewModel(_customerId, _accountNumber);
            Console.WriteLine("[DEBUG] ViewModel initialized with both values.");
        }
    }
}
