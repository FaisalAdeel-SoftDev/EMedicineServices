using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using EMDModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
namespace MedDataService
{
    public class MedicineRepositaryServices
    {
        //private readonly Medicine_Repositary_Data _data;
        private readonly MedicineRepositaryData _dataacess;
        private readonly IWebHostEnvironment hostEnvironment;

        public MedicineRepositaryServices(MedicineRepositaryData _dataacess, IWebHostEnvironment hostEnvironment)
        {
           this._dataacess = _dataacess;
            this.hostEnvironment = hostEnvironment;
        }


        public IEnumerable<Medicine> getallmedicines()
        {
            return _dataacess.GetAllMedicines();

           
        }

        public void Insertmedicine(Medicine m)
        {


            if (m.filename != null && m.filename.Length > 0)
            {
                var name = Path.GetFileNameWithoutExtension(m.filename.FileName);
                var ext = Path.GetExtension(m.filename.FileName);
                var filename = name + Guid.NewGuid().ToString() + ext;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", filename);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    m.filename.CopyTo(fileStream);
                }

                var relativePath = $"~/Images/{filename}";
                m.CreatedDate = DateTime.Now;
                m.ImagePath = relativePath.ToString();
                 _dataacess.AddMedicine(m);
               
            }        
        }


        public void Deletemedicine(int id)
        {
            var row = _dataacess.GetMedicine(id);

            if (row != null)
            {
                var relativepath = row.ImagePath.ToString();
                //if (string.IsNullOrEmpty(relativepath))
                //{
                //    return BadRequest("Path is null or empty.");
                //}

                // Convert the relative path to an absolute path
                var filePath = hostEnvironment.WebRootPath + relativepath.Replace("~/", "/");

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Delete the file
                    System.IO.File.Delete(filePath);
                    // return Ok("File deleted successfully.");
                }
                //else
                //{
                //    return NotFound("File not found.");
                //}


                _dataacess.DeleteMedicine(row);
                
            }


        }

        public Medicine Getmedicine(int id)
        {

           return _dataacess.GetMedicine(id);

        }

        public IEnumerable<Medicine> GetmedicinebyName(string name)
        {

            return _dataacess.GetMedicinebyName(name);

        }

        
        public bool UpdateMedicine(Medicine m)
        {
             if (m.filename != null)
            {
                var relativepath = m.ImagePath.ToString();
                //if (string.IsNullOrEmpty(relativepath))
                //{
                //    return BadRequest("Path is null or empty.");
                //}

                // Convert the relative path to an absolute path
                var filePath = hostEnvironment.WebRootPath + relativepath.Replace("~/", "/");

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Delete the file
                    System.IO.File.Delete(filePath);
                    // return Ok("File deleted successfully.");
                }
                else
                {
                    //return NotFound("File not found.");
                }



                if (m.filename != null && m.filename.Length > 0)
                {
                    var name = Path.GetFileNameWithoutExtension(m.filename.FileName);
                    var ext = Path.GetExtension(m.filename.FileName);
                    var filename = name + Guid.NewGuid().ToString() + ext;

                    var filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", filename);

                    using (var fileStream = new FileStream(filePath1, FileMode.Create))
                    {
                        m.filename.CopyTo(fileStream);
                    }

                    var relativePath = $"~/Images/{filename}";
                    m.ImagePath = relativePath.ToString();
                    _dataacess.UpdateMedicine(m);
                    //Context.Entry(m).State = EntityState.Modified;
                    //Context.SaveChanges();
                    return true;
                }


            }
            else
            {
                _dataacess.UpdateMedicine(m);
                return true;
                //Context.Entry(m).State = EntityState.Modified;
                //Context.SaveChanges();
            }
             return false;

        }



    }
}
