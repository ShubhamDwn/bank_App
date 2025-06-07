

namespace bank_demo.Services
{
    public class AccountModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpeningDate { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        // Add any other properties that your SP returns
    }

}
