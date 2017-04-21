using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class EmployeeDocument
    {
        [Key]
        public int EmployeeDocumentId { get; set; }

        public string Name { get; set; }

        public DocumentCategory DocumentCategory { get; set; }

        public virtual List<EmployeeEmployeeDocumentLink> EmployeeEmployeeDocumentLinks { get; set; }
    }

    public enum DocumentCategory
    {
        [Display(Name = "Personal Documents")]
        PersonalDocument = 1,

        [Display(Name = "Pre-Employement")]
        PreEmployment = 2,

        [Display(Name = "Induction Forms")]
        InductionForms = 3,

        [Display(Name = "L&D")]
        LearningAndDevelopment = 4,

        [Display(Name = "Other")]
        Other = 5
    };
}