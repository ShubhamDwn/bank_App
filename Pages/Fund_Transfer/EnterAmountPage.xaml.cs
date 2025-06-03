using Microsoft.Maui.Controls;
using System;
using bank_demo.ViewModels.FeaturesPages.FundTransfer;

namespace bank_demo.Pages.Fund_Transfer;

[QueryProperty(nameof(AccountNumber), "account_number")]
[QueryProperty(nameof(BeneficiaryAccountNumber), "beneficiary_account_number")]
public partial class EnterAmountPage : ContentPage
{
    private int _accountNumber;
    private int _beneficiaryAccountNumber;
    private bool _isAccountSet = false;
    private bool _isBeneficiarySet = false;
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
            BindingContext = new EnterAmountViewModel(_accountNumber, _beneficiaryAccountNumber);
            Console.WriteLine("[DEBUG] ViewModel initialized with both values.");
        }
    }
    public EnterAmountPage()
    {
        InitializeComponent();
    }
}





