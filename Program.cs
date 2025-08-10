using DataAccess;
using DinkToPdf.Contracts;
using DinkToPdf;
using EMDModels;

using MedDataService;
using Microsoft.EntityFrameworkCore;
using log4net.Config;
using log4net;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var provider = builder.Services.BuildServiceProvider();
var config = provider.GetService<IConfiguration>();
//builder.Services.AddDbContext<EMEDContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));
builder.Services.AddDbContext<EMEDContext>(options =>
{
    options.UseSqlServer(config.GetConnectionString("dbcs"));
}, ServiceLifetime.Scoped);
builder.Services.AddScoped<MedicineRepositaryServices>();
builder.Services.AddScoped<MedicineRepositaryData>();
builder.Services.AddScoped<Userdata>();
builder.Services.AddScoped<UserRepositaryService>();
builder.Services.AddScoped<Orderdata>();
builder.Services.AddScoped<OrdersRepositaryServices>();






builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddHttpContextAccessor();

var path = ((IWebHostEnvironment)builder.Environment).ContentRootPath;

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});





var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("D:\\AllCompanyoldlaptopdata\\EMEDICINE1\\EMedicineServices\\Log4Net\\log4net.config"));









var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
