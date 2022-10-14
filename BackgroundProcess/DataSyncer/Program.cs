using DataSyncer.Entities;
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
            SyncService syncService = new SyncService();
            AuthResponse = await syncService.Login();

            var billdata = await syncService.GetRecentDataFromDatabase();

            foreach (var header in billdata.BillHeader)
            {
                header.BillTransactions = billdata.BillTran.Where(p => p.bill_no == header.bill_no && p.bill_date == header.bill_date).ToList();
                header.Format();
                await syncService.SyncData(header);
            }
        }
    }
}
