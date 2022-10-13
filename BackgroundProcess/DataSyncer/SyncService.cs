using DataSyncer.Entities;
using DataSyncer.Helpers;
using DataSyncer.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
        }

        public async Task<Bill> GetRecentDataFromDatabase(string billDate, string billNo)
        {
            var bill = new Bill();

            var connectionString = ConfigurationManager.ConnectionStrings["prod"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(
                     @"SELECT hh.*,da.cMobile FROM [dbo].[bill_header] as hh
                      left join [dbo].[delivery_moreAddress] as da on hh.cCode = da.cCode and da.cMobile != ''
                      where hh.bill_date >  @0 and hh.bill_no > @1
					  order by hh.bill_date;", conn);

                command.Parameters.Add(new SqlParameter("0", billDate));
                command.Parameters.Add(new SqlParameter("1", Convert.ToInt32(billNo)));

                SqlCommand command1 = new SqlCommand(
                  @"SELECT * FROM [dbo].[bill_tran]
                      WHERE bill_date >  @0 and bill_no > @1;", conn);

                command1.Parameters.Add(new SqlParameter("0", billDate));
                command1.Parameters.Add(new SqlParameter("1", Convert.ToInt32(billNo)));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newObject = new BillHeader();
                        reader.MapDataToObject(newObject);
                        bill.BillHeader.Add(newObject);
                    }
                }

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
