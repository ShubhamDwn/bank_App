namespace bank_demo.Models
{
    public class Beneficiary
    {
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public int BeneficiaryAccountNumber { get; set; }
        public string Branch { get; set; }
        public string Nickname { get; set; }
    }
}
