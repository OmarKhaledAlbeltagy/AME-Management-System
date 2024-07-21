using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class EmailModel
    {
        public static string SmtpServer { get; } = "smtp.hostinger.com";

        public static int port { get; } = 587;  //465 //8889

        public static string EmailAddress { get; } = "register@{{DashboardURL}}";

        public static string Password { get; } = "Adminpass@1";
    }
}
