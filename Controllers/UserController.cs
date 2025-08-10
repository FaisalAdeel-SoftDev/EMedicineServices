using EMDModels;
using MedDataService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace EMEDICINE.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepositaryService user;

        public UserController(UserRepositaryService user)
        {
            this.user = user;
        }



        public IActionResult Index()
        {
            return View();
        }




        public IActionResult Register()
        {
            
                return View();

        }

        [HttpPost]
        public IActionResult Register(User u)
        {
            string hashedString = GetSHA256Hash(u.Password);

            u.Password = hashedString;

            if (!user.GetUserbyName(u.Name))
            {
                user.AddUser(u);
            }
            else
            {
                ViewBag.error = "<Script>alert('User name already registered')</script>";
                return View(u);
            }
              
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                return RedirectToAction("listall");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }



        public IActionResult Listall()
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                return View(user.Getallusers());
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                if (HttpContext.Session.GetString("name").ToString() == "Admin")
                {
                    var row = user.GetUser(id);
                    return View(row);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        [HttpPost]
        public IActionResult Edit(User u)
        {
           user.UpdateUser(u);
            return RedirectToAction("Listall");
        }



        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                if (HttpContext.Session.GetString("name").ToString() == "Admin")
                {
                    user.DeleteUser(id);
                    return RedirectToAction("Listall");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {

                if (HttpContext.Session.GetString("name").ToString() == "Admin")
                {
                    var row = user.GetUser(id);
                    return View(row);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }






        public IActionResult User_Edit(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                    var row = user.GetUser(id);
                    return View(row);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }


        [HttpPost]
        public IActionResult User_Edit(User u)
        {
            user.UpdateUser(u);
            return RedirectToAction("Index","Home");
        }




        public IActionResult User_Pass_Update(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                var row = user.GetUser(id);
                return View(row);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }


        [HttpPost]
        public IActionResult User_Pass_Update(User u)
        {
            u.Password = GetSHA256Hash(u.Password);
            user.UpdateUser(u);
            return RedirectToAction("Index", "Home");
        }











        public static string GetSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert byte to hex string
                }
                return builder.ToString();
            }
        }















    }
}
