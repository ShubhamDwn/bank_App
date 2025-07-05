using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank_demo.Services.API
{
    public class TransactionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TransactionId { get; set; }
    }
    public class NeftTransactionRequest
    {
        public int SubSchemeId { get; set; }
        public long AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
        public int BranchId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal NeftAmount { get; set; }
        public decimal NeftCharges { get; set; }
        public string BenificiaryRemark { get; set; } = "";
        public string BenificiaryIFSCCode { get; set; }
        public string BenificiaryAccountNumber { get; set; }
        public string BenificiaryAccountHolderName { get; set; }
        public string BenificiaryBranchName { get; set; }
        public string BenificiaryBankName { get; set; }
        public bool IsFileGenerate { get; set; }
        public string UserName { get; set; }
        public string BenificiaryMobileNo { get; set; }
        public string BenificiaryEmail { get; set; }
    }

    public class NeftTransactionModel
    {
        public DateTime TransactionDate { get; set; }
        public int ScrollNumber { get; set; }
        public int SubSchemeId { get; set; }
        public int BatchNumber { get; set; }
        public int AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public string BenificiaryAccountHolderName { get; set; }
        public decimal Amount { get; set; }
        public string BenificiaryCode { get; set; }
        public string BenificiaryAccountNumber { get; set; }
        public string BenificiaryBankName { get; set; }
        public string BenificiaryIFSCCode { get; set; }
        public string BenificiaryRemark { get; set; } = "";
        public string UniqueTransactionId { get; set; }
        public string UserName { get; set; }
        public int BranchId { get; set; }
        public int PaymentTypeId { get; set; }
    }
}
