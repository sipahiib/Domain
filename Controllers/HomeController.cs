
using IbrahimSipahi.UI.Models;
using System;
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
        sipahiibEntities sipahiibEntity = new sipahiibEntities();

        public ActionResult Index()
        {
            string userIP = Request.ServerVariables["HTTP_CLIENT_IP"] == null ? Request.UserHostAddress : Request.ServerVariables["HTTP_CLIENT_IP"];

            //if (!(sipahiibEntity.IPAddress.ToList().Where(x => x.IPAddress1 == userIP).Select(y => y.IPAddress1).Any()))
            //{
            //    ViewBag.Mesaj = "İlk kez bu siteye giriş yapıyorsun. Teşekkürler!";
            //}
            //else
            //{
            //    ViewBag.Mesaj = "Tekrar hoşgeldin!";
            //}

            sipahiibEntity.IPAddress.Add(new Models.IPAddress { IPAddress1 = userIP, CreateDate = DateTime.Now });
            sipahiibEntity.SaveChanges();

            return View();
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
                using (var smtpClient = new SmtpClient("smtp.ibrahimsipahi.com", 587))
                {
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = false;
                    NetworkCredential cred = new NetworkCredential("info@ibrahimsipahi.com", "4XuRF3K1");
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