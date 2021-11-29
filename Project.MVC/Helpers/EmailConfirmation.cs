using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project.MVC.Helpers
{
    public static class EmailConfirmation
    {
        public static void EmailConfirmationSendEmail(string link, string email)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("n.ceren097@gmail.com");
            mail.To.Add(email);
            mail.Subject = $"Email Verification";
            mail.Body = "Click on the link to verify your mail";
            mail.Body += $"<a href='{link}'> email verification link</a>";
            mail.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential("n.ceren097@gmail.com", "nacibo097");
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.office365.com";
            smtpClient.EnableSsl = true;


            smtpClient.Send(mail);
        }
    }
}
