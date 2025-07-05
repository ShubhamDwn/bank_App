namespace bank_demo.Services.API
{
    public class Beneficiary
    {
        public string BeneficiaryName { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string ConfirmAccountNumber { get; set; }
        public string BranchName { get; set; }
        public string BeneficiaryNickName { get; set; }
        public long CustomerId { get; set; }
        public string BeneficiaryCode { get; set; }


        // Optional filters
        public bool IsRegister { get; set; }
        public bool Status { get; set; }
    }

    public class AddBeneficiaryRequest
    {
        public long CustomerId { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryNickName { get; set; }
        public string AccountNumber { get; set; }
        public string ConfirmAccountNumber { get; set; }
        public string IFSC { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string DeviceId { get; set; }
        public string RegFrom { get; set; }
    }

    public class AddBeneficiaryModel
    {
        public string DeviceId { get; set; }
        public int CustomerId { get; set; }
        public string BeneficiaryCode { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryNickName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string RegFrom { get; set; }
    }

    public class BeneficiaryDeleteRequest
    {
        public long CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
    }

    public class BeneficiaryDetailResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class BeneficiaryStatusResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class IFSCBankInfo
    {
        public string BANK { get; set; }
        public string BRANCH { get; set; }
        public string ADDRESS { get; set; }
        public string CITY { get; set; }
        public string DISTRICT { get; set; }
        public string STATE { get; set; }
        public bool NEFT { get; set; }
        public bool IMPS { get; set; }
        public bool RTGS { get; set; }
        public bool UPI { get; set; }
        public string SWIFT { get; set; }
    }
}
