namespace bank_demo.Services.API
{
    public class LoginRequest
    {
        public string DeviceId { get; set; }
        public string Pin { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int CustomerId { get; set; }
    }
    public class DeviceCheckRequest
    {
        public string DeviceId { get; set; }
    }

    public class DeviceCheckResponse
    {
        public bool Success { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Message { get; set; }
    }
    public class LogoutAllRequest
    {
        public int CustomerId { get; set; }
    }
    public class LogoutAllResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
