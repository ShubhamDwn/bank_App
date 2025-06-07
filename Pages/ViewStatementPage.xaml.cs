using bank_demo.ViewModels;
using Microsoft.Maui.Controls;

namespace bank_demo.Pages;

public partial class ViewStatementPage : ContentPage
{
    public ViewStatementPage()
    {
       // InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

      //  if (BindingContext is ViewStatementViewModel vm)
       // {
       //     vm.LoadTransactionsCommand.Execute(null);
       // }
    }
}
