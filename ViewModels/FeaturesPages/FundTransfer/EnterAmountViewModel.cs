﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using bank_demo.Pages.Fund_Transfer;
using bank_demo.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace bank_demo.ViewModels.FeaturesPages.FundTransfer
{
    public class EnterAmountViewModel : BaseViewModel
    {
        public ObservableCollection<Beneficiary> Beneficiaries { get; set; } = new();

        private int _accountNumber;
        private int _beneficiaryAccountNumber;

        public int AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        public int BeneficiaryAccountNumber
        {
            get => _beneficiaryAccountNumber;
            set { _beneficiaryAccountNumber = value; OnPropertyChanged(); }
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

        public ICommand LoadBeneficiariesCommand { get; }
        public ICommand ProceedCommand { get; }

        public EnterAmountViewModel(int accountNumber, int beneficiaryAccountNumber)
        {
            AccountNumber = accountNumber;
            BeneficiaryAccountNumber = beneficiaryAccountNumber;

            LoadBeneficiariesCommand = new Command(async () => await LoadBeneficiariesAsync());
            LoadBeneficiariesCommand.Execute(null);

            ProceedCommand = new Command(OnProceed);
            SelectedTransferOption = TransferOptions.First();
        }

        private async Task LoadBeneficiariesAsync()
        {
            try
            {
                using var conn = await DBHelper.GetConnectionAsync();
                string query = @"SELECT BeneficiaryName, BeneficiaryBankName, BeneficiaryIFSCCode, 
                                        BeneficiaryAccountNumber, BeneficiaryBankBranch, BeneficiaryNickname
                                 FROM beneficiaries 
                                 WHERE LoginedAccountNumber = @AccountNumber AND BeneficiaryAccountNumber = @BeneficiaryAccountNumber";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                cmd.Parameters.AddWithValue("@BeneficiaryAccountNumber", BeneficiaryAccountNumber);

                using var reader = await cmd.ExecuteReaderAsync();

                Beneficiaries.Clear();
                while (await reader.ReadAsync())
                {
                    Beneficiaries.Add(new Beneficiary
                    {
                        Name = reader["BeneficiaryName"].ToString(),
                        BankName = reader["BeneficiaryBankName"].ToString(),
                        IFSCCode = reader["BeneficiaryIFSCCode"].ToString(),
                        BeneficiaryAccountNumber = Convert.ToInt32(reader["BeneficiaryAccountNumber"]),
                        Branch = reader["BeneficiaryBankBranch"].ToString(),
                        Nickname = reader["BeneficiaryNickname"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load beneficiary details: " + ex.Message, "OK");
            }
        }

        private async void OnProceed()
        {
            if (string.IsNullOrEmpty(Amount))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter an amount", "OK");
                return;
            }

            var beneficiary = Beneficiaries.FirstOrDefault();
            if (beneficiary == null)
            {
                await Shell.Current.DisplayAlert("Error", "Beneficiary not found", "OK");
                return;
            }

            string dateTime = DateTime.Now.ToString("dd MMM yyyy hh:mm tt");

            string summary =
                $"🧾 Bank Transfer Receipt\n" +
                $"-----------------------------\n" +
                $"👤 Beneficiary: {beneficiary.Name}\n" +
                $"🏦 Bank Name: {beneficiary.BankName}\n" +
                $"💰 Amount: ₹{Amount}\n" +
                $"✏️ Remarks: {Remarks}\n" +
                $"🔄 Transfer Mode: {SelectedTransferOption}\n" +
                $"📅 Date & Time: {dateTime}\n" +
                $"-----------------------------";

            bool confirm = await Shell.Current.DisplayAlert("Confirm Transfer", summary, "Confirm", "Cancel");

            if (confirm)
            {
                // TODO: Add database logic to save the transaction

                bool share = await Shell.Current.DisplayAlert("Success", "Transfer Initiated", "Share", "OK");

                if (share)
                {
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Title = "Fund Transfer Details",
                        Text = summary
                    });
                }

                // Optional: Navigate back or to another page
                // await Shell.Current.GoToAsync(nameof(FundTransferPage));
            }
        }
    }
}
