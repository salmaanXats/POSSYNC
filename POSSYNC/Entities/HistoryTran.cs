using System;

namespace POSSYNC.Entities
{
    public class HistoryTran
    {
        public int bill_no { get; set; }
        public DateTime bill_date { get; set; }
        public string tran_type { get; set; }
        public string tran_code { get; set; }
        public string tran_desc { get; set; }
        public decimal tran_amt { get; set; }
    }
}
