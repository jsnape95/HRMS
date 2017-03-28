using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Applicant
    {
        public int ApplicantId { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string ResumeURL { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual List<VacancyApplicantLink> VacancyApplicantLinks { get; set; }
    }
}