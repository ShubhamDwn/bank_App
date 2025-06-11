namespace bank_demo.Services.API
{
    public class ForgotPasswordRequest
    {
        public string Aadhaar { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
