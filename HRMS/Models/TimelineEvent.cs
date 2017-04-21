using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class TimelineEvent
    {
        [Key]
        public int TimelineEventId { get; set; }

        [Required]
        public int ApplicantId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a correct event")]
        public Event Event { get; set; }

        [Required]
        public string Heading { get; set; }

        [Required]
        public string Notes { get; set; }

        public string FontAwesomeIcon { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }


        public virtual Applicant Applicant { get; set; }

    }

    public enum Event
    {
        Interview = 1,
        Assessment = 2,
        CV = 3,
        [Display(Name = "Background Check")]
        BackgroundCheck = 4,
        Other = 5
    };
}