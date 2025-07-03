namespace bank_demo.Services.API
{

    
    public class SignupRequest
    {
        public int Id { get; set; }
        public string Pin { get; set; }         // 4-digit PIN
        public string DeviceId { get; set; }    // GUID from device
        public bool ForceOverride { get; set; } = false;
    }
    public class SignupResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string DeviceGuid { get; set; }
    }
}
