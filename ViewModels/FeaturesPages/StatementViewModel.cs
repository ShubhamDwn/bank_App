﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Services;
using Microsoft.Maui.Storage;

namespace bank_demo.ViewModels.FeaturesPages;

public class StatementViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<string> AccountTypes { get; } = new();
    public ObservableCollection<AccountModel> AvailableAccounts { get; } = new();

    public List<string> TimePeriodOptions { get; } = new() { "Last Week", "Last 1 Month", "Last 3 Months", "Last 1 Year", "Custom" };

    private string _selectedAccountType;
    public string SelectedAccountType
    {
        get => _selectedAccountType;
        set
        {
            if (_selectedAccountType != value)
            {
                _selectedAccountType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAccountTypeSelected));

            }
        }
    }

    private bool _isAccountListVisible;
    public bool IsAccountListVisible
    {
        get => _isAccountListVisible;
        set
        {
            _isAccountListVisible = value;
            OnPropertyChanged();
        }
    }

    private AccountModel _selectedAccount;
    public AccountModel SelectedAccount
    {
        get => _selectedAccount;
        set
        {
            if (_selectedAccount != value)
            {
                _selectedAccount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAccountSelected));
                OnPropertyChanged(nameof(IsTimePeriodVisible));
                OnPropertyChanged(nameof(IsViewStatementVisible));
            }
        }
    }

    private string _selectedTimePeriod;
    public string SelectedTimePeriod
    {
        get => _selectedTimePeriod;
        set
        {
            if (_selectedTimePeriod != value)
            {
                _selectedTimePeriod = value;
                IsCustomDateRange = value == "Custom";
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsViewStatementVisible));
            }
        }
    }

    private bool _isCustomDateRange;
    public bool IsCustomDateRange
    {
        get => _isCustomDateRange;
        set
        {
            if (_isCustomDateRange != value)
            {
                _isCustomDateRange = value;
                OnPropertyChanged();
            }
        }
    }

    private DateTime _fromDate = DateTime.Now.AddMonths(-1);
    public DateTime FromDate
    {
        get => _fromDate;
        set
        {
            if (_fromDate != value)
            {
                _fromDate = value;
                OnPropertyChanged();
            }
        }
    }

    private DateTime _toDate = DateTime.Now;
    public DateTime ToDate
    {
        get => _toDate;
        set
        {
            if (_toDate != value)
            {
                _toDate = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<TransactionModel> Transactions { get; set; } = new();

    public ICommand LoadStatementCommand { get; }
    public ICommand ExportPdfCommand { get; }
    public ICommand LoadAccountTypesCommand { get; }
    public ICommand LoadAccountsCommand { get; }

    private readonly int _customerId;

    public StatementViewModel(int customerId)
    {
        _customerId = customerId;

        LoadAccountsCommand = new Command(async () => await LoadAccountsAsync());
        LoadAccountTypesCommand = new Command(async () => await LoadAccountTypesAsync());
        LoadStatementCommand = new Command(async () => await LoadStatementAsync());
        ExportPdfCommand = new Command(async () => await ExportToPdfAsync());

        _ = LoadAccountTypesAsync();
    }


    

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public bool IsAccountSelected => SelectedAccount != null;
    public bool IsAccountTypeSelected => !string.IsNullOrEmpty(SelectedAccountType);
    public bool IsTimePeriodVisible => IsAccountSelected;
    public bool IsViewStatementVisible => IsAccountSelected && !string.IsNullOrEmpty(SelectedTimePeriod);

    public async Task LoadAccountTypesAsync()
    {
        try
        {
            IsLoading = true;
            var accountTypes = await DBHelper.GetAccountTypesAsync(_customerId);
            AccountTypes.Clear();
            foreach (var type in accountTypes)
                AccountTypes.Add(type);

            if (AccountTypes.Count > 0)
                SelectedAccountType = AccountTypes[0];
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load accounts: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAccountsAsync()
    {
        try
        {
            IsLoading = true;
            IsAccountListVisible = false;
            AvailableAccounts.Clear();

            if (string.IsNullOrWhiteSpace(SelectedAccountType))
            {
                await Shell.Current.DisplayAlert("Error", "Please select a valid account type.", "OK");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"SelectedAccountType: {SelectedAccountType}");

            var accounts = await DBHelper.GetCustomerAccountsAsync(_customerId, SelectedAccountType.Trim());
            System.Diagnostics.Debug.WriteLine($"Fetched Accounts: {accounts.Count}");

            foreach (var acc in accounts)
            {
                AvailableAccounts.Add(acc);
                System.Diagnostics.Debug.WriteLine($"Added Account: {acc.AccountNumber} - {acc.AccountName}");
            }

            SelectedAccount = AvailableAccounts.FirstOrDefault();
            IsAccountListVisible = AvailableAccounts.Count > 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load account details: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }




    private async Task LoadStatementAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedAccountType) || string.IsNullOrWhiteSpace(SelectedTimePeriod))
        {
            await Shell.Current.DisplayAlert("Validation", "Please select both Account Type and Time Period", "OK");
            return;
        }

        if (SelectedAccount == null)
        {
            await Shell.Current.DisplayAlert("Validation", "Please select an account.", "OK");
            return;
        }

        DateTime start = FromDate;
        DateTime end = ToDate;

        switch (SelectedTimePeriod)
        {
            case "Last Week":
                start = DateTime.Now.AddDays(-7);
                end = DateTime.Now;
                break;
            case "Last 1 Month":
                start = DateTime.Now.AddMonths(-1);
                end = DateTime.Now;
                break;
            case "Last 3 Months":
                start = DateTime.Now.AddMonths(-3);
                end = DateTime.Now;
                break;
            case "Last 1 Year":
                start = DateTime.Now.AddYears(-1);
                end = DateTime.Now;
                break;
        }

        if (end > DateTime.Now)
        {
            await Shell.Current.DisplayAlert("Validation", "To date cannot be in the future.", "OK");
            return;
        }

        Transactions.Clear();

        var fetchedData = await DBHelper.GetTransactionsAsync(_customerId, SelectedAccountType, start, end);
        foreach (var txn in fetchedData)
            Transactions.Add(txn);
    }

    public async Task ExportToPdfAsync()
    {
        try
        {
            var bytes = StatementPdfExporter.GeneratePdf(Transactions.ToList(), SelectedAccountType, FromDate, ToDate);

            var fileName = $"Statement_{SelectedAccountType}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllBytes(filePath, bytes);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
