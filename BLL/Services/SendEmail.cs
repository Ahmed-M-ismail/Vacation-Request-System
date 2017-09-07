using System.Net;
using System.Net.Mail;

namespace BLL.Services
{
    public class SendEmail : ISendEmail
    {
        public bool SendingEmail(string subject, string Body, string emailTo , string adminEmail , string adminPass)
        {
      
            var message = new MailMessage();

            message.To.Add(emailTo);


            message.From = new MailAddress(adminEmail); // replace with valid value
            message.Subject = subject;
            message.Body = Body;
            message.IsBodyHtml = true;
          

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {




                    UserName = adminEmail, // replace with valid value
                    Password = adminPass // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "mail.youxel.com";
                smtp.Port = 26;

               
                smtp.Send(message);

             

            }


            return true;


        }
    }
}
