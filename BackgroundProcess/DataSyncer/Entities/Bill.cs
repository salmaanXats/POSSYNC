using System.Collections.Generic;

namespace DataSyncer.Entities
{
    public class Bill
    {
        public List<BillHeader> BillHeader { get; set; } = new List<BillHeader>();
        public List<BillTran> BillTran { get; set; } = new List<BillTran>();
    }
}
