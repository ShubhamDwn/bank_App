namespace bank_demo.Services.API
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class AccountModel
    {
        public int PrimaryId { get; set; }
        public int Id { get; set; }
        public int SubSchemeId { get; set; }
        public string SubSchemeName { get; set; }
        public int PigmyAgentId { get; set; }
        public string AccountNumber { get; set; }
        public string OldAccountNumber { get; set; }
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
        public string ODLoanAccountNo { get; set; }
        public bool? IsOverdueAccount { get; set; }
        public string DirectorName { get; set; }
        public decimal Balance { get; set; }
    }




    // Models/TransactionModel.cs
    public class TransactionModel
    {
        public int PrimaryId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int SubSchemeId { get; set; }
        public string AccountNumber { get; set; }  // Changed to string to avoid overflow
        public int ScrollNumber { get; set; }
        public string Narration { get; set; }
        public string TransactionType { get; set; }
        public decimal Deposite { get; set; }
        public decimal Withdraw { get; set; }
        public decimal Plain { get; set; }
        public decimal PlainCr { get; set; }
        public decimal PlainDr { get; set; }
        public decimal Penalty { get; set; }
        public decimal PenaltyCr { get; set; }
        public decimal PenaltyDr { get; set; }
        public decimal Payable { get; set; }
        public decimal Receivable { get; set; }
        public decimal Dividend { get; set; }
        public string DrCr { get; set; }
        public decimal Balance { get; set; }
    }

}
