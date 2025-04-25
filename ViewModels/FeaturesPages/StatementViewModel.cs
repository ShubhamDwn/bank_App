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

    public List<string> AccountTypes { get; } = new() { "Savings", "Loan", "FD" };
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

    public StatementViewModel()
    {
        LoadStatementCommand = new Command(async () => await LoadStatementAsync());
        LoadStatementCommand = new Command(async () => await LoadStatementAsync());
        ExportPdfCommand = new Command(async () => await ExportToPdfAsync());
    }

    private async Task LoadStatementAsync()
    {
        try
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
            var fetchedData = await GetMockDataAsync(SelectedAccountType, start, end);
            foreach (var txn in fetchedData)
                Transactions.Add(txn);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
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


    private Task<List<TransactionModel>> GetMockDataAsync(string type, DateTime from, DateTime to)
    {
        return Task.FromResult(new List<TransactionModel>
        {
            new() { Description = $"{type} Txn 1", Amount = 2500, Date = DateTime.Now.AddDays(-3) },
            new() { Description = $"{type} Txn 2", Amount = -1200, Date = DateTime.Now.AddDays(-1) }
        });
    }


}
