namespace bank_demo.Services.API
{
    public class AccountModel
    {
        public int PrimaryId { get; set; }
        public int Id { get; set; }
        public int SubSchemeId { get; set; }
        public int PigmyAgentId { get; set; }
        public string AccountNumber { get; set; }
        public string? OldAccountNumber { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? RateOfInterestId { get; set; }
        public decimal? IAmount { get; set; }
        public decimal? MaturityAmount { get; set; }
        public decimal? Installment { get; set; }
        public bool Closed { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool IsApplyInterest { get; set; }
        public string? ODLoanAccountNo { get; set; }
        public bool? IsOverdueAccount { get; set; }
        public string? DirectorName { get; set; }
        public decimal Balance { get; set; }
    }
    public class TransactionModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
}
