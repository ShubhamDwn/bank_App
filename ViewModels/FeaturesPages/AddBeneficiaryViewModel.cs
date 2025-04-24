using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class AddBeneficiaryViewModel : BaseViewModel
    {
        public ICommand AddBeneficiaryCommand { get; }

        public AddBeneficiaryViewModel()
        {
            AddBeneficiaryCommand = new Command(OnAddBeneficiary);
        }

        private async void OnAddBeneficiary()
        {
            // Placeholder logic for adding beneficiary
            await Shell.Current.DisplayAlert("Success", "Beneficiary added successfully!", "OK");

            // Optionally navigate back
            await Shell.Current.GoToAsync("..");
        }
    }
}
