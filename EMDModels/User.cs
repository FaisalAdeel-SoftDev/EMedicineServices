using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EMDModels
{
    public class User
    {
        private readonly EMEDContext _context;

       
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string? Password { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? Roletype { get; set; }
        public DateTime? CreatedDate { get; set; }

        [NotMapped]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Password do not match")]
        public string? Confirm_Password { get; set; }
    }

}

