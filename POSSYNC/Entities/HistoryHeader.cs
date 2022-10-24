using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace POSSYNC.Entities
{
    public class HistoryHeader
    {
        public string tenantId { get; set; } = ConfigurationManager.AppSettings["tenantId"]; // "T-001";
        public string posId { get; set; } = ConfigurationManager.AppSettings["posId"]; //"EXT-K002-1";
        public int stallNo { get; set; } = Convert.ToInt32(ConfigurationManager.AppSettings["stallNo"]); // 32;
        public string cashierId { get; set; } = "CASH-01";
        public string cMobile { get; set; }
        public string invoiceType { get; set; } = "Sale";
        public int bill_no { get; set; }
        public InvoiceDate billDate { get; set; }

        public string currencyCode { get; set; } = "LKR";

        public decimal currencyRate { get; set; } = 1.0000m;
        public decimal bill_amt { get; set; }
        public decimal tax { get; set; }
        public decimal Discount_amt { get; set; }
        public decimal totalGiftVoucherSale { get; set; } = 0;
        public decimal totalGiftVoucherTax { get; set; } = 0;
        public decimal totalGiftVoucherDiscount { get; set; } = 0;
        public decimal paidByCash { get; set; } = 0;
        public decimal paidByCard { get; set; } = 0;
        public string cardBank { get; set; }
        public string cardCategory { get; set; }
        public string cardType { get; set; }
        public decimal giftVoucherBurn { get; set; } = 0;
        public decimal hcmLoyalty { get; set; } = 0;
        public decimal tenantLoyalty { get; set; } = 0;
        public decimal creditNote { get; set; } = 0;
        public decimal otherPayments { get; set; } = 0;
        public string bill_mode { get; set; }
        public string bill_valid { get; set; }


        public string bill_start_time { get; set; }
        public DateTime bill_date { get; set; }

        public List<HistoryTran> HistoryTransactions { get; set; }


        public HistoryHeader Format()
        {
            bill_date = bill_date.Add(TimeSpan.Parse(bill_start_time));
            billDate = new InvoiceDate()
            {
                DD = bill_date.Day.ToString(),
                MM = bill_date.Month.ToString(),
                YYYY = bill_date.Year.ToString(),
                HH = bill_date.Hour.ToString(),
                Minutes = bill_date.Minute.ToString(),
                SS = bill_date.Second.ToString(),
            };

            var discount = HistoryTransactions.Where(p => p.tran_type == "D").FirstOrDefault();
            var taxTrans = HistoryTransactions.Where(p => p.tran_type == "L").FirstOrDefault();
            var paidBills = HistoryTransactions.Where(p => p.tran_type == "P").FirstOrDefault();

            Discount_amt = discount == null ? 0.00m : discount.tran_amt * -1;
            tax = taxTrans == null ? 0 : taxTrans.tran_amt;

            if (bill_valid != "X" || bill_valid != "Y" || !bill_amt.ToString().StartsWith("-"))
            {
                if (paidBills == null)
                    paidByCash = 0;

                if (paidBills != null)
                {
                    if (paidBills.tran_desc == null)
                        paidBills.tran_desc = "cash";

                    if (paidBills.tran_desc.Trim().ToLower() == "cash")
                    {
                        paidByCash = bill_amt;
                    }
                    else
                    {
                        if (paidBills.tran_desc.Trim().ToLower() == "visa")
                        {
                            cardType = "VISA";
                            cardCategory = "DEBIT";
                            paidByCard = bill_amt;
                            cardBank = "VISA1";
                        }
                        else if (paidBills.tran_desc.Trim().ToLower() == "master")
                        {
                            cardType = "MASTER";
                            cardCategory = "DEBIT";
                            paidByCard = bill_amt;
                            cardBank = "Mast1";

                        }
                        else if (paidBills.tran_desc.Trim().ToLower().Contains("american exp"))
                        {
                            cardType = "AMEX";
                            cardCategory = "DEBIT";
                            paidByCard = bill_amt;
                            cardBank = "Amex1";
                        }
                        else
                        {
                            cardType = "Other";
                            otherPayments = bill_amt;
                        }
                    }
                }
            }

            if (bill_valid == "X" || bill_valid == "Y" || bill_amt.ToString().StartsWith("-"))
                paidByCash = 0;

            if (bill_mode == "CO")
            {
                paidByCash = 0;
                Discount_amt = bill_amt;
            }

            return this;
        }
    }
    public class HistoryHeaderMapper
    {
        public string tenantId { get; set; } = "T-001";
        public string posId { get; set; } = "EXT-K002-1";
        public int stallNo { get; set; } = 32;
        public string cashierId { get; set; } = "CASH-01";
        public string cMobile { get; set; }
        public string invoiceType { get; set; } = "Sale";
        public int bill_no { get; set; }
        public InvoiceDate billDate { get; set; }

        public string currencyCode { get; set; } = "LKR";

        public decimal currencyRate { get; set; } = 1.0000m;
        public decimal bill_amt { get; set; }
        public decimal tax { get; set; }
        public decimal Discount_amt { get; set; }
        public decimal totalGiftVoucherSale { get; set; } = 0;
        public decimal totalGiftVoucherTax { get; set; } = 0;
        public decimal totalGiftVoucherDiscount { get; set; } = 0;
        public decimal paidByCash { get; set; } = 0;
        public decimal paidByCard { get; set; } = 0;
        public string cardBank { get; set; }
        public string cardCategory { get; set; } = "DEBIT";
        public string cardType { get; set; }
        public decimal giftVoucherBurn { get; set; } = 0;
        public decimal hcmLoyalty { get; set; } = 0;
        public decimal tenantLoyalty { get; set; } = 0;
        public decimal creditNote { get; set; } = 0;
        public decimal otherPayments { get; set; } = 0;
    }

    public class InvoiceDate
    {
        public string DD { get; set; }
        public string MM { get; set; }
        public string YYYY { get; set; }
        public string HH { get; set; }
        public string Minutes { get; set; }
        public string SS { get; set; }
    }
}
