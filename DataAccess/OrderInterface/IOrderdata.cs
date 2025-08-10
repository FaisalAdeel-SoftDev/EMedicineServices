using EMDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.OrderInterface
{
    public interface IOrderdata
    {
        public IEnumerable<Order> GetAllOrders();
        public Order GetOrder(int id);

        public int GetPendingOrderCount();

        public bool AddOrder(Order o);

        public bool DeleteOrder(Order o);

        public bool UpdateOrder(Order o);

        public List<Order> CheckOrder(int Customer_Id);
    }
}
