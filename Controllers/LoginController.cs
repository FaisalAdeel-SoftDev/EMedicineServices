using EMedicineServices.Models;
using MedDataService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;


namespace EMedicineServices.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepositaryService urs;
        private readonly OrdersRepositaryServices ord;

        public LoginController(UserRepositaryService Urs,OrdersRepositaryServices ord )
        {
            urs = Urs;
            this.ord = ord;
        }
        public IActionResult Index()
        {
            
            
            return View();
        }

        public IActionResult login()
        {


            return View();
        }
        [HttpPost]
        public IActionResult login(string name,string password)
        {
            password = GetSHA256Hash(password);
            var row=urs.AuthUser(name, password);
            if (row != null)
            {
                if (row.Roletype == "Admin")
                {
                    HttpContext.Session.SetString("name", row.Roletype);
                    HttpContext.Session.SetInt32 ("Userid", row.Id);
                    HttpContext.Session.SetString("username", row.Name);
                    int Counts = ord.GetPendingOrderCount();
                    HttpContext.Session.SetInt32("count",Counts );
                    return RedirectToAction("Adminpanel", "Login");
                }
                else
                {
                    HttpContext.Session.SetString("name", row.Roletype);
                    HttpContext.Session.SetInt32("Userid", row.Id);
                    HttpContext.Session.SetString("username", row.Name);
                    return RedirectToAction("Index", "Home");
                }
            }
            {
                return RedirectToAction("login");

            }
           
                
        }


        public IActionResult logout()
        {

            HttpContext.Session.Remove("name");
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("Userid");
            return RedirectToAction("login", "Login");
        }

        public IActionResult Adminpanel()
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("login");
            }
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
