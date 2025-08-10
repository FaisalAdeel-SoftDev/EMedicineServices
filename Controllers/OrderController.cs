using EMDModels;
using EMedicineServices.Models;
using MedDataService;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Humanizer;
using System.Security.Policy;
using NuGet.Protocol.Plugins;
using System.Numerics;
using System;
using System.IO;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;
using System.Collections;
using System.Globalization;
using log4net;
using OpenHtmlToPdf;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Routing;
using static System.Net.Mime.MediaTypeNames;




namespace EMedicineServices.Controllers
{
    public class OrderController : Controller
    {
        private readonly MedicineRepositaryServices medservice;
        private readonly OrdersRepositaryServices ordservice;
        private readonly UserRepositaryService usrservice;


        private readonly IServiceProvider _serviceProvider;

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConverter _converter;
        private readonly IUrlHelperFactory urlHelperFactory;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(OrderController));
        public OrderController(MedicineRepositaryServices Medservice, OrdersRepositaryServices Ordservice,
                               UserRepositaryService Usrservice, IServiceProvider serviceProvider,
                               IWebHostEnvironment hostingEnvironment, IConverter converter, IUrlHelperFactory urlHelperFactory)
        {
            medservice = Medservice;
            ordservice = Ordservice;
            usrservice = Usrservice;

            _serviceProvider = serviceProvider;

            _hostingEnvironment = hostingEnvironment;
            _converter = converter;
            this.urlHelperFactory = urlHelperFactory;
        }
        public IActionResult Index()
        {
            _logger.Info("user orders");
            return View();
        }

        public async Task<IActionResult> OnlinePay(int id)
        {
            _logger.Info("Online payment begining");
            if (HttpContext.Session.GetString("name") != null)
            {
                var o = ordservice.GetOrder(id);
                _logger.Info("OrderId="+id);
                var client = new HttpClient();

                var url = "https://test-gateway.mastercard.com/api/rest/version/79/merchant/TEST0007/session";

                var payload = new
                {
                    apiOperation = "INITIATE_CHECKOUT",
                    interaction = new
                    {
                        operation = "PURCHASE",
                          returnUrl = "https://localhost:44337/Order/Paid?id=" + id,
                        merchant = new
                        {
                            name = "TEST0007",

                        }
                    },
                    order = new
                    {
                        currency = "KWD",
                        amount = o.Order_Amount,
                        id = o.Id,
                        description = "Nothing",
                    }
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes("merchant.TEST0007:c4a9e601eed59c5a45cf31044e617581"));

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuth);

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseBody);

                    // Extract the value of the 'id' field
                    string sessionId = (string)responseJson["session"]["id"];
                    _logger.Info("SessionId="+sessionId);
                    sessiona s = new sessiona();
                    s.sessionida = sessionId.ToString();

