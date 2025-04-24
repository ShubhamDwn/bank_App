using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class PaymentsViewModel
    {
        public ICommand UPICommand { get; }
        public ICommand NEFTCommand { get; }
        public ICommand IMPSCommand { get; }

        public PaymentsViewModel()
        {
            UPICommand = new Command(() =>
            {
                // Add UPI navigation or logic here
            });

            NEFTCommand = new Command(() =>
            {
                // Add NEFT navigation or logic here
            });

            IMPSCommand = new Command(() =>
            {
                // Add IMPS navigation or logic here
            });
        }
    }
}
