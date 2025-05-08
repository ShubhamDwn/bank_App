using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using bank_demo.Services;

namespace bank_demo.ViewModels.FeaturesPages.PaymentsVM
{
    public class ContactViewModel : BindableObject
    {
        public ObservableCollection<ContactModel> Contacts { get; set; }
        public ICommand ContactSelectedCommand { get; }
        public ICommand PayWithEnteredNumberCommand { get; }
        private string _enteredNumber;

        public string EnteredNumber
        {
            get => _enteredNumber;
            set
            {
                _enteredNumber = value;
                OnPropertyChanged();
            }
        }

        public ContactViewModel()
        {
            Contacts = new ObservableCollection<ContactModel>
            {
                new ContactModel { Name = "ATharv", PhoneNumber = "9999911111" },
                new ContactModel { Name = "Shubham", PhoneNumber = "9999922222" },
                new ContactModel { Name = "Ashish", PhoneNumber = "9999933333" },
            };

            ContactSelectedCommand = new Command<ContactModel>(async contact =>
            {
                if (contact is not null)
                {
                    await Shell.Current.GoToAsync($"SendMoneyPage?phone={contact.PhoneNumber}");
                }
            });

            PayWithEnteredNumberCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(EnteredNumber))
                {
                    await Shell.Current.GoToAsync($"SendMoneyPage?phone={EnteredNumber}");
                }
            });
        }
    }

    
}
