using HRMS.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    [Authorize]
    public class HolidayController : Controller
    {
        
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Holiday
        public ActionResult Index()
        {
            var name = User.Identity.Name;
            var user = db.Users.FirstOrDefault(x => x.UserName == name);

            return View(user);
        }

        public ActionResult ViewCalendar()
        {
            return View();
        }

        public JsonResult GetUsersHolidays()
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            var allHolidays = db.EmployeeHolidayLinks.Where(x => x.EmployeeId == (int)user.EmployeeId && x.Approved != ApprovedStatus.Rejected).ToList();
            var jsonHols = allHolidays.Select(x => new HolidayJsonViewModel {
                    title = x.Approved == ApprovedStatus.Approved ? "Approved" : "Pending",
                    start = x.Holiday.StartDate.ToString("yyyy-MM-dd"),
                    end = x.Holiday.EndDate.ToString("yyyy-MM-dd"),
                    color = x.Approved == ApprovedStatus.Approved ? "#00ff00" : "#ffcc00",
                    textColor = "#000000"
            });
            
            return Json(jsonHols, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult RequestHoliday(DateTime start, DateTime end)
        {
            if (start != end)
            {
                start = start.AddHours(9);
                end = end.AddHours(17);
            }

            var holidayRequest = new Holiday
            {
                StartDate = start,
                EndDate = end 
            };

            return PartialView(holidayRequest);
        }

        [HttpPost]
        public ActionResult RequestHoliday(Holiday holiday, int? time)
        {
            //get user info
            var user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            switch (time)
            {
                case 1:
                    holiday.StartDate = holiday.StartDate.AddHours(9);
                    holiday.EndDate = holiday.EndDate.AddHours(17);
                    break;
                case 2:
                    holiday.StartDate = holiday.StartDate.AddHours(9);
                    holiday.EndDate = holiday.EndDate.AddHours(13);
                    break;
                case 3:
                    holiday.StartDate = holiday.StartDate.AddHours(13);
                    holiday.EndDate = holiday.EndDate.AddHours(17);
                    break;
                case null:
                    break;
            }


            //see if holiday already exsists in db
            var holidayExists = db.Holidays.FirstOrDefault(x => x.StartDate == holiday.StartDate && x.EndDate == holiday.EndDate);

            int newHolId;
            if (holidayExists == null)
            {
                db.Holidays.Add(holiday);
                db.SaveChanges();
                newHolId = holiday.HolidayId;
            }
            else
            {
                newHolId = holidayExists.HolidayId;
            }

            var holidayLink = new EmployeeHolidayLink
            {
                EmployeeId = (int)user.EmployeeId,
                HolidayId = newHolId,
                Approved = ApprovedStatus.Pending
            };

            db.EmployeeHolidayLinks.Add(holidayLink);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}