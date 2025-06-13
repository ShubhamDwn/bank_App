using bank_demo.ViewModels.FeaturesPages;
using System;

namespace bank_demo.Pages;

[QueryProperty(nameof(CustomerId), "CustomerId")]
[QueryProperty(nameof(AccountNumber), "AccountNumber")]
[QueryProperty(nameof(PigmyAgentId), "PigmyAgentId")]
[QueryProperty(nameof(Start), "Start")]
[QueryProperty(nameof(End), "End")]
public partial class ViewStatementPage : ContentPage
{
    public int CustomerId { get; set; }
    public int subSchemeId { get; set; }
    public int AccountNumber { get; set; }
    public int PigmyAgentId { get; set; }
    public DateTime Start { get; set; } // passed as string from Shell
    public DateTime End { get; set; }

    public ViewStatementPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);


       // {
//string deviceId = DeviceInfo.Current.Idiom.ToString(); // Or your actual DeviceId logic

       //     var viewModel = new ViewStatementViewModel(CustomerId,subSchemeId, // if not passed, assign default or retrieve dynamicallyAccountNumber,PigmyAgentId);

          //  BindingContext = viewModel;

        //    await viewModel.LoadTransactionsAsync();
      //  }
       // else{await DisplayAlert("Error", "Invalid date format received.", "OK");}
    }
}
