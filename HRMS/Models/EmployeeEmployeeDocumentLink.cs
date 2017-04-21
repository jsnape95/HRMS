using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class EmployeeEmployeeDocumentLink
    {
        [Key]
        public int EmployeeEmployeeDocumentLinkId { get; set; }

        [Required]
        public int EmployeeDocumentId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string URL { get; set; }

        public DateTime DateUploaded { get; set; }


        public virtual EmployeeDocument EmployeeDocument { get; set; }

        public virtual Employee Employee { get; set; }
    }
}