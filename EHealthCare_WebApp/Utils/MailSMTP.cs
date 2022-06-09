using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace EHealthCare_WebApp.Utils
{
    public class MailSMTP
    {
        public static void Send(String body, String subject, String to)
        {
            try
            {
                var smtp = new SmtpClient();
                var mail = new MailMessage();

                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("huynhthikimthoa192001@gmail.com", "amnlvikpxihyfcxa");

                mail.From = new MailAddress("huynhthikimthoa192001@gmail.com", "EHealthcare");
                mail.BodyEncoding = mail.SubjectEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                mail.Body = body;
                mail.Subject = subject;
                mail.To.Add(to);
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Send(mail);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}