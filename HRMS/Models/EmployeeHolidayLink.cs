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

        public ApprovedStatus Approved { get; set; }

        public string Reason { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Holiday Holiday { get; set; }
    }

    public enum ApprovedStatus
    {
        Approved = 1,
        Pending = 2,
        Rejected = 3
    };
}