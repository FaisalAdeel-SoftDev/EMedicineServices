using DataAccess.OrderInterface;
using EMDModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Orderdata : IOrderdata
    {
        private readonly EMEDContext Context;

        public Orderdata(EMEDContext _Context)
        {
            Context = _Context;
        }
        public bool AddOrder(Order o)
        {
            Context.Orders.Add(o);
            Context.SaveChanges();
            return true;
        }

        public List<Order> CheckOrder(int Customer_Id)
        {
            return Context.Orders.Where(x => x.Customer_Id == Customer_Id).ToList();
        }

        public bool DeleteOrder(Order o)
        {
            Context.Remove(o);
            Context.SaveChanges();
            return true;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return Context.Orders.ToList().OrderByDescending(x => x.Ordered_date);
        }

        public Order GetOrder(int id)
        {
            return Context.Orders.Find(id);
        }


        public int GetPendingOrderCount()
        {
            return Context.Orders.Count(x => x.Order_status == "Pending");
        }

        public bool UpdateOrder(Order o)
        {
            Context.Entry(o).State = EntityState.Modified;
            Context.SaveChanges();
            return true;
        }
    }
}
