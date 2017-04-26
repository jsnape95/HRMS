using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class VacancyApplicantLink
    {
        [Key]
        public int VacancyApplicantLinkId { get; set; }

        public int VacancyId { get; set; }

        public int ApplicantId { get; set; }

        public Status ApplicantStatus { get; set; }


        public virtual Vacancy Vacancy { get; set; }

        public virtual Applicant Applicant { get; set; }

        public virtual List<TimelineEvent> TimelineEvents { get; set; }
    }

    public enum Status {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }
}