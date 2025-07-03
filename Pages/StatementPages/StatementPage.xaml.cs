using bank_demo.Services;
using bank_demo.ViewModels.FeaturesPages;
using System.Globalization;

namespace bank_demo.Pages.StatementPages;

public partial class StatementPage : ContentPage
{
    private bool _isInitialized = false;

    public StatementPage()
    {
        InitializeComponent();
        var vm = new StatementViewModel();
        BindingContext = vm;
        _ = vm.LoadAccountTypesAsync(); // Fire and forget
        _isInitialized = true;
}
}
