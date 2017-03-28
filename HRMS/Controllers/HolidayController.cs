using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    [Authorize]
    public class HolidayController : Controller
    {
        // GET: Holiday
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCalendar()
        {
            return View();
        }
    }
}