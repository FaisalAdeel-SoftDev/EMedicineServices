using DataAccess;
using EMDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedDataService
{
    public class UserRepositaryService
    {
        private readonly Userdata userdata;

        public UserRepositaryService(Userdata userdata)
        {
            this.userdata = userdata;
        }

        public bool AddUser(User u)
        {
            u.CreatedDate = DateTime.Now;
            u.Roletype = "User";
            userdata.AddUser(u);
            return true;

        }

        public User GetUser(int id)
        {
            var row = userdata.GetUser(id);
            return row;

        }

        public bool GetUserbyName(string name)
        {
             
            return userdata.GetUserbyname(name);

        }



        public bool UpdateUser(User u)
        {
            userdata.UpdateUser(u);
            return true;
        }

        public bool DeleteUser(int id)
        {
            var row = userdata.GetUser(id);
            userdata.DeleteUser(row);
            return true;
        }

        public IEnumerable<User> Getallusers()
        {
            return userdata.GetAllUsers();

        }

        public User AuthUser(String name, string pass)
        {
            var row = userdata.CheckUser(name, pass);

            if (row != null)
            {
                return row;
            }
            else
            {
                return null;
            }

        }
    }
}
