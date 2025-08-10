using DataAccess.MedInterface;
using EMDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MedicineRepositaryData: IMedicine_Repositary
    {
        private readonly EMEDContext Context;
        public MedicineRepositaryData(EMEDContext _Context)
        {
            Context = _Context;
        }
        public void AddMedicine(Medicine m)
        {
            Context.Medicines.Add(m);
            Context.SaveChanges();
        }
        public bool DeleteMedicine(Medicine row)
        {
           
            if (row != null)
            {
                Context.Medicines.Remove(row);
                Context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public IEnumerable<Medicine> GetAllMedicines()
        {
            return Context.Medicines.ToList().OrderByDescending(x=>x.CreatedDate);
        }

        public Medicine GetMedicine(int id)
        {
            var row = Context.Medicines.Find(id);

            return row;
        }
        public IEnumerable<Medicine> GetMedicinebyName(string name)
        {
            return Context.Medicines.Where(x => x.Name.Contains(name)).ToList();
        }
        public bool UpdateMedicine(Medicine m)
        {
            Context.Entry(m).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            Context.SaveChanges();
            return true;
        }
    }
}
