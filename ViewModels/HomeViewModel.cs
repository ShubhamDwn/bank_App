using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using bank_demo.Pages;
using bank_demo.Services.API;

namespace bank_demo.ViewModels 
{
    public class HomeViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _customerName;
        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); }
        }

        private decimal _savingsBalance;
        public decimal SavingsBalance
        {
            get => _savingsBalance;
            set { _savingsBalance = value; OnPropertyChanged(); }
        }
        private bool _isMenuVisible;
        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set
            {
                if (_isMenuVisible != value)
                {
                    _isMenuVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Advertisement> Advertisements { get; } = new()
        {
            new Advertisement { ImageUrl = "ad1.jpg" },
            new Advertisement { ImageUrl = "ad2.jpg" },
            new Advertisement { ImageUrl = "ad3.jpg" }
        };


        private bool _isCarouselRunning = true;

        [Obsolete]
        public void StartCarousel()
        {
            if (_isCarouselRunning) return;

            _isCarouselRunning = true;

            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                if (!_isCarouselRunning || Advertisements.Count == 0)
                    return false;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectedIndex = (SelectedIndex + 1) % Advertisements.Count;
                });

                return true;
            });
        }

        public void StopCarousel()
        {
            _isCarouselRunning = false;
        }








        public ICommand AboutCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand QRCommand { get; }
        public ICommand ScanToPayCommand { get; }
        public ICommand StatementCommand { get; }
        public ICommand CustomerLedgerCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand AddBeneficiaryCommand { get; }
        public ICommand PaymentsCommand { get; }
        public ICommand FundTransferCommand { get; }


        public ICommand ProfileCommand { get; }
        public ICommand TransactionHistoryCommand { get; }
        public ICommand BeneficiaryStatusCommand { get; }
        public ICommand ContactSupportCommand { get; }
        public ICommand SecuritySettingsCommand { get; }
        public ICommand TermsCommand { get; }
        public ICommand LogoutCommand { get; }




#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public HomeViewModel(int customerId)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            LoadCustomerData(customerId);


            //bottom Navigation commands
            AboutCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("About");
            });

            HomeCommand = new Command(async () =>
            {
                await Shell.Current.DisplayAlert("Home", "Already on Home page", "OK");
            });

            SettingsCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync($"Settings?CustomerId={customerId}");
            });


            // Commands for main page
            QRCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(MyQRCodePage));
            });

            ScanToPayCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ScanToPayPage));
            });

            StatementCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync($"StatementPage?CustomerId={customerId}");
            });

            CustomerLedgerCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync($"CustomerLedgerPage?CustomerId={customerId}");
            });
            
            HistoryCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(HistoryPage));
            });

            AddBeneficiaryCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync($"BeneficiaryStatusPage?CustomerId={customerId}");
            });

            PaymentsCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(PaymentsPage));
            });

            FundTransferCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync($"FundTransferPage?account_number={customerId}");
            });


            // Commands for the menu drawer
            ProfileCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ProfilePage));
            });

            TransactionHistoryCommand = new Command(async () => { 
                await Shell.Current.GoToAsync(nameof(HistoryPage)); 
            });

            BeneficiaryStatusCommand = new Command(async () => {
                await Shell.Current.GoToAsync($"BeneficiaryStatusPage?CustomerId={customerId}");
            });

            ContactSupportCommand = new Command(async () => { 
                await Shell.Current.GoToAsync(nameof(ContactSupportPage)); 
            });

            //SecuritySettingsCommand = new Command(async () => { 
            //    await Shell.Current.GoToAsync(nameof(SecuritySettingsPage)); 
            //});

            TermsCommand = new Command(async () => { 
                await Shell.Current.GoToAsync(nameof(TermsPage)); 
            });

            LogoutCommand = new Command(async () => {
                Preferences.Set("IsLoggedIn", false);
                await AppShell.RecheckDeviceAsync();
            });
        }

        private async void LoadCustomerData(int customerId)
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{BaseURL.Url()}api/home/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = System.Text.Json.JsonSerializer.Deserialize<HomeResponse>(json, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        CustomerName = data.CustomerName;
                        SavingsBalance = data.SavingsBalance;
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load customer data from API", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"API error: {ex.Message}", "OK");
            }
        }

    }

}