                    _logger.Info("Session Created");
                    //  return Ok(sessionId);
                    return View(s);
                }
                else
                {
                    return Ok("Bad");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }



        }

        public async Task<IActionResult> Paid(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                // Replace these with your actual credentials
                string username = "merchant.TEST0007";
                string password = "c4a9e601eed59c5a45cf31044e617581";

                // API endpoint URL
                string apiUrl = "https://test-gateway.mastercard.com/api/rest/version/79/merchant/TEST0007/order/" + id;

                // Create an HttpClientHandler to handle authentication
                var httpClientHandler = new HttpClientHandler
                {
                    // Set credentials
                    Credentials = new System.Net.NetworkCredential(username, password)
                };

                // Create an HttpClient with the handler
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    // Make sure we request JSON
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Send GET request
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Check if the request was successful (status code 200)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read response content
                        string responseData = await response.Content.ReadAsStringAsync();
                        JObject responseJson = JObject.Parse(responseData);

                        // Extract the value of the 'id' field
                        string result = (string)responseJson["result"];
                        if (result == "SUCCESS")
                        {
                            Onlinepaid();

                            _logger.Info("Payment Completed successfully");
                           // _logger.Info(responseData);

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {
                        return BadRequest("Bad");
                    }
                }

            }
            else 
            {
                return RedirectToAction("login", "Login");
            }
        }



        //Add to Cart 

        public IActionResult Placeorder(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {

                var row = medservice.Getmedicine(id);
                return View(row);
            }
            else {
                return RedirectToAction("login", "Login");
            }
        }




        //place new item in cart or merge items in cart

        [HttpPost]
        public IActionResult OrderNow(int Quantity, int Amount, int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                mergeduplicatecart(Quantity, Amount, id);

                return RedirectToAction("Index", "Home");
            }
            else {
                return RedirectToAction("login", "Login");
            }
        }



        //user_order_list
        public IActionResult Listorders()
        {
            
            if (HttpContext.Session.GetString("name") != null)
            {
                return View(OrderList().OrderByDescending(x => x.o.Ordered_date));
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }




        //For user as well as admin

        public IActionResult ListCart()
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                return View(OrderList().OrderByDescending(x => x.o.Ordered_date));
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }






        //Admin_listallorders

        public IActionResult Listallorders(int pageNumber = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                List<Order> list = ordservice.GetAllOrders().ToList();
                HttpContext.Session.SetInt32("count", ordservice.GetPendingOrderCount());


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

                return View(ConvertOrderToViewModelAll(pagedData));

                // return View(ConvertOrderToViewModelAll(list));
            }

            else
            {
                return RedirectToAction("login", "Login");
            }
        }





        //for user
        public IActionResult PreConfirm()
        {
            List<OrderViewModelAll> list = new List<OrderViewModelAll>();
            if (HttpContext.Session.GetString("name") != null)
            {

                list = generate_invoice();

                if (list != null)
                {
                    return View(list);
                }
                else
                {
                    return RedirectToAction("ListCart", "Order");
                }
            }
            else
            {
                return RedirectToAction("login", "Login");
            }


        }




        //user_checkout

        public IActionResult Confirm(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                Confirmation_Email();
                return RedirectToAction("Listorders", "Order");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }





        // user_cancel

        public IActionResult Cancel(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                var row = ordservice.GetOrder(id);
                row.Order_status = "Cancelled";
                ordservice.UpdateOrder(row);
                return RedirectToAction("Listorders", "Order");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }




        //user_delete

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("name") != null)
            {
                ordservice.DeleteOrder(ordservice.GetOrder(id));
                return RedirectToAction("Listorders", "Order");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }





        //Admin_Approve

        public IActionResult Admin_Approve(int id)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                var row = ordservice.GetOrder(id);
                if (row != null)
                {
                    var Ordered_med = medservice.Getmedicine(row.Med_Id.GetValueOrDefault());
                    Ordered_med.Quantity = Ordered_med.Quantity - row.Order_Qty;
                    medservice.UpdateMedicine(Ordered_med);
                    row.Order_status = "Approved:preparing delivery";
                    ordservice.UpdateOrder(row);
                }
                return RedirectToAction("Listallorders", "Order");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }






        //Admin_Cancel
        public IActionResult Admin_Cancel(int id)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                var row = ordservice.GetOrder(id);
                row.Order_status = "Cancelled";
                ordservice.UpdateOrder(row);
                return RedirectToAction("Listallorders", "Order");
            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }






        //Admin_Delete
        public IActionResult Admin_Delete(int id)
        {
            if (HttpContext.Session.GetString("name") != null && HttpContext.Session.GetString("name").ToString() == "Admin")
            {
                ordservice.DeleteOrder(ordservice.GetOrder(id));
                return RedirectToAction("Listallorders", "Order");

            }
            else
            {
                return RedirectToAction("login", "Login");
            }
        }
















































        //Helping methods


        public void Onlinepaid()
        {

            int userid = HttpContext.Session.GetInt32("Userid").GetValueOrDefault();
            var ordlist = ordservice.CheckOrder(userid);
            foreach (var item in ordlist)
            {

                if (item.Order_status == "InCart")
                {
                    item.Order_status = "Approved:preparing delivery";
                    ordservice.UpdateOrder(item);
                }
            }
        }

        public void Confirmation_Email()
        {

            int userid = HttpContext.Session.GetInt32("Userid").GetValueOrDefault();
            var ordlist = ordservice.CheckOrder(userid);
            foreach (var item in ordlist)
            {

                if (item.Order_status == "InCart")
                {
                    item.Order_status = "Pending";
                    ordservice.UpdateOrder(item);
                }
            }


            string email_address = usrservice.GetUser(userid).Email.ToString();
            string msg = $@"Dear,{usrservice.GetUser(userid).Name}
            An order has been placed from Emedicine.
            You will receive the order in 2 working days. 
            Click below link for more details,
            https://emedicine.com

            Thanks and Regards,
            Support Emed";
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "Images", "output.pdf");
            SendEmailWithAttachment(email_address, "Order Confirmation from Emed", msg, path);
            //   SendEmail(usrservice.GetUser(userid).Email, "Order Confirmation from Emed", msg);


        }





        //send email method
        public void SendEmail(string to, string subject, string body)
        {
            // Configure SMTP client
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
               
                Credentials = new NetworkCredential("emedicine253@gmail.com", "dfpw dmyj uzdv qlzt"),
                EnableSsl = true,
            };

            // Create email message
            var message = new MailMessage("emedicine253@gmail.com", to, subject, body);

            // Send email
            smtpClient.Send(message);
        }







        public void SendEmailWithAttachment(string recipient, string subject, string body, string attachmentPath)
        {
            // SMTP server settings
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587; // SMTP port (e.g., 587 for Gmail)
            string smtpUsername = "emedicine253@gmail.com";
            string smtpPassword = "dfpw dmyj uzdv qlzt";

            // Create the email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(smtpUsername);
            mail.To.Add(recipient);
            mail.Subject = subject;
            mail.Body = body;

            // Attach the file
            Attachment attachment = new Attachment(attachmentPath);
            mail.Attachments.Add(attachment);

            // Configure the SMTP client
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true; // Enable SSL if required by your SMTP server

            // Send the email
            smtpClient.Send(mail);

            // Dispose the attachment and mail objects
            attachment.Dispose();
            mail.Dispose();
        }






       public void mergeduplicatecart(int Quantity, int Amount, int id)
        {
            int user_id = HttpContext.Session.GetInt32("Userid").GetValueOrDefault();
            var row = usrservice.GetUser(user_id);
            var customer_ord_list = ordservice.CheckOrder(user_id);
            int duplicate_flag = 0;
            foreach (var item in customer_ord_list)
            {
                if (item.Med_Id == id && item.Order_status == "InCart")
                {
                    item.Order_Qty = item.Order_Qty + Quantity;
                    item.Ordered_date = DateTime.Now;
                    item.Order_Amount = item.Order_Amount + Amount;
                    ordservice.UpdateOrder(item);
                    duplicate_flag++;
                }
            }
            if (duplicate_flag == 0)
            {
                Order o = new Order();
                o.Customer_Id = user_id;
                o.Order_status = "InCart";
                o.Order_Qty = Quantity;
                o.Order_deliver_address = row.Address;
                o.Ordered_date = DateTime.Now;
                o.Order_Amount = Amount;
                o.Med_Id = id;
                ordservice.AddOrder(o);
            }

        }



        public List<OrderViewModel> OrderList()
        {
            int user_id = HttpContext.Session.GetInt32("Userid").GetValueOrDefault();
            var list = ordservice.CheckOrder(user_id);


            var viewlist = new List<OrderViewModel>();
            foreach (var item in list)
            {
                var orderViewModel = new OrderViewModel
                {
                    m = medservice.Getmedicine(item.Med_Id.GetValueOrDefault()),
                    o = ordservice.GetOrder(item.Id)
                };
                viewlist.Add(orderViewModel);

            }
            return viewlist;

        }




        public List<OrderViewModelAll> ConvertOrderToViewModelAll(List<Order> list)
        {

            var viewlist = new List<OrderViewModelAll>();
            foreach (var item in list)
            {
                var orderViewModelAll = new OrderViewModelAll
                {
                    m = medservice.Getmedicine(item.Med_Id.GetValueOrDefault()),
                    o = ordservice.GetOrder(item.Id),
                    u = usrservice.GetUser(item.Customer_Id.GetValueOrDefault())
                };
                viewlist.Add(orderViewModelAll);

            }
            return viewlist;

        }












        public List<OrderViewModelAll> generate_invoice()
        {
            int user_id = HttpContext.Session.GetInt32("Userid").GetValueOrDefault();
            List<OrderViewModelAll> viewlist = new List<OrderViewModelAll>();
            var list = ordservice.GetAllCartItems(user_id);
            if (list.Count != 0)
            {
                viewlist = ConvertOrderToViewModelAll(list);
            }
            else
            {
                return null;
            }
            CultureInfo culture = new CultureInfo("ur-PK");

            string htmlContent = "";


            htmlContent += $@"
<html>
<head>
<link rel=""stylesheet"" href=""D:\AllCompanyoldlaptopdata\EMEDICINE1\EMedicineServices\wwwroot\lib\bootstrap\dist\css\bootstrap.min.css"">
</head>
<body>
<div class=""Container"" style=""border:1px solid lightgrey;"">
    <div class=""row"">
        <div class=""col-sm-4"" style=""background-color:black;color:white;padding-top:20px;padding-bottom:10px;padding-left:10px"">
            
<img src=""D:\AllCompanyoldlaptopdata\EMEDICINE1\EMedicineServices\wwwroot\Images\pics\logo.png"" />
            <span>  EMEDICINE</span>       
        </div>
        <div class=""col-sm-4""></div>       
        <div class=""col-sm-4"" style=""background-color:#10e7f4;color:white;text-align:center"">
            <h1>Invoice</h1>
        </div>
    </div>
    <div class=""row"">
        <div class=""col-md-4"">
            <span>Invoice Number: {viewlist.FirstOrDefault().o.Id}</span>
            <br />
            <span>Dated: {DateTime.Now.ToShortDateString()}</span>
        </div>
        <div class=""col-md-4""></div>
        <div class=""col-md-4""></div>
        <hr />
    </div>
    <div class=""row"">
        <div class=""col-md-4"">
            <p>From:</p>
            <p><b>Company Name</b>: <span>Emedicine</span></p>
            <p><b>Phone</b>: <span>051-4472446</span></p>
            <p><b>Email</b>: <span>emed253@gmail.com</span></p>
        </div>
        <div class=""col-md-4""></div>
        <div class=""col-md-4"">
            <p>To:</p>
            <p><b>Customer Name</b>: {viewlist.FirstOrDefault().u.Name}</p>
            <p><b>Email</b>: {viewlist.FirstOrDefault().u.Email}</p>
            <p><b>Address</b>: {viewlist.FirstOrDefault().u.Address}</p>
        </div>
       
    </div>
    <div class=""row"">
        <div class=""col-md-12"">
            <table class=""table"" style=""margin-bottom:100px; text-align:center"">
                <thead>
                    <tr >
                        <th>Name</th>
                        <th>Qty</th>                   
                        <th>Price(PKR)</th>
                        <th>Amount(PKR)</th>
                    </tr>    
                </thead>
                <tbody>";

            // Calculate total amount
            int total = 0;
            
            foreach (var item in viewlist)
            {
                if (item.o.Order_status == "InCart")
                {
                    // Calculate amount for the current item
                    int amount = item.m.Price.GetValueOrDefault() * item.o.Order_Qty.GetValueOrDefault();
                    total += amount; // Accumulate total amount
                   
                    // Add row to the table
                    htmlContent += $@"
        <tr >
            <td>{item.m.Name}</td>
            <td>{item.o.Order_Qty}</td>
            <td>{item.m.Price.GetValueOrDefault().ToString("C",culture)}</td>
            <td>{amount.ToString("C",culture)}</td>
        </tr>";
                }
            }

            htmlContent += $@"
                </tbody>
            </table>
        </div>
        <hr />
    </div>
    <div class=""row"">
        <div class=""col-md-4"">
            <p>Terms and Conditions:</p>
        </div>
        <div class=""col-md-4""></div>
        <div class=""col-md-4"">
            <p>Subtotal: <span style=""margin-left:200px;"">{total.ToString("C", culture)}</span></p>
            <hr />
            <p>Discount:</p>
            <hr />
            <p>Tax:</p>
            <hr />
            <p>Total: <span style=""margin-left:225px;"">{total.ToString("C", culture)}</span></p>
        </div>
        <hr />
    </div>
    <div class=""row"">
        <div class=""col-md-4""></div>
        <div class=""col-md-4""></div>
        <div class=""col-md-4"" style=""background-color:black;color:white;"">
                    
            <p>Total: <span  style=""margin-left:220px;"">{total.ToString("C", culture)}</span></p>
        </div>
    </div>
</div>
</body>
</html>";




            _logger.Info("Starting ConvertHtmlToPdf");
            Console.WriteLine("Starting ConvertHtmlToPdf");
            var pdfBytes = ConvertHtmlToPdf1(htmlContent);
            Console.WriteLine("ConvertHtmlToPdf is done");
            _logger.Info("ConvertHtmlToPdf is done");
            // Return the PDF as a file
            //  return File(pdfBytes, "~/Images", "generated.pdf");
            // return View(viewlist);
            if (pdfBytes != null)
            {
                string wwwRootPath = _hostingEnvironment.WebRootPath;

                // Construct the path to the "images" folder within the "wwwroot" folder
                string imagesFolderPath = Path.Combine(wwwRootPath, "images");

                // Ensure the "images" folder exists
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                // Construct the file path for the PDF file within the "images" folder
                string pdfFilePath = Path.Combine(imagesFolderPath, "output.pdf");
                _logger.Info("Saving PdfBytes");
                Console.WriteLine("Saving PdfBytes");
                // Write the PDF byte array to the file
                System.IO.File.WriteAllBytes(pdfFilePath, pdfBytes);
                _logger.Info("SSaving PdfBytes is done");
                Console.WriteLine("Saving PdfBytes is done");
            }
            
            return viewlist;

        }





        //convert html string to pdf

        private byte[] ConvertHtmlToPdf(string htmlContent)
        {
            try
            {
                var converter = new BasicConverter(new PdfTools());
                _logger.Info("Before new HtmlToPdfDocument");
                Console.WriteLine("Before new HtmlToPdfDocument");
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                                  //  PaperSize = PaperKind.A4,
                                    Orientation = Orientation.Portrait
                        },

                    Objects = {
                        new ObjectSettings() {
                                    HtmlContent = htmlContent
                                 }
                              }
                 };
                _logger.Info("[INFO] Before Convert Method");
                Console.WriteLine("[INFO] Before Convert Method");
                if (doc.GetObjects().Count() != 0)
                {
                    return _converter.Convert(doc);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            { 
                _logger.Error("[PDF Conversion failed: "+ex.Message);
                Console.WriteLine("[Error]PDF Conversion failed");
                return null;
            }

        }


        public byte[] ConvertHtmlToPdf1(string htmlContent)
        {
            //var pdf = Pdf.From(htmlContent).Content();
            //return pdf.Bytes;


            //var pdf1 = Pdf.From(htmlContent).Content();
            //byte[] pdfBytes = pdf1.ToArray();
            //return pdfBytes;


            var pdf1 = Pdf.From(htmlContent).Content();
            return pdf1;
        }

    }
}
