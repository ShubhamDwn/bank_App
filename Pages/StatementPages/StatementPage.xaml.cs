using bank_demo.Services;
using bank_demo.ViewModels.FeaturesPages;
using System.Globalization;

namespace bank_demo.Pages.StatementPages;

[QueryProperty(nameof(CustomerId), "CustomerId")]
public partial class StatementPage : ContentPage
{
    private bool _isInitialized = false;

    private int _customerId;
    public int CustomerId
    {
        get => _customerId;
        set
        {
            _customerId = value;
            Console.WriteLine($"[DEBUG] CustomerId set via QueryProperty: {_customerId}");

            // Now initialize ViewModel only when CustomerId is set
            if (_customerId > 0 && !_isInitialized)
            {
                var vm = new StatementViewModel(_customerId);
                BindingContext = vm;
                _ = vm.LoadAccountTypesAsync(); // Fire and forget
                _isInitialized = true;
            }
        }
    }

    public StatementPage()
    {
        InitializeComponent();
    }
}
