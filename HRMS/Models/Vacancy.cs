using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Vacancy
    {
        [Key]
        public int VacancyId { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }

        [Required]
        public DateTime CloseDate { get; set; }

        public int AmountOfOpenings { get; set; }

        public int PosistionsFilled { get; set; }

        public int PositionsOffered { get; set; }

        public int PositionsRejected { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public int JobId { get; set; }

        public virtual Job Job { get; set; }
    }
}