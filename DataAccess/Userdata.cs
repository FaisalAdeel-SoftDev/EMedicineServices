using DataAccess.UserInterface;
using EMDModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Userdata : IUserdata
    {
        private readonly EMEDContext Context;
        public Userdata(EMEDContext _Context)
        {
            Context = _Context;
        }



        public bool AddUser(User u)
        {
            Context.Add(u);
            Context.SaveChanges();
            return true;
        }

        public bool DeleteUser(User u)
        {
            Context.Remove(u);
            Context.SaveChanges();
            return true;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return Context.Users.ToList().OrderByDescending(x => x.CreatedDate);
        }

        public User GetUser(int id)
        {
            var row = Context.Users.Find(id);
            return row;
        }



        public bool GetUserbyname(string name)
        {
            var row=Context.Users.Where(x => x.Name == name).FirstOrDefault();
            if (row != null)
                return true;
            else
                return false;
        }

        public bool UpdateUser(User u)
        {
            Context.Entry(u).State = EntityState.Modified;
            Context.SaveChanges();
            return true;
        }


        public User CheckUser(String name, String pass)
        {
            var row = Context.Users.FirstOrDefault(x => x.Name == name && x.Password == pass);
            return row;
        }
    }
}
