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

    public ObservableCollection<TransactionModel> Transactions { get; } = new();

    private int _customerId;
    private int _accountNumber;
    private int _subSchemeId;
    private int _pigmyAgentId;
    private string _deviceId;
    private DateTime _fromDate;
    private DateTime _toDate;

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

    public ViewStatementViewModel(int CustomerId, int subSchemeId, int accountNumber, int pigmyAgentId, DateTime start, DateTime end)
    {
        _customerId = CustomerId;
        _subSchemeId = subSchemeId;
        _accountNumber = accountNumber;
        _pigmyAgentId= pigmyAgentId;
        _fromDate=start;
        _toDate=end;

        LoadTransactionsCommand = new Command(async () => await LoadTransactionsAsync());
        ExportPdfCommand = new Command(async () => await ExportPdfAsync());
    }


    public async Task LoadTransactionsAsync()
    {

        try
        {
            IsLoading = true;

            var data = await DBHelper.GetTransactionsAsync(
                _customerId,
                _subSchemeId,
                _accountNumber,
                _pigmyAgentId,
                _fromDate,
                _toDate
            );

            foreach (var txn in data)
                Transactions.Add(txn);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Unable to load statement: " + ex.Message, "OK");
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
            //var bytes = StatementPdfExporter.GeneratePdf(Transactions.ToList(), _accountNumber, CalculateStartDate(), CalculateEndDate());

            var fileName = $"Statement_{_accountNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            //File.WriteAllBytes(filePath, bytes);

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
