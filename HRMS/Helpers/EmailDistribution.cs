using RA.EmailDistribution.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class EmailDistribution
    {
        public void SendEmail(string emailTo, string subject, string text, string html)
        {
            var email = new EmailDistributionData();
            email.AddEmail(
                    "no-reply@hrms.co.uk",
                    emailTo,
                    "",
                    "",
                    subject,
                    new List<string>{ },
                    text,
                    html,
                    1,
                    "",
                    "",
                    "",
                    DateTime.Now
                );
            
        }
    }
}