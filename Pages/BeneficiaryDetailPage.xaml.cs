using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages;

[QueryProperty(nameof(AccountNumber), "account_number")]
[QueryProperty(nameof(BeneficiaryAccountNumber), "beneficiary_account_number")]
public partial class BeneficiaryDetailPage : ContentPage
{
    private int _accountNumber;
    private int _beneficiaryAccountNumber;
    private bool _isAccountSet = false;
    private bool _isBeneficiarySet = false;

    public BeneficiaryDetailPage()
    {
        InitializeComponent();
    }

    public int AccountNumber
    {
        get => _accountNumber;
        set
        {
            _accountNumber = value;
            _isAccountSet = true;
            Console.WriteLine($"[DEBUG] AccountNumber received: {_accountNumber}");
            TryInitializeViewModel();
        }
    }

    public int BeneficiaryAccountNumber
    {
        get => _beneficiaryAccountNumber;
        set
        {
            _beneficiaryAccountNumber = value;
            _isBeneficiarySet = true;
            Console.WriteLine($"[DEBUG] BeneficiaryAccountNumber received: {_beneficiaryAccountNumber}");
            TryInitializeViewModel();
        }
    }

    private void TryInitializeViewModel()
    {
        if (_isAccountSet && _isBeneficiarySet)
        {
            BindingContext = new BeneficiaryDetailPageViewModel(_accountNumber, _beneficiaryAccountNumber);
            Console.WriteLine("[DEBUG] ViewModel initialized with both values.");
        }
    }
}
