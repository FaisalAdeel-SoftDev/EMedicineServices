using EMDModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MedInterface
{
    public interface IMedicine_Repositary
    {
        public IEnumerable<Medicine> GetAllMedicines();
        public Medicine GetMedicine(int id);

        public IEnumerable<Medicine> GetMedicinebyName(string name);

        public void AddMedicine(Medicine m);

        public bool DeleteMedicine(Medicine m);

        public bool UpdateMedicine(Medicine m);
    }
}
