using bank_demo.ViewModels;
using Microsoft.Maui.Controls;


namespace bank_demo.Pages.AuthenticationPage;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel(); // ViewModel connection
    }
    private bool _isPasswordVisible = false;

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;

        PasswordEntry.IsPassword = !_isPasswordVisible;

        PasswordToggleButton.Source = _isPasswordVisible ? "eye_open.png" : "eye_icon.png";
    }

}
