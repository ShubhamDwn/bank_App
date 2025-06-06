using System.Collections.ObjectModel;
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

    // Make AccountTypes ObservableCollection to update UI dynamically
    public ObservableCollection<string> AccountTypes { get; } = new();

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
                // You can optionally load transactions automatically on selection here
                // _ = LoadStatementAsync();
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

    private readonly int _customerId; // ✅ use only this

    public StatementViewModel(int customerId)
    {
        _customerId = customerId; // ✅ set this
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



    private async Task LoadStatementAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedAccountType) || string.IsNullOrWhiteSpace(SelectedTimePeriod))
        {
            await Shell.Current.DisplayAlert("Validation", "Please select both Account Type and Time Period", "OK");
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

        // Replace this with actual call to your transactions retrieval based on SelectedAccountType, date range, etc.
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
