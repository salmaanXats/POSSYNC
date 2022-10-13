using DataSyncer.Entities;
using DataSyncer.Helpers;
using DataSyncer.Interfaces;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataSyncer
{
    public class SyncService
    {
        private IApiService _apiService;
        private IHttpService _httpService;
        public SyncService()
        {
            _httpService = new HttpService();
            _apiService = new ApiService(_httpService);
        }

        public async Task<AuthenticationResponse> Login()
        {
            return await _apiService.Login();
        }

        public async Task SyncData(BillHeader request)
        {
            await _apiService.SyncData(request);
            await UpdateDatabase(request);
        }

        public async Task UpdateDatabase(BillHeader bill)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["prod"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(
                     @"UPDATE [dbo].[bill_header]
                       SET IsSynced = 1
                       WHERE bill_no = @0 and bill_date = @1", conn);

                command.Parameters.Add(new SqlParameter("0", bill.bill_no));
                command.Parameters.Add(new SqlParameter("1", bill.bill_date));

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Bill> GetRecentDataFromDatabase()
        {
            var bill = new Bill();

            var connectionString = ConfigurationManager.ConnectionStrings["prod"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(
                     @"SELECT hh.*,da.cMobile FROM [dbo].[bill_header] as hh
                      left join [dbo].[delivery_moreAddress] as da on hh.cCode = da.cCode and da.cMobile != ''
                      where hh.IsSynced = 0
					  order by hh.bill_date;", conn);


                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new BillHeader();
                        reader.MapDataToObject(newObject);
                        bill.BillHeader.Add(newObject);
                    }
                }

                if (bill.BillHeader.Count == 0)
                    return bill;

                var sqlQuery = @"select * from [dbo].[bill_tran]
                                    where bill_no in (@0) and bill_date in (@1)";

                var billNos = bill.BillHeader.Select(p => p.bill_no).ToList();
                var dates = bill.BillHeader.Select(p => p.bill_date).Distinct().ToList();

                var numbersString = string.Join(",", billNos.ToList());
                var dateString = string.Join(",", dates.Select(p=> String.Format("'{0}'", p)));

                sqlQuery = sqlQuery.Replace("@0", numbersString)
                                   .Replace("@1", dateString);

                SqlCommand command1 = new SqlCommand(sqlQuery, conn);

                using (SqlDataReader reader = command1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new BillTran();
                        reader.MapDataToObject(newObject);
                        bill.BillTran.Add(newObject);
                    }
                }
            }

            return bill;
        }
    }
}
