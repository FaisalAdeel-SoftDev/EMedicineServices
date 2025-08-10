using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EMDModels
{
    public class Medicine
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int? Price { get; set; }
        [Required]
        public int? Quantity { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        [NotMapped]
        public IFormFile filename { get; set; }
    }
}
