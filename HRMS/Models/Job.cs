using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        public string JobTitle { get; set; }

        public string JobSpecification { get; set; }

        public virtual List<Employee> Employees { get; set; }

        public virtual List<Vacancy> Vacancy { get; set; }
    }
}