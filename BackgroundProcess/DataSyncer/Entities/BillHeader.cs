using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DataSyncer.Entities
{
    public class BillHeader
    {
        public string tenantId { get; set; } = ConfigurationManager.AppSettings["tenantId"];
        public string posId { get; set; } = ConfigurationManager.AppSettings["posId"];
        public int stallNo { get; set; } = Convert.ToInt32(ConfigurationManager.AppSettings["stallNo"]);
        public string cashierId { get; set; } = "CASH-01";
        public string customerMobileNo { get; set; }
        public string invoiceType { get; set; } = "Sale";
        public string invoiceNo { get; set; }
        public DateTime invoiceDate { get; set; }
        public string currencyCode { get; set; } = "LKR";
        public decimal currencyRate { get; set; } = 1.0000m;
        public decimal totalInvoice { get; set; }
        public decimal totalTax { get; set; }
        public decimal totalDiscount { get; set; }
        public decimal totalGiftVoucherSale { get; set; } = 0;
        public decimal totalGiftVoucherDiscount { get; set; } = 0;
        public decimal paidByCash { get; set; } = 0;
        public decimal paidByCard { get; set; } = 0;
        public string cardBank { get; set; }
        public string cardCategory { get; set; }
        public string cardType { get; set; }
        public decimal giftVoucherBurn { get; set; } = 0;
        public decimal hcmLoyalty { get; set; } = 0;
        public decimal tenantLoyalty { get; set; } = 0;
        public decimal creditNotes { get; set; } = 0;
        public decimal otherPayments { get; set; } = 0;


        public string bill_start_time { get; set; }
        public int bill_no { get; set; }
        public DateTime bill_date { get; set; }
        public decimal bill_amt { get; set; }
        public decimal tax { get; set; }
        public decimal Discount_amt { get; set; }
        public string cMobile { get; set; }
        public string bill_mode { get; set; }
        public string bill_valid { get; set; }

        public List<BillTran> BillTransactions { get; set; }



        public BillHeader Format()
        {
            this.invoiceDate = bill_date.Add(TimeSpan.Parse(bill_start_time));
            this.invoiceNo = bill_no.ToString();
            this.customerMobileNo = cMobile;
            this.totalInvoice = bill_amt;


            var discount = BillTransactions.Where(p => p.tran_type == "D").FirstOrDefault();
            var taxTrans = BillTransactions.Where(p => p.tran_type == "L").FirstOrDefault();
            var paidBills = BillTransactions.Where(p => p.tran_type == "P").FirstOrDefault();

            this.totalDiscount = discount == null ? 0.00m : discount.tran_amt * -1;
            totalTax = taxTrans == null ? 0 : taxTrans.tran_amt;

            if (bill_valid != "X" || bill_valid != "Y" || !bill_valid.StartsWith("-"))
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
                        }
                        else if (paidBills.tran_desc.Trim().ToLower() == "master")
                        {
                            cardType = "MASTER";
                            cardCategory = "DEBIT";
                            paidByCard = bill_amt;
                        }
                        else if (paidBills.tran_desc.Trim().ToLower().Contains("american exp"))
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
                }
            }

            if (bill_valid == "X" || bill_valid == "Y" || bill_valid.StartsWith("-"))
                paidByCash = 0;

            if (bill_mode == "CO")
                paidByCash = 0;
            return this;
        }
    }
}
