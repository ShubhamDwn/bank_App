using System;
using Microsoft.Maui.Controls;
using bank_demo.Services;
using bank_demo.Services.API;

namespace bank_demo.Pages.Fund_Transfer;

public partial class TransferMethodPage : ContentPage
{
    public Beneficiary SelectedBeneficiary { get; set; }
    public decimal Amount { get; set; }

    // Modify constructor to accept parameters
    public TransferMethodPage(Beneficiary beneficiary, decimal amount)
    {
        InitializeComponent();
        SelectedBeneficiary = beneficiary;
        Amount = amount;
        // Do something with these values if needed
    }
}
