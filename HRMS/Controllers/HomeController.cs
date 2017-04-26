using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        HRMS.Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        public ActionResult Index()
        {

            var user = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

            if (User.IsInRole("Admin"))
            {
                //send to reports section

            }
            else
            {
                //redirect to 
            }


            return View();
        }

        [AllowAnonymous]
        public PartialViewResult _HeaderPartial()
        {
            var userName = User.Identity.Name;

            var user = db.Users.FirstOrDefault(x => x.Email == userName);

            return PartialView(user);
        }
        
    }
}