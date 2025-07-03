using bank_demo.ViewModels;
using Microsoft.Maui.Controls;


namespace bank_demo.Pages.AuthenticationPage;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();

    }
    private bool _isPinVisible = false;

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        _isPinVisible = !_isPinVisible;
        PinEntry.IsPassword = !_isPinVisible;
        PasswordToggleButton.Source = _isPinVisible ? "eye_open.png" : "eye_icon.png";
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is LoginViewModel vm)
        {
            // Call LoadCustomerName explicitly here
            vm.LoadCustomerNameCommand.Execute(null);
        }
    }

}
