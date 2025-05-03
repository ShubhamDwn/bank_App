namespace bank_demo.Models
{
    public class Beneficiary
    {
        public required string Name { get; set; }
        public required string BankName { get; set; }
        public required string IFSCCode { get; set; }
        public required int BeneficiaryAccountNumber { get; set; }
        public required string Branch { get; set; }
        public required string Nickname { get; set; }
    }
}
