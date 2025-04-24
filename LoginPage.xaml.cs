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
}
