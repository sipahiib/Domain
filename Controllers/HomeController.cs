
using IbrahimSipahi.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.Mvc;


namespace IbrahimSipahi.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var followups = new List<Followups>()
            {
                new Followups() { Name= "Tim Corey", Link="https://www.youtube.com/user/IAmTimCorey"} ,
                new Followups() { Name= "Bora Kaşmer", Link="https://www.youtube.com/channel/UCEQB9Atxfn5AJgX6KfwvTyA"} ,
                new Followups() { Name= "dotNET", Link="https://www.youtube.com/channel/UCvtT19MZW8dq5Wwfu6B0oxw"},
                new Followups() { Name= "NDC Conferences", Link="https://www.youtube.com/channel/UCTdw38Cw6jcm0atBPA39a0Q"},
                new Followups() { Name= "Barış Özcan", Link="https://www.youtube.com/channel/UCv6jcPwFujuTIwFQ11jt1Yw"},
                new Followups() { Name= "Özgür Demirtaş", Link="https://twitter.com/ProfDemirtas"},
                new Followups() { Name= "Emin Çapa", Link="https://twitter.com/ecapa_aklinizi"}
            };

            var educations = new List<Education>()
            {
                new Education() { Name="Medium" , Link="https://www.medium.com/"},
                new Education() { Name="Machine Learning" , Link="https://www.coursera.org/learn/machine-learning"},
                new Education() { Name="edX-Online Course" , Link="https://www.edx.org/"},
                new Education() { Name="Edabit" , Link="https://edabit.com"},
                new Education() { Name="Hackerrank" , Link="https://hackerrank.com"},
                new Education() { Name="Flutter" , Link="https://www.youtube.com/watch?v=ulg2dpPkulw&list=PLUbFnGajtZlX9ubiLzYz_cw92esraiIBi&index=1"},
                new Education() { Name="Microservices" , Link="https://microservices.io/"},
                new Education() { Name="Techie Delight" , Link="https://www.techiedelight.com/"},
                new Education() { Name="Geeks for Geeks" , Link="https://www.geeksforgeeks.org/"},
                new Education() { Name="Mobilhanem" , Link="https://www.mobilhanem.com/"}
            };

            var model = new ViewModels.ViewModels()
            {
                Followups = followups,
                Educations = educations
            };

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(MailModels e)
        {
            if (ModelState.IsValid)
            {

                StringBuilder message = new StringBuilder();
                MailAddress from = new MailAddress(e.Email.ToString());
                message.AppendLine("Email: " + e.Email);
                message.AppendLine(e.Message);

                MailMessage mail = new MailMessage();

                mail.From = from;
                mail.To.Add("info@ibrahimsipahi.com");
                mail.Subject = "Yeni Bir Görüş";
                mail.Body = message.ToString();
                mail.IsBodyHtml = true;

                SendMail(mail);
            }

            ModelState.Clear();
            return View();
        }

        private void SendMail(MailMessage mail)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.ibrahimsipahi.com", port))
                {
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = false;
                    NetworkCredential cred = new NetworkCredential("username", "pass");
                    cred.Domain = "info@ibrahimsipahi.com";
                    smtpClient.Credentials = cred;

                    smtpClient.Send(mail);
                    ViewBag.Mail = "Mail gönderildi";
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                {
                    var status = t.StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Response.Write("Delivery failed - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                    {
                        ViewBag.Mail = "Mail gönderilemedi";
                        Response.Write("Failed to deliver message ");
                    }
                }
            }
            catch (SmtpException Se)
            {
                ViewBag.Mail = "Mail gönderilemedi";
                Response.Write(Se.ToString());
            }

            catch (Exception ex)
            {
                ViewBag.Mail = "Mail gönderilemedi";
                Response.Write(ex.ToString());
            }
        }
    }
}
