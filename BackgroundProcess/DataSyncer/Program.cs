using DataSyncer.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSyncer
{
    public class Program
    {
        public static AuthenticationResponse AuthResponse { get; set; }
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var path = "C:/lastUpdated.txt";
            string readText = File.ReadAllText(path);
            var lines = readText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var bill_no = lines[1];
            var bill_date = lines[0];

            SyncService syncService = new SyncService();
            AuthResponse = await syncService.Login();

            var billdata = await syncService.GetRecentDataFromDatabase(bill_date, bill_no);

            foreach (var header in billdata.BillHeader)
            {
                header.BillTransactions = billdata.BillTran.Where(p => p.bill_no == header.bill_no && p.bill_date == header.bill_date).ToList();

                header.Format();
                await syncService.SyncData(header);
            }

            if (billdata.BillHeader.Count > 0)
            {
                var lastUpdatedRecord = billdata.BillHeader.Last();

                File.WriteAllText(path, string.Empty);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(lastUpdatedRecord.bill_date.ToString("yyyy-MM-dd"));
                    writer.WriteLine(lastUpdatedRecord.bill_no);
                }
            }
        }
    }
}
