using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        public string Name { get; set; }

        public virtual List<Job> Jobs { get; set; }
    }
}