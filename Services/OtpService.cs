

namespace bank_demo.Services
{
    class OtpService
    {
        private string _generatedOTP;
        private readonly Random _random;

        public OtpService()
        {
            _random = new Random();
        }

        public async Task<bool> SendAndVerifyOtpAsync(string mobile)
        {
            // Generate 6-digit OTP
            _generatedOTP = _random.Next(1000, 9999).ToString();

            // Display OTP sent message (for testing, includes the OTP)
            await Shell.Current.DisplayAlert(
                "OTP Sent",
                $"OTP sent to Registered Mobile Number \n(For testing: {_generatedOTP})",
                "OK"
            );

            // Prompt user for OTP
            string enteredOTP = await Shell.Current.DisplayPromptAsync(
                "OTP Verification",
                "Enter the OTP:"
            );

            // Verify OTP
            if (enteredOTP != _generatedOTP)
            {
                await Shell.Current.DisplayAlert("Error", "Invalid OTP", "OK");
                return false;
            }
            else
            {
                await Shell.Current.DisplayAlert("OTP Verified", "Valid OTP", "OK");
                return true;
            }
        }
    }
}