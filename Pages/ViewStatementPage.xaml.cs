using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages;

[QueryProperty(nameof(CustomerId), "CustomerId")]
[QueryProperty(nameof(SubSchemeId), "SubSchemeId")]
[QueryProperty(nameof(AccountNumber), "AccountNumber")]
[QueryProperty(nameof(PigmyAgentId), "PigmyAgentId")]
[QueryProperty(nameof(Start), "Start")]
[QueryProperty(nameof(End), "End")]

public partial class ViewStatementPage : ContentPage
{
    private int _customerId;
    private int _subSchemeId;
    private int _accountNumber;
    private int _pigmyAgentId;
    private string _start;
    private string _end;

    private bool _customerSet, _subSchemeSet, _accountSet, _agentSet, _startSet, _endSet;

    public ViewStatementPage()
    {
        InitializeComponent();
    }

    public int CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            _customerSet = true;
            TryInitializeViewModel();
        }
    }

    public int SubSchemeId
    {
        get => _subSchemeId;
        set
        {
            _subSchemeId = value;
            _subSchemeSet = true;
            TryInitializeViewModel();
        }
    }

    public int AccountNumber
    {
        get => _accountNumber;
        set
        {
            _accountNumber = value;
            _accountSet = true;
            TryInitializeViewModel();
        }
    }

    public int PigmyAgentId
    {
        get => _pigmyAgentId;
        set
        {
            _pigmyAgentId = value;
            _agentSet = true;
            TryInitializeViewModel();
        }
    }

    public string Start
    {
        get => _start;
        set
        {
            _start = value;
            _startSet = true;
            TryInitializeViewModel();
        }
    }

    public string End
    {
        get => _end;
        set
        {
            _end = value;
            _endSet = true;
            TryInitializeViewModel();
        }
    }

    private void TryInitializeViewModel()
    {
        if (_customerSet && _subSchemeSet && _accountSet && _agentSet && _startSet && _endSet)
        {
            if (DateTime.TryParse(_start, out var fromDate) && DateTime.TryParse(_end, out var toDate))
            {
                BindingContext = new ViewStatementViewModel(_customerId, _subSchemeId, _accountNumber, _pigmyAgentId, fromDate, toDate);
                Shell.Current.DisplayAlert("Success", "ViewModel Loaded", "OK");
            }
            else
            {
                Shell.Current.DisplayAlert("Error", "Invalid date format", "OK");
            }
        }
    }
}
