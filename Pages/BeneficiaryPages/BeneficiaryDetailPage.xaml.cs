using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages.BeneficiaryPages;

[QueryProperty(nameof(AccountNumber), "AccountNumber")]
public partial class BeneficiaryDetailPage : ContentPage
{
    private string _accountNumber;
    private bool _isAccountSet = false;
    private bool _isBeneficiarySet = false;

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
            _isBeneficiarySet = true;
            Console.WriteLine($"[DEBUG] BeneficiaryAccountNumber received: {_accountNumber}");
            TryInitializeViewModel();
        }
    }

    private void TryInitializeViewModel()
    {
        if (_isAccountSet && _isBeneficiarySet)
        {
            BindingContext = new BeneficiaryDetailPageViewModel(_accountNumber);
            Console.WriteLine("[DEBUG] ViewModel initialized with both values.");
        }
    }
}
