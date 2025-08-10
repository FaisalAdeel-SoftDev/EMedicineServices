using EMDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UserInterface
{
    public interface IUserdata
    {
        public IEnumerable<User> GetAllUsers();
        public User GetUser(int id);
        public bool GetUserbyname(string name);

        public bool AddUser(User u);

        public bool DeleteUser(User u);

        public bool UpdateUser(User u);

        public User CheckUser(String name, String pass);
    }
}
