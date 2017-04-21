using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models
{
    public class HolidayJsonViewModel
    {
        public int id { get; set; }

        public string title { get; set; }

        public string start { get; set; }

        public string end { get; set; }

        public string color { get; set; }

        public string textColor { get; set; }
    }
}