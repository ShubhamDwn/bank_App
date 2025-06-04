using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using bank_demo.Services;
using System.Text;

namespace bank_demo.ViewModels
{
    public class SignupViewModel : BaseViewModel
    {
        private string _aadhaar;
        public string Aadhaar
        {
            get => _aadhaar;
            set => SetProperty(ref _aadhaar, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand SignupCommand { get; }

        private readonly OtpService _otpService;

        public SignupViewModel()
        {
            SignupCommand = new Command(async () => await ExecuteSignup());
            _otpService = new OtpService();
        }

        private async Task ExecuteSignup()
        {
            // 1. Validate Fields
            if (string.IsNullOrWhiteSpace(Aadhaar) || string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "All fields are required", "OK");
                return;
            }

            if (!Regex.IsMatch(Aadhaar, @"^\d{12}$"))
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid 12-digit Aadhaar number", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            if (Password.Length < 8)
            {
                await Shell.Current.DisplayAlert("Error", "Password must be at least 8 characters", "OK");
                return;
            }

            string hashedPassword = SecurityHelper.HashPassword(Password);

            using var conn = await DBHelper.GetConnectionAsync();

            // 2. Check Aadhaar exists
            string checkAadhaarQuery = "SELECT EmailId, CellPhone, UserName, UserPassword FROM Customer WHERE AdharNumber = @AdharNumber";
            using var cmd = new SqlCommand(checkAadhaarQuery, conn);
            cmd.Parameters.AddWithValue("@AdharNumber", Aadhaar);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await Shell.Current.DisplayAlert("Not Found", "No account associated with this Aadhaar number.", "OK");
                return;
            }

            string email = "", mobile = "", existingUsername = null, existingUserPassword = null;
            if (await reader.ReadAsync())
            {
                email = reader["EmailId"]?.ToString();
                mobile = reader["CellPhone"]?.ToString();
                existingUsername = reader["UserName"]?.ToString();
                existingUserPassword = reader["UserPassword"]?.ToString();
            }
            reader.Close();

            // 3. Check if username is already used by someone else
            string checkUsernameQuery = "SELECT COUNT(*) FROM Customer WHERE UserName = @UserName";
            using var usernameCmd = new SqlCommand(checkUsernameQuery, conn);
            usernameCmd.Parameters.AddWithValue("@UserName", Username);
            int count = (int)await usernameCmd.ExecuteScalarAsync();

            if (count > 0)
            {
                await Shell.Current.DisplayAlert("Username Exists", "Username already exists. Please choose another.", "OK");
                return;
            }

            // 4. If already signed up
            if (!string.IsNullOrWhiteSpace(existingUsername) && !string.IsNullOrWhiteSpace(existingUserPassword))
            {
                await Shell.Current.DisplayAlert("Account Exists", "Account already exists. Please login.", "OK");
                await Shell.Current.GoToAsync("///LoginPage");
                return;
            }

            // 5. Verify OTP
            if (!await _otpService.SendAndVerifyOtpAsync(mobile))
            {
                await Shell.Current.DisplayAlert("Failed", "OTP verification failed.", "OK");
                return;
            }

            // 6. Update Username and Password
            string updateQuery = "UPDATE Customer SET UserName = @UserName, UserPassword = @UserPassword WHERE AdharNumber = @AdharNumber";
            using var updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@UserName", Username);
            updateCmd.Parameters.AddWithValue("@UserPassword", hashedPassword);
            updateCmd.Parameters.AddWithValue("@AdharNumber", Aadhaar);

            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await Shell.Current.DisplayAlert("Success", "Signup successful. You can now login.", "OK");
                await Shell.Current.GoToAsync("///LoginPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Signup failed. Try again.", "OK");
            }
        }
    }
}
