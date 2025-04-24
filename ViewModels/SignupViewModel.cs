using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace bank_demo.ViewModels
{
    public class SignupViewModel : INotifyPropertyChanged
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public ICommand SignupCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public SignupViewModel()
        {
            SignupCommand = new Command(async () => await SignUpAsync());
            NavigateToLoginCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("//LoginPage"); // Absolute route ensures reliability
            });

        }

        private async Task SignUpAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "All fields are required!", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match!", "OK");
                return;
            }

            try
            {
                using var conn = await Services.DBHelper.GetConnectionAsync();

                // Check if username or email already exists
                var checkCmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT COUNT(*) FROM users WHERE username = @username OR email = @email", conn);
                checkCmd.Parameters.AddWithValue("@username", Username);
                checkCmd.Parameters.AddWithValue("@email", Email);

                var existing = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                if (existing > 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Username or Email already exists.", "OK");
                    return;
                }

                // Insert user into table
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "INSERT INTO users (username, email, password) VALUES (@username, @email, @password)", conn);

                cmd.Parameters.AddWithValue("@username", Username);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@password", Password); // You can hash this if needed

                await cmd.ExecuteNonQueryAsync();

                await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
