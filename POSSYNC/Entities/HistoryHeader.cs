using CsvHelper.Configuration.Attributes;
using System;
using System.Configuration;

namespace POSSYNC.Entities
{
    public class HistoryHeader
    {
        [Name("Tenant ID")]
        public string tenantId { get; set; } = ConfigurationManager.AppSettings["tenantId"]; // "T-001";
        [Name("POS ID")]
        public string posId { get; set; } = ConfigurationManager.AppSettings["posId"]; //"EXT-K002-1";
        [Name("Stall No")]
        public int stallNo { get; set; } = Convert.ToInt32(ConfigurationManager.AppSettings["stallNo"]); // 32;
        [Name("Cashier ID")]
        public string cashierId { get; set; } = "CASH-01";
        [Name("Customer Mobile No")]
        public string cMobile { get; set; }
        [Name("InvoiceType")]
        public string invoiceType { get; set; } = "Sale";
        [Name("Invoice No")]
        public int bill_no { get; set; }
        [Name("Invoice Date")]
        public InvoiceDate billDate { get; set; }

        [Name("Currency Code")]
        public string currencyCode { get; set; } = "LKR";

        [Name("Currency Rate")]
        public decimal currencyRate { get; set; } = 1.0000m;
        [Name("Total Invoice")]
        public decimal bill_amt { get; set; }
        [Name("Total Tax")]
        public decimal tax { get; set; }
        [Name("Total Discount")]
        public decimal Discount_amt { get; set; }
        [Name("Total Gift Voucher Sale")]
        public decimal totalGiftVoucherSale { get; set; } = 0;
        [Name("Total Gift Voucher Tax")]
        public decimal totalGiftVoucherTax { get; set; } = 0;
        [Name("Total Gift Voucher Discount")]
        public decimal totalGiftVoucherDiscount { get; set; } = 0;
        [Name("Paid By Cash")]
        public decimal paidByCash { get; set; } = 0;
        [Name("Paid By Card")]
        public decimal paidByCard { get; set; } = 0;
        [Name("Card Bank")]
        public string cardBank { get; set; }
        [Name("Card Category")]
        public string cardCategory { get; set; }
        [Name("Card Type")]
        public string cardType { get; set; }
        [Name("Gift Voucher Burn")]
        public decimal giftVoucherBurn { get; set; } = 0;
        [Name("HCM Loyalty")]
        public decimal hcmLoyalty { get; set; } = 0;
        [Name("Tenant Loyalty")]
        public decimal tenantLoyalty { get; set; } = 0;
        [Name("Credit Note")]
        public decimal creditNote { get; set; } = 0;
        [Name("Other Payments")]
        public decimal otherPayments { get; set; } = 0;


        public string bill_start_time { get; set; }
        public string tran_code { get; set; }
        public string tran_desc { get; set; }
        public DateTime bill_date { get; set; }


        public HistoryHeader Format()
        {
            bill_date = bill_date.Add(TimeSpan.Parse(bill_start_time));
            billDate = new InvoiceDate()
            {
                DD = bill_date.Day,
                MM = bill_date.Month,
                YYYY = bill_date.Year,
                HH = bill_date.Hour,
                Minutes = bill_date.Minute,
                SS = bill_date.Second,
            };

            Discount_amt = Discount_amt * -1;

            if (tran_desc == null)
                tran_desc = "cash";
            
            if(tran_desc.Trim().ToLower() == "cash")
            {
                paidByCash = bill_amt;
            }
            else
            {
                if(tran_desc.Trim().ToLower() == "visa") 
                {
                    cardType = "VISA";
                    cardCategory = "DEBIT";
                    paidByCard = bill_amt;
                }
                else if (tran_desc.Trim().ToLower() == "master")
                {
                    cardType = "MASTER";
                    cardCategory = "DEBIT";
                    paidByCard = bill_amt;
                }
                else if(tran_desc.Trim().ToLower().Contains("american exp"))
                {
                    cardType = "AMEX";
                    cardCategory = "DEBIT";
                    paidByCard = bill_amt;
                }
                else
                {
                    cardType = "Other";
                    otherPayments = bill_amt;
                }
            }
            return this;
        }
    }
    public class HistoryHeaderMapper
    {
        [Name("Tenant ID")]
        public string tenantId { get; set; } = "T-001";
        [Name("POS ID")]
        public string posId { get; set; } = "EXT-K002-1";
        [Name("Stall No")]
        public int stallNo { get; set; } = 32;
        [Name("Cashier ID")]
        public string cashierId { get; set; } = "CASH-01";
        [Name("Customer Mobile No")]
        public string cMobile { get; set; }
        [Name("InvoiceType")]
        public string invoiceType { get; set; } = "Sale";
        [Name("Invoice No")]
        public int bill_no { get; set; }
        [Name("Invoice Date")]
        public InvoiceDate billDate { get; set; }

        [Name("Currency Code")]
        public string currencyCode { get; set; } = "LKR";

        [Name("Currency Rate")]
        public decimal currencyRate { get; set; } = 1.0000m;
        [Name("Total Invoice")]
        public decimal bill_amt { get; set; }
        [Name("Total Tax")]
        public decimal tax { get; set; }
        [Name("Total Discount")]
        public decimal Discount_amt { get; set; }
        [Name("Total Gift Voucher Sale")]
        public decimal totalGiftVoucherSale { get; set; } = 0;
        [Name("Total Gift Voucher Tax")]
        public decimal totalGiftVoucherTax { get; set; } = 0;
        [Name("Total Gift Voucher Discount")]
        public decimal totalGiftVoucherDiscount { get; set; } = 0;
        [Name("Paid By Cash")]
        public decimal paidByCash { get; set; } = 0;
        [Name("Paid By Card")]
        public decimal paidByCard { get; set; } = 0;
        [Name("Card Bank")]
        public string cardBank { get; set; }
        [Name("Card Category")]
        public string cardCategory { get; set; } = "DEBIT";
        [Name("Card Type")]
        public string cardType { get; set; }
        [Name("Gift Voucher Burn")]
        public decimal giftVoucherBurn { get; set; } = 0;
        [Name("HCM Loyalty")]
        public decimal hcmLoyalty { get; set; } = 0;
        [Name("Tenant Loyalty")]
        public decimal tenantLoyalty { get; set; } = 0;
        [Name("Credit Note")]
        public decimal creditNote { get; set; } = 0;
        [Name("Other Payments")]
        public decimal otherPayments { get; set; } = 0;
    }

    public class InvoiceDate
    {
        public int DD { get; set; }
        public int MM { get; set; }
        public int YYYY { get; set; }
        public int HH { get; set; }
        [Name("MM")]
        public int Minutes { get; set; }
        public int SS { get; set; }
    }
}
