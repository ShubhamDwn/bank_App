using bank_demo.ViewModels;
using Microsoft.Maui.Controls;

namespace bank_demo.Pages.AuthenticationPage;

public partial class SignupPage : ContentPage
{
	public SignupPage()
	{
		InitializeComponent();
        BindingContext = new SignupViewModel();
    }

    
}