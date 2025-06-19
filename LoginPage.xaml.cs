using Microsoft.Maui.Controls;
using bank_demo.ViewModels;

namespace bank_demo;

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
