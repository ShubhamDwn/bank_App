using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using bank_demo.ViewModels;


namespace bank_demo.ViewModels.FeaturesPages
{
    public class StatementViewModel : BaseViewModel
    {
        public ObservableCollection<string> Transactions { get; set; }
        public ICommand LoadStatementCommand { get; }

        public StatementViewModel()
        {
            Transactions = new ObservableCollection<string>();
            LoadStatementCommand = new Command(LoadStatements);

            LoadStatements(); // Load by default on viewmodel init
        }

        private void LoadStatements()
        {
            Transactions.Clear();
            Transactions.Add("Debit - ₹1,200 - 02/04/2025");
            Transactions.Add("Credit - ₹5,000 - 01/04/2025");
            Transactions.Add("UPI - ₹599 - 29/03/2025");
        }
    }
}