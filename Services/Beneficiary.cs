namespace bank_demo.Services
{
    public class Beneficiary
    {
        public required string BeneficiaryName { get; set; }
        public required string BankName { get; set; }
        public required string IFSCCode { get; set; }
        public required int CustomerId { get; set; }
        public required string BranchName { get; set; }
        public required string BeneficiaryNickName { get; set; }
        public int AccountNumber { get; set; }
    }

}
