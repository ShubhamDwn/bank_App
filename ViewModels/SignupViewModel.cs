using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace bank_demo.ViewModels
{
    public class SignupViewModel : INotifyPropertyChanged
    {
        private string _username, _fullName, _accountNumber, _mobileNumber, _aadhaarNumber, _email, _password, _confirmPassword;
        private string _emailOtp, _enteredEmailOtp, _mobileOtp, _enteredMobileOtp;
        private bool _isEmailVerified, _isMobileVerified;

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }
        public string AccountNumber { get => _accountNumber; set { _accountNumber = value; OnPropertyChanged(); } }
        public string MobileNumber { get => _mobileNumber; set { _mobileNumber = value; OnPropertyChanged(); } }
        public string AadhaarNumber { get => _aadhaarNumber; set { _aadhaarNumber = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        public string EmailOtp { get => _emailOtp; set { _emailOtp = value; OnPropertyChanged(); } }
        public string EnteredEmailOtp { get => _enteredEmailOtp; set { _enteredEmailOtp = value; OnPropertyChanged(); } }
        public bool IsEmailVerified { get => _isEmailVerified; set { _isEmailVerified = value; OnPropertyChanged(); } }

        public string MobileOtp { get => _mobileOtp; set { _mobileOtp = value; OnPropertyChanged(); } }
        public string EnteredMobileOtp { get => _enteredMobileOtp; set { _enteredMobileOtp = value; OnPropertyChanged(); } }
        public bool IsMobileVerified { get => _isMobileVerified; set { _isMobileVerified = value; OnPropertyChanged(); } }

        public ICommand SignupCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand SendMobileOtpCommand { get; }
        public ICommand VerifyMobileOtpCommand { get; }
        public ICommand SendEmailOtpCommand { get; }
        public ICommand VerifyEmailOtpCommand { get; }

        public SignupViewModel()
        {
            SendMobileOtpCommand = new Command(async () => await ShowMobileOtpPopupAsync());
            VerifyMobileOtpCommand = new Command(async () => await VerifyMobileOtpAsync());
            SendEmailOtpCommand = new Command(async () => await ShowEmailOtpPopupAsync());
            VerifyEmailOtpCommand = new Command(async () => await VerifyEmailOtpAsync());
            SignupCommand = new Command(async () => await SignUpAsync());
            NavigateToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }

        private async Task ShowMobileOtpPopupAsync()
        {
            if (!Regex.IsMatch(MobileNumber ?? string.Empty, "^\\d{10}$"))
            {
                await ShowAlert("Enter a valid 10-digit mobile number before requesting OTP.");
                return;
            }
            MobileOtp = GenerateOtp();
            Console.WriteLine($"Generated Mobile OTP: {MobileOtp}");
            string result = await Application.Current.MainPage.DisplayPromptAsync("OTP Sent", $"Enter OTP sent to {MobileNumber}", "Verify", "Cancel");
            if (!string.IsNullOrEmpty(result))
            {
                EnteredMobileOtp = result;
                await VerifyMobileOtpAsync();
            }
        }

        private async Task ShowEmailOtpPopupAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
            {
                await ShowAlert("Enter a valid email address before requesting OTP.");
                return;
            }
            EmailOtp = GenerateOtp();
            Console.WriteLine($"Generated Email OTP: {EmailOtp}");
            string result = await Application.Current.MainPage.DisplayPromptAsync("OTP Sent", $"Enter OTP sent to {Email}", "Verify", "Cancel");
            if (!string.IsNullOrEmpty(result))
            {
                EnteredEmailOtp = result;
                await VerifyEmailOtpAsync();
            }
        }

        private async Task VerifyMobileOtpAsync()
        {
            if (EnteredMobileOtp == MobileOtp)
            {
                IsMobileVerified = true;
                await Application.Current.MainPage.DisplayAlert("Verified", "Mobile number verified!", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Incorrect OTP. Please try again.", "OK");
            }
        }

        private async Task VerifyEmailOtpAsync()
        {
            if (EnteredEmailOtp == EmailOtp)
            {
                IsEmailVerified = true;
                await Application.Current.MainPage.DisplayAlert("Verified", "Email verified!", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Incorrect OTP. Please try again.", "OK");
            }
        }

        private async Task SignUpAsync()
        {
            if (!ValidateFields()) return;

            try
            {
                using var conn = await Services.DBHelper.GetConnectionAsync();

                // Check if Username already exists
                var checkUsernameCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", conn);
                checkUsernameCmd.Parameters.AddWithValue("@username", Username);
                var existingUsername = Convert.ToInt32(await checkUsernameCmd.ExecuteScalarAsync());

                if (existingUsername > 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Username already exists.", "OK");
                    return;
                }

                // Check if Email already exists
                var checkEmailCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email = @email", conn);
                checkEmailCmd.Parameters.AddWithValue("@email", Email);
                var existingEmail = Convert.ToInt32(await checkEmailCmd.ExecuteScalarAsync());

                if (existingEmail > 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Email already exists.", "OK");
                    return;
                }

                // Check if Account Number already exists
                var checkAccountNumberCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE account_number = @accountNumber", conn);
                checkAccountNumberCmd.Parameters.AddWithValue("@accountNumber", AccountNumber);
                var existingAccountNumber = Convert.ToInt32(await checkAccountNumberCmd.ExecuteScalarAsync());

                if (existingAccountNumber > 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Account number already exists.", "OK");
                    return;
                }

                // If no issues, proceed to the next steps


                var cmd = new MySqlCommand("INSERT INTO users (username, full_name, account_number, mobile_number, aadhaar_number, email, password) " +
                                           "VALUES (@username, @fullName, @accountNumber, @mobileNumber, @aadhaarNumber, @email, @password)", conn);

                cmd.Parameters.AddWithValue("@username", Username);
                cmd.Parameters.AddWithValue("@fullName", FullName);
                cmd.Parameters.AddWithValue("@accountNumber", AccountNumber);
                cmd.Parameters.AddWithValue("@mobileNumber", MobileNumber);
                cmd.Parameters.AddWithValue("@aadhaarNumber", AadhaarNumber);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@password", HashPassword(Password));

                await cmd.ExecuteNonQueryAsync();

                await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Sign-up failed: {ex.Message}", "OK");
            }
        }

        private string GenerateOtp() => new Random().Next(100000, 999999).ToString();

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(AccountNumber) ||
                string.IsNullOrWhiteSpace(MobileNumber) || string.IsNullOrWhiteSpace(AadhaarNumber) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ShowAlert("All fields are required."); return false;
            }

            if (!Regex.IsMatch(MobileNumber, "^\\d{10}$")) { ShowAlert("Mobile number must be 10 digits."); return false; }
            if (!Email.Contains("@") || !Email.Contains(".")) { ShowAlert("Invalid email format."); return false; }
            if (Username.Contains(" ")) { ShowAlert("Username must not contain spaces."); return false; }
            if (!Regex.IsMatch(Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W_]).{8,}$"))
            { ShowAlert("Password must be at least 8 characters, include upper & lowercase, digit, and symbol."); return false; }
            if (Password != ConfirmPassword) { ShowAlert("Passwords do not match."); return false; }
            if (!IsMobileVerified) { ShowAlert("Mobile number must be verified."); return false; }
            if (!IsEmailVerified) { ShowAlert("Email must be verified."); return false; }

            return true;
        }

        private async 
        Task
ShowAlert(string message) =>
            await Application.Current.MainPage.DisplayAlert("Validation Error", message, "OK");

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
