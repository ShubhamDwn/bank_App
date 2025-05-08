using System.Windows.Input;
using bank_demo.Pages.Fund_Transfer;
using bank_demo.Services;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    public class EnterAmountViewModel : BaseViewModel
    {
        private string _beneficiaryName;
        public string BeneficiaryName
        {
            get => _beneficiaryName;
            set => SetProperty(ref _beneficiaryName, value);
        }

        private string _bankName;
        public string BankName
        {
            get => _bankName;
            set => SetProperty(ref _bankName, value);
        }

        private string _amount;
        public string Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private string _remarks;
        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        private string _selectedTransferOption;
        public string SelectedTransferOption
        {
            get => _selectedTransferOption;
            set => SetProperty(ref _selectedTransferOption, value);
        }

        public List<string> TransferOptions { get; } = new List<string> { "NEFT", "IMPS" };


        public ICommand ProceedCommand { get; }

        public EnterAmountViewModel()
        {
            ProceedCommand = new Command(OnProceed);
            SelectedTransferOption = TransferOptions.First();
        }

        private async void OnProceed()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter an amount", "OK");
                return;
            }

           

            string dateTime = DateTime.Now.ToString("dd MMM yyyy hh:mm tt");

            string summary =
                $"🧾 Bank Transfer Receipt\n" +
                $"-----------------------------\n" +
                $"👤 Beneficiary: {BeneficiaryName}\n" +
                $"🏦 Bank Name: {BankName}\n" +
                $"💰 Amount: ₹{Amount}\n" +
                $"✏️ Remarks: {Remarks}\n" +
                $"🔄 Transfer Mode: {SelectedTransferOption}\n" +
                $"📅 Date & Time: {dateTime}\n" +
                $"-----------------------------";

            bool confirm = await Shell.Current.DisplayAlert("Confirm Transfer", summary, "confirm", "Cancel");

            if (confirm)
            {
                // Show a success message with Share option
                bool share = await Shell.Current.DisplayAlert("Success", "Transfer Initiated", "Share", "OK");

                if (share)
                {
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Title = "Fund Transfer Details",
                        Text = summary
                    });
                }
                //await Shell.Current.GoToAsync(nameof(FundTransferPage));

            }
            
        }



    }
}
