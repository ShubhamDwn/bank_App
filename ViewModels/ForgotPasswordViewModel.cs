using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using bank_demo.Services;
using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;

namespace bank_demo.ViewModels
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private string _adharNumber;
        private string _newPassword;
        private string _confirmPassword;
        private bool _isOtpSent;

        public string AdharNumber
        {
            get => _adharNumber;
            set => SetProperty(ref _adharNumber, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsOtpSent
        {
            get => _isOtpSent;
            set => SetProperty(ref _isOtpSent, value);
        }

        public ICommand SendOtpCommand => new Command(async () => await SendOtpAsync());
        public ICommand ResetPasswordCommand => new Command(async () => await ResetPasswordAsync());

        private readonly OtpService _otpService = new OtpService();

        private string _mobile;

        private async Task SendOtpAsync()
        {
            if (string.IsNullOrWhiteSpace(AdharNumber))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Aadhar number is required.", "OK");
                return;
            }

            using var conn = await DBHelper.GetConnectionAsync();

            string checkAadhaarQuery = "SELECT EmailId, CellPhone FROM Customer WHERE AdharNumber = @AdharNumber";
            using var cmd = new SqlCommand(checkAadhaarQuery, conn);
            cmd.Parameters.AddWithValue("@AdharNumber", AdharNumber);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await Shell.Current.DisplayAlert("Not Found", "No account associated with this Aadhaar number.", "OK");
                return;
            }

            _mobile = reader["CellPhone"]?.ToString();

            // ✅ Fix: assign to property, not local variable
            IsOtpSent = await _otpService.SendAndVerifyOtpAsync(_mobile);
        }


        private async Task ResetPasswordAsync()
        {
            if (!IsOtpSent)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please verify OTP before resetting password.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Password fields cannot be empty.", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (NewPassword.Length < 8)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Password must be at least 8 characters.", "OK");
                return;
            }

            string hashedPassword = SecurityHelper.HashPassword(NewPassword);

            using var conn = await DBHelper.GetConnectionAsync();
            var updateCmd = new SqlCommand("UPDATE Customer SET UserPassword = @Password WHERE AdharNumber = @AdharNumber", conn);
            updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
            updateCmd.Parameters.AddWithValue("@AdharNumber", AdharNumber);

            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Password reset successful.", "OK");
                await Shell.Current.GoToAsync("///LoginPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update password.", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(backingStore, value)) return;
            backingStore = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
