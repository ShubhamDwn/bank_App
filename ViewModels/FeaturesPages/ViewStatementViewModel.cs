using bank_demo.Services;
using bank_demo.Services.API;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class ViewStatementViewModel : BaseViewModel
    {
        public ObservableCollection<TransactionModel> Transactions { get; set; } = new();

        public ICommand LoadStatementCommand { get; }
        public ICommand ExportPdfCommand { get; }

        private int _customerId;
        private int _subSchemeId;
        private int _accountNumber;
        private int _pigmyAgentId;

        // ✅ These are the actual bindable properties used in UI
        private DateTime _from;
        public DateTime FromDate
        {
            get => _from;
            set => SetProperty(ref _from, value);
        }

        private DateTime _to;
        public DateTime ToDate
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isStatementVisible;
        public bool IsStatementVisible
        {
            get => _isStatementVisible;
            set => SetProperty(ref _isStatementVisible, value);
        }

        public bool IsCustomDateRange => true;
        public bool IsViewStatementVisible => true;

        // ✅ Constructor now assigns dates to bindable properties FromDate and ToDate
        public ViewStatementViewModel(int customerId, int subSchemeId, int accountNumber, int pigmyAgentId, DateTime fromDate, DateTime toDate)
        {
            _customerId = customerId;
            _subSchemeId = subSchemeId;
            _accountNumber = accountNumber;
            _pigmyAgentId = pigmyAgentId;

            FromDate = fromDate;  // ✅ show correct value in DatePicker
            ToDate = toDate;

            LoadStatementCommand = new Command(async () => await LoadStatementAsync());
            ExportPdfCommand = new Command(ExportToPdf);

            // ✅ Load using these properties (you can test with log alert too)
            _ = LoadStatementAsync();
        }

        private async Task LoadStatementAsync()
        {
            try
            {
                IsLoading = true;
                Transactions.Clear();

                var data = await DBHelper.GetTransactionsAsync(
                    _customerId,
                    _subSchemeId,
                    _accountNumber,
                    _pigmyAgentId,
                    FromDate,     // ✅ using bound property, not private field
                    ToDate
                );

                foreach (var txn in data)
                    Transactions.Add(txn);

                if (Transactions.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Info", "No transactions found for selected period.", "OK");
                }

                IsStatementVisible = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load statement: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ExportToPdf()
        {
            try
            {
                var bytes = StatementPdfExporter.GenerateStatementPdf(Transactions.ToList());

                string fileName = $"Statement_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                File.WriteAllBytes(filePath, bytes);

                await Shell.Current.DisplayAlert("Success", $"PDF exported: {filePath}", "OK");

                // Optionally open the file:
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }

    }
}
