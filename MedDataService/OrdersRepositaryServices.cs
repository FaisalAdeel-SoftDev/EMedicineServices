using DataAccess;
using EMDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedDataService
{
    public class OrdersRepositaryServices
    {
        private readonly Orderdata data;

        public OrdersRepositaryServices(Orderdata data)
        {
            this.data = data;
        }



        public bool AddOrder(Order o)
        {
            data.AddOrder(o);
            return true;
        }

        public List<Order> CheckOrder(int Customer_Id)
        {
            return data.CheckOrder(Customer_Id);
        }

        public int GetPendingOrderCount()
        {
            return data.GetPendingOrderCount();
        }

        public bool DeleteOrder(Order o)
        {
            data.DeleteOrder(o);
            return true;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return data.GetAllOrders();
        }

        public Order GetOrder(int id)
        {
            return data.GetOrder(id);
        }

        public bool UpdateOrder(Order o)
        {
            data.UpdateOrder(o);
            return true;
        }

        public List<Order> GetAllCartItems(int userid)
        {
            var list=data.GetAllOrders();
            List<Order> Pendingorders = new List<Order>();
            foreach (var o in list)
            {
                if (o.Order_status == "InCart" && o.Customer_Id==userid)
                { 
                    Pendingorders.Add(o);
                }
            }
            return Pendingorders;
        }

    }
}
