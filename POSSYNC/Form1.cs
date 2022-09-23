using CsvHelper;
using POSSYNC.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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
                var fileName = txtFileName.Text;

                fileName = fileName == string.Empty ? "Report" : fileName;
                using (var writer = new StreamWriter($"{txtPath.Text}/{fileName}.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(formattedData);
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
