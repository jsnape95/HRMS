using RA.EmailDistribution.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class EmailDistribution
    {
        public void SendEmail(string emailTo, string subject)
        {
            var email = new EmailDistributionData();
            email.AddEmail(
                    "no-reply@hrms.co.uk",
                    emailTo,
                    "",
                    "",
                    subject,
                    new List<string>{ },
                    "Welcome to the HRMS.",
                    "<html><body><p>Welcome to the HRMS!</p></body></html>",
                    1,
                    "",
                    "",
                    "",
                    DateTime.Now
                );
            
        }
    }
}