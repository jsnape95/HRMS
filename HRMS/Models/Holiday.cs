using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
       
        public virtual List<EmployeeHolidayLink> EmployeeHolidayLinks { get; set; }
    }


}