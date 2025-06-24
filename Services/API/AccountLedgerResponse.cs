using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace bank_demo.Services.API
{
    public class CustomerAccountLedgerModel
    {
        public DateTime TransactionDate { get; set; }
        public string SubSchemeName { get; set; }

        public int PrimaryId { get; set; }
        public int Id { get; set; }
        public int SubSchemeId { get; set; }
        public int PigmyAgentId { get; set; }

        public string AccountNumber { get; set; }
        public string OldAccountNumber { get; set; }
        public int CustomerId { get; set; }

        public DateTime OpeningDate { get; set; }
        public DateTime AsOnDate { get; set; }
        public string ReceiptNo { get; set; }

        public int PeriodInDay { get; set; }
        public int PeriodInMonth { get; set; }
        public int PeriodInYear { get; set; }

        public DateTime? ExpiryDate { get; set; }  // Nullable in case it's not always set
        public int RateOfInterestId { get; set; }
        public decimal IAmount { get; set; }
        public decimal MaturityAmount { get; set; }
        public decimal Installment { get; set; }

        public bool Closed { get; set; }
        public DateTime? ClosedDate { get; set; }

        public bool IsApplyInterest { get; set; }
        public DateTime? LastInterestDate { get; set; }

        public string ODLoanAccountNo { get; set; }
        public bool IsOverdueAccount { get; set; }
        public string DirectorName { get; set; }

        public decimal Balance { get; set; }

        public string Name { get; set; }
        public string PermanentAddress { get; set; }
        public string CellPhone { get; set; }
        public string AreaName { get; set; }
    }

}
