using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project.MVC.Helpers
{
    public static class PasswordReset
    {
        public static void PasswordResetSendEmail(string link)
        {
            MailMessage mail = new MailMessage();
            
            mail.From = new MailAddress("n.ceren097@gmail.com");
            mail.To.Add("naciye.ceren097@gmail.com");
            mail.Subject = $"Şifre Sıfırlama";
            mail.Body = "Şifrenizi yenilemek için lütfen aşağıdaki linke tıklayınız.";
            mail.Body += $"<a href='{link}'> şifre yenileme linki</a>";
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
