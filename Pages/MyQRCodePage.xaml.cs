using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages;

public partial class MyQRCodePage : ContentPage
{
	public MyQRCodePage()
	{
		InitializeComponent();
        BindingContext = new MyQRCodeViewModel();
    }
}