using ZXing.Net.Maui;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

using ZXing.Net.Maui.Controls;
using ZXing;
using bank_demo.ViewModels.FeaturesPages;

namespace bank_demo.Pages
{
    public partial class ScanToPayPage : ContentPage
    {

        public ScanToPayPage()
        {
            InitializeComponent();

        }

        /*private void OnBarcodeDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            var vm = BindingContext as ScanToPayViewModel;

            // Ensure Results is not null and has at least one scanned value
            if (e.Results != null && e.Results.Count() > 0 && vm != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    cameraView.IsDetecting = false;
                    vm.HandleScanned(e.Results.First().Value);
                });
            }
        }*/


    }
}