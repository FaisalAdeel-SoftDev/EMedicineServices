using EMDModels;

using MedDataService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMedicineServices.Controllers
{
   
    public class MedController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        private readonly MedicineRepositaryServices dataservice;
        private readonly IWebHostEnvironment _hostEnvironment;
       
        public MedController(ILogger<HomeController> logger, MedicineRepositaryServices _Context, IWebHostEnvironment hostEnvironment
            )
        {
            _logger = logger;
            dataservice = _Context;
            _hostEnvironment = hostEnvironment;
          
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("name")!=null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                return View();
            }
            else
            { 
                return RedirectToAction("login","Login");
            }
        }



        [HttpPost]
        public ActionResult Create(Medicine m)
        {

            dataservice.Insertmedicine(m);

            return RedirectToAction("Listall");
        }



        public IActionResult Listall()
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {

                return View(dataservice.getallmedicines());
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

        }


        public IActionResult delete(int id)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                dataservice.Deletemedicine(id);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            return RedirectToAction("Listall");
        }




        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                var row = dataservice.Getmedicine(id);
                return View(row);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
            

        }








        [HttpPost]
        public IActionResult Edit(Medicine m)
        {
            //if (m.filename != null)
            //{
            //    var relativepath = m.ImagePath.ToString();
            //    if (string.IsNullOrEmpty(relativepath))
            //    {
            //        return BadRequest("Path is null or empty.");
            //    }

            //    // Convert the relative path to an absolute path
            //    var filePath = _hostEnvironment.WebRootPath + relativepath.Replace("~/", "/");

            //    // Check if the file exists
            //    if (System.IO.File.Exists(filePath))
            //    {
            //        // Delete the file
            //        System.IO.File.Delete(filePath);
            //        // return Ok("File deleted successfully.");
            //    }
            //    else
            //    {
            //        return NotFound("File not found.");
            //    }



            //    if (m.filename != null && m.filename.Length > 0)
            //    {
            //        var name = Path.GetFileNameWithoutExtension(m.filename.FileName);
            //        var ext = Path.GetExtension(m.filename.FileName);
            //        var filename = name + Guid.NewGuid().ToString() + ext;

            //        var filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", filename);

            //        using (var fileStream = new FileStream(filePath1, FileMode.Create))
            //        {
            //            m.filename.CopyTo(fileStream);
            //        }

            //        var relativePath = $"~/Images/{filename}";
            //        m.ImagePath = relativePath.ToString();
            //        Context.Entry(m).State = EntityState.Modified;
            //        Context.SaveChanges();
            //    }


            //}
            //else
            //{
            //    Context.Entry(m).State = EntityState.Modified;
            //    Context.SaveChanges();
            //}

            dataservice.UpdateMedicine(m);

            return RedirectToAction("Listall");

        }


        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                var row = dataservice.Getmedicine(id);
                return View(row);
            }
            else
            {
                return RedirectToAction("login", "Login");
            }

           
        }



    }
}
