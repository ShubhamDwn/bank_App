using Microsoft.Maui.Controls;
using bank_demo.ViewModels;

namespace bank_demo.Pages.AuthenticationPage;

public partial class SignupPage : ContentPage
{
	public SignupPage()
	{
		InitializeComponent();
        BindingContext = new SignupViewModel();
    }

    
}