using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Services;
using Microsoft.Maui.Storage;
using bank_demo.Services.API;

namespace bank_demo.ViewModels.FeaturesPages;

public class ViewStatementViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    private string _accountNumber;
    private DateTime _fromDate;
    private DateTime _toDate;
    private string _timePeriod;

    public ObservableCollection<TransactionModel> Transactions { get; } = new();

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand LoadTransactionsCommand { get; }
    public ICommand ExportPdfCommand { get; }

    public ViewStatementViewModel()
    {
        LoadTransactionsCommand = new Command(async () => await LoadTransactionsAsync());
        ExportPdfCommand = new Command(async () => await ExportPdfAsync());
    }

    // Called when page is navigated to
    public void Initialize(string accountNumber, DateTime fromDate, DateTime toDate, string timePeriod)
    {
        _accountNumber = accountNumber;
        _fromDate = fromDate;
        _toDate = toDate;
        _timePeriod = timePeriod;
    }

    private DateTime CalculateStartDate()
    {
        if (_timePeriod == "Custom")
            return _fromDate;

        return _timePeriod switch
        {
            "Last Week" => DateTime.Now.AddDays(-7),
            "Last 1 Month" => DateTime.Now.AddMonths(-1),
            "Last 3 Months" => DateTime.Now.AddMonths(-3),
            "Last 1 Year" => DateTime.Now.AddYears(-1),
            _ => DateTime.Now.AddMonths(-1),
        };
    }

    private DateTime CalculateEndDate()
    {
        if (_timePeriod == "Custom")
            return _toDate;

        return DateTime.Now;
    }

    public async Task LoadTransactionsAsync()
    {
        if (string.IsNullOrWhiteSpace(_accountNumber))
            return;

        IsLoading = true;
        Transactions.Clear();

        DateTime start = CalculateStartDate();
        DateTime end = CalculateEndDate();

        var customerId = await SecureStorage.Default.GetAsync("CustomerId");
        if (!int.TryParse(customerId, out int custId))
            custId = 0; // or handle appropriately

        try
        {
            //var transactions = await DBHelper.GetTransactionsByAccountAsync(custId, _accountNumber, start, end);

            //foreach (var txn in transactions)
              //  Transactions.Add(txn);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load transactions: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ExportPdfAsync()
    {
        try
        {
            var bytes = StatementPdfExporter.GeneratePdf(Transactions.ToList(), _accountNumber, CalculateStartDate(), CalculateEndDate());

            var fileName = $"Statement_{_accountNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
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
