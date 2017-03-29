using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class EmployeeDocument
    {
        [Key]
        public int EmployeeDocumentId { get; set; }

        public DocumentType DocumentType { get; set; }

        public string DocumentURL { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
    }

    public enum DocumentType {
        Contract = 1
    };
}