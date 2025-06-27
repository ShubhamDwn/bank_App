using bank_demo.ViewModels;
using Microsoft.Maui.Controls;


namespace bank_demo.Pages.AuthenticationPage;

[QueryProperty(nameof(CustomerId), "CustomerId")]
public partial class LoginPage : ContentPage
{
    private int _customerId;

    public int CustomerId
    {
        get => _customerId;

        set
        {
            _customerId = value;
            //Application.Current.MainPage.DisplayAlert("Debug", $"Received CustomerId: {_customerId}", "OK");

            BindingContext = new LoginViewModel(_customerId); // Now ViewModel gets the ID
        }
    }

    public LoginPage()
    {
        InitializeComponent();

    }
    private bool _isPinVisible = false;

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        _isPinVisible = !_isPinVisible;
        PinEntry.IsPassword = !_isPinVisible;
        PasswordToggleButton.Source = _isPinVisible ? "eye_open.png" : "eye_icon.png";
    }


}
