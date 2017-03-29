using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        public bool IsManager { get; set; }

        public int? LineManagerId { get; set; }

        [Required]
        public int JobId { get; set; }

        public string ProfileImageUrl { get; set; }

        [Required]
        public decimal Salary { get; set; }
        
        [Required]
        public int HolidayEntitlement { get; set; }


        public virtual Job Job { get; set; }

        [ForeignKey("LineManagerId")]
        public virtual Employee LineManager { get; set; }

        public virtual List<EmployeeHolidayLink> EmployeeHolidayLinks { get; set; }

        public virtual List<EmployeeDocument> EmployeeDocuments { get; set; }
    }
}