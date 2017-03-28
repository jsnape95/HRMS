using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class EmployeeHolidayLink
    {
        [Key]
        public int EmployeeHolidayLinkId { get; set; }

        public int EmployeeId { get; set; }

        public int HolidayId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Holiday Holiday { get; set; }
    }
}