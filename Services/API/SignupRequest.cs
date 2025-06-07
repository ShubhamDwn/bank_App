namespace bank_demo.Services.API
{
    public class SignupRequest
    {
        public string Aadhaar { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class SignupResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
