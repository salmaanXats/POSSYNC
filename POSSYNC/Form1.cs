using ClosedXML.Excel;
using POSSYNC.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace POSSYNC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (txtPath.Text == String.Empty)
            {
                MessageBox.Show("Select a path to generate the report", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var data = ReadHistoryDataFromDB();
            if (data.Count == 0)
            {
                MessageBox.Show("No records found", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var formattedData = FormatData(data);
            GenerateCSV(formattedData);
        }

        private void GenerateCSV(List<HistoryHeaderMapper> formattedData)
        {
            try
            {
                using (var wbook = new XLWorkbook())
                {
                    var ws = wbook.Worksheets.Add("Sheet1");

                    ws.Cell("H1").Value = "Invoice Date";
                    ws.Range("H1:M1").Merge();


                    ws.Cell("A2").Value = "Tenant ID";
                    ws.Cell("B2").Value = "POS ID";
                    ws.Cell("C2").Value = "Stall No";
                    ws.Cell("D2").Value = "Cashier ID";
                    ws.Cell("E2").Value = "Customer Mobile No";
                    ws.Cell("F2").Value = "InvoiceType";
                    ws.Cell("G2").Value = "Invoice No";
                    ws.Cell("H2").Value = "DD";
                    ws.Cell("I2").Value = "MM";
                    ws.Cell("J2").Value = "YY";
                    ws.Cell("K2").Value = "HH";
                    ws.Cell("L2").Value = "MM";
                    ws.Cell("M2").Value = "SS";
                    ws.Cell("N2").Value = "Currency Code";
                    ws.Cell("O2").Value = "Currency Rate";
                    ws.Cell("P2").Value = "Total Invoice";
                    ws.Cell("Q2").Value = "Total Tax";
                    ws.Cell("R2").Value = "Total Discount";
                    ws.Cell("S2").Value = "Total Gift Voucher Sale";
                    ws.Cell("T2").Value = "Total Gift Voucher Tax";
                    ws.Cell("U2").Value = "Total Gift Voucher Discount";
                    ws.Cell("V2").Value = "Paid By Cash";
                    ws.Cell("W2").Value = "Paid By Card";
                    ws.Cell("X2").Value = "Card Bank";
                    ws.Cell("Y2").Value = "Card Category";
                    ws.Cell("Z2").Value = "Card Type";
                    ws.Cell("AA2").Value = "Gift Voucher Burn";
                    ws.Cell("AB2").Value = "HCM Loyalty";
                    ws.Cell("AC2").Value = "Tenant Loyalty";
                    ws.Cell("AD2").Value = "Credit Note";
                    ws.Cell("AE2").Value = "Other Payments";

                    var index = 3;

                    foreach (var item in formattedData)
                    {
                        ws.Cell($"A{index}").Value = item.tenantId;
                        ws.Cell($"B{index}").Value = item.posId;
                        ws.Cell($"C{index}").Value = item.stallNo;
                        ws.Cell($"D{index}").Value = item.cashierId;
                        ws.Cell($"E{index}").Value = item.cMobile;
                        ws.Cell($"F{index}").Value = item.invoiceType;
                        ws.Cell($"G{index}").Value = item.bill_no;
                        ws.Cell($"H{index}").Value = item.billDate.DD;
                        ws.Cell($"I{index}").Value = item.billDate.MM;
                        ws.Cell($"J{index}").Value = item.billDate.YYYY;
                        ws.Cell($"K{index}").Value = item.billDate.HH;
                        ws.Cell($"L{index}").Value = item.billDate.Minutes;
                        ws.Cell($"M{index}").Value = item.billDate.SS;
                        ws.Cell($"N{index}").Value = item.currencyCode;
                        ws.Cell($"O{index}").Value = item.currencyRate;
                        ws.Cell($"P{index}").Value = item.bill_amt;
                        ws.Cell($"Q{index}").Value = item.tax;
                        ws.Cell($"R{index}").Value = item.Discount_amt;
                        ws.Cell($"S{index}").Value = item.totalGiftVoucherSale;
                        ws.Cell($"T{index}").Value = item.totalGiftVoucherTax;
                        ws.Cell($"U{index}").Value = item.totalGiftVoucherDiscount;
                        ws.Cell($"V{index}").Value = item.paidByCash;
                        ws.Cell($"W{index}").Value = item.paidByCard;
                        ws.Cell($"X{index}").Value = item.cardBank;
                        ws.Cell($"Y{index}").Value = item.cardCategory;
                        ws.Cell($"Z{index}").Value = item.cardType;
                        ws.Cell($"AA{index}").Value = item.giftVoucherBurn;
                        ws.Cell($"AB{index}").Value = item.hcmLoyalty;
                        ws.Cell($"AC{index}").Value = item.tenantLoyalty;
                        ws.Cell($"AD{index}").Value = item.creditNote;
                        ws.Cell($"AE{index}").Value = item.otherPayments;
                        index = index + 1;
                    }

                    var fileName = txtFileName.Text;
                    fileName = fileName == string.Empty ? "Report" : fileName;


                    wbook.SaveAs($"{txtPath.Text}/{fileName}.xlsx");
                    MessageBox.Show("Successfully Generated!");
                    txtFileName.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("it is being used by another process."))
                    MessageBox.Show("This file is already open.Close exsiting file and try again");
                return;
            }
        }

        private List<HistoryHeaderMapper> FormatData(List<HistoryHeader> data)
        {
            data.ForEach(p => p.Format());
            var formattedData = data.Select(p => new HistoryHeaderMapper()
            {
                tenantId = p.tenantId,
                posId = p.posId,
                stallNo = p.stallNo,
                cashierId = p.cashierId,
                cMobile = p.cMobile,
                invoiceType = p.invoiceType,
                bill_no = p.bill_no,
                billDate = p.billDate,
                currencyCode = p.currencyCode,
                currencyRate = p.currencyRate,
                bill_amt = p.bill_amt,
                tax = p.tax,
                Discount_amt = p.Discount_amt,
                totalGiftVoucherSale = p.totalGiftVoucherSale,
                paidByCash = p.paidByCash,
                paidByCard = p.paidByCard,
                cardBank = p.cardBank,
                cardCategory = p.cardCategory,
                cardType = p.cardType,
                giftVoucherBurn = p.giftVoucherBurn,
                hcmLoyalty = p.hcmLoyalty,
                tenantLoyalty = p.tenantLoyalty,
                creditNote = p.creditNote,
                otherPayments = p.otherPayments,
            }).ToList();

            return formattedData;
        }

        private List<HistoryHeader> ReadHistoryDataFromDB()
        {
            var startDate = dtpStartDate.Value.ToString("yyyy-MM-dd");
            var endDate = dtpEndDate.Value.ToString("yyyy-MM-dd");

            var data = new List<HistoryHeader>();
            var connectionString = ConfigurationManager.ConnectionStrings["prod"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(
                    @"SELECT hh.*,da.cMobile,ht.tran_code,ht.tran_desc FROM [dbo].[History_header] as hh
                      left join [dbo].[delivery_moreAddress] as da on hh.cCode = da.cCode and da.cMobile != ''
                      left join (SELECT distinct tran_code,tran_desc,bill_date,bill_no FROM [dbo].[History_tran]
                      where tran_type ='P') as ht on hh.bill_date = ht.bill_date and hh.bill_no = ht.bill_no
                      WHERE hh.bill_date between @0 and @1
                      order by hh.bill_date ,hh.bill_no;", conn);

                command.Parameters.Add(new SqlParameter("0", startDate));
                command.Parameters.Add(new SqlParameter("1", endDate));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new HistoryHeader();
                        reader.MapDataToObject(newObject);
                        data.Add(newObject);
                    }
                }
            }

            return data;
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            var dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
