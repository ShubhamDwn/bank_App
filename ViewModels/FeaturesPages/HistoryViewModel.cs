// HistoryViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using bank_demo.ViewModels;


public class HistoryViewModel : BaseViewModel
{
    public ObservableCollection<string> HistoryItems { get; set; }
    public ICommand LoadHistoryCommand { get; }

    public HistoryViewModel()
    {
        HistoryItems = new ObservableCollection<string>();
        LoadHistoryCommand = new Command(LoadHistory);

        LoadHistory();
    }

    private void LoadHistory()
    {
        HistoryItems.Clear();
        HistoryItems.Add("Login - 02/04/2025 09:15 AM");
        HistoryItems.Add("Password Changed - 01/04/2025 03:42 PM");
        HistoryItems.Add("Logged Out - 01/04/2025 04:00 PM");
    }
}

