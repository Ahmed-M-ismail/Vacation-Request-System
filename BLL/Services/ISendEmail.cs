using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface ISendEmail
    {

        /// <summary>
        /// Sendings the email.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="emailTo">The email to.</param>
        /// <param name="adminEmail">The admin email.</param>
        /// <param name="adminPass">The admin pass.</param>
        /// <returns></returns>
        bool SendingEmail(string subject , string body  , string emailTo, string adminEmail, string adminPass);
    }
}
