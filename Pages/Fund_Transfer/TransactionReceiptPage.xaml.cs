using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace bank_demo.Pages.Fund_Transfer;

public partial class TransactionReceiptPage : ContentPage
{
	public TransactionReceiptPage()
	{
		InitializeComponent();
	}

    private async void OnShareReceiptClicked(object sender, EventArgs e)
    {
        await Share.RequestAsync(new ShareTextRequest
        {
            Text = "Transaction details here",
            Title = "Share Receipt"
        });
    }
}