using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bank_demo.ViewModels.FeaturesPages
{
    public class MyQRCodeViewModel : INotifyPropertyChanged
    {
        private string _upiId;

        public string UPIId
        {
            get => _upiId;
            set
            {
                if (_upiId != value)
                {
                    _upiId = value;
                    OnPropertyChanged();
                }
            }
        }

        public MyQRCodeViewModel()
        {
            // Set default UPI ID (can be changed based on user)
            UPIId = "ashish@bank";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
