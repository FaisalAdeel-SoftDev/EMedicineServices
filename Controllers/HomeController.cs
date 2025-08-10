using EMDModels;
using EMedicineServices.Models;
using MedDataService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace EMedicineServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public MedicineRepositaryServices Data;

        public HomeController(ILogger<HomeController> logger, MedicineRepositaryServices data)
        {
            _logger = logger;
            Data = data;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 6,string query="")
        {
            List<Medicine> list = new List<Medicine>();
            if (query != null)
            {
                 list = Data.GetmedicinebyName(query).ToList();
            }
            else
            {
                 list = Data.getallmedicines().ToList();
            }
            int itemsToSkip = (pageNumber - 1) * pageSize;

            // Skip the specified number of items
            var pagedData = list.Skip(itemsToSkip)
                                 // Take the specified number of items
                                 .Take(pageSize)
                                 .ToList();

            // Calculate total number of items
            int totalItems = list.Count();

            // Calculate total number of pages
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Pass paged data and pagination information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.HasPrevious = pageNumber > 1;
            ViewBag.HasNext = pageNumber < totalPages;




            return View(pagedData);
            
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
