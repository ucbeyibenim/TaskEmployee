using System;
using System.ComponentModel.DataAnnotations;

namespace TaskEmployee.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Payroll Number")]
        public string Payroll_Number { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Forenames")]
        public string Forenames { get; set; }



        [Required]
        [StringLength(50)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }


        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime Date_of_Birth { get; set; }



        [Required]
        [StringLength(50)]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }



        [Required]
        [StringLength(50)]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Address")]
        public string Address { get; set; }



        [Required]
        [StringLength(50)]
        [Display(Name = "Address_2")]
        public string Address_2 { get; set; }



        [Required]
        [StringLength(50)]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }



        [Required]
        [StringLength(50)]
        [Display(Name = "EMail_Home")]
        public string EMail_Home { get; set; }


        [Required]
        [Display(Name = "Start Date")]
        public DateTime Start_Date { get; set; }
    }
}
