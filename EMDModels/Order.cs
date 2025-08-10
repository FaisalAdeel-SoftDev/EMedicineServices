using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMDModels
{
    public class Order
    {
        public int Id { get; set; }
        public int? Customer_Id { get; set; }
        public DateTime? Ordered_date { get; set; }
        public string? Order_status { get; set; }
        public string? Order_deliver_address { get; set; }

        public int? Order_Amount { get; set; }

        public int? Order_Qty { get; set; }

        public int? Med_Id { get; set; }
       
    }
}
