using Microsoft.Maui.Controls;
using System;
using bank_demo.ViewModels.FeaturesPages.FundTransfer;

namespace bank_demo.Pages.Fund_Transfer;

[QueryProperty(nameof(CustomerId), "CustomerId")]
[QueryProperty(nameof(AccountNumber), "AccountNumber")]
public partial class EnterAmountPage : ContentPage
{
    private string _accountNumber;
    private int _customerId;
    private bool _customerReady, _accountReady;

    public string AccountNumber
    {
        get => _accountNumber;
        set
        {
            _accountNumber = value;
            _accountReady = true;
            InitViewModelIfReady();
        }
    }

    public int CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            _customerReady = true;
            InitViewModelIfReady();
        }
    }

    private void InitViewModelIfReady()
    {
        if (_accountReady && _customerReady && BindingContext == null)
        {
            BindingContext = new EnterAmountViewModel(_accountNumber, _customerId);
        }
    }

    public EnterAmountPage()
    {
        InitializeComponent();
    }
}





