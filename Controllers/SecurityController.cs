
using IbrahimSipahi.UI.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace PersonelMVCUI.Controllers
{
    public class SecurityController : Controller
    {
        //[AllowAnonymous] //global.asax dosyasına "GlobalFilters" satırı eklendi.
        public ActionResult Login()
        {
            return View();
        }

        //[AllowAnonymous]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Id,Email,Sifre,Onaysifre")] Users kullanici)
        {
            Session["kullanici"] = null;
            if (ModelState.IsValid)
            {
                sipahiibEntities sipahiibEntity = new sipahiibEntities();

                var firstLogin = sipahiibEntity.Users.FirstOrDefault(m => m.Email == kullanici.Email);
                String[] parts = kullanici.Email.Split(new[] { '@' });

                if (firstLogin == null && kullanici.Email != null)
                {
                    FormsAuthentication.SetAuthCookie(kullanici.Email, false);

                    kullanici.OnaySifre = kullanici.Sifre;
                    Session["kullanici"] = parts[0];

                    sipahiibEntity.Users.Add(kullanici);
                    sipahiibEntity.SaveChanges();
                }

                var validKullanici = sipahiibEntity.Users.FirstOrDefault(m => m.Email == kullanici.Email && m.Sifre == kullanici.Sifre);

                if (validKullanici != null)
                {
                    Session["kullanici"] = parts[0];
                    FormsAuthentication.SetAuthCookie(validKullanici.Email, false);
                }
                else
                {
                    ViewBag.Invalid = "Geçersiz kullanıcı adı veya şifre";
                    return View();
                }

                return RedirectToAction("Index", "Home");
            }

            return View(kullanici);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["kullanici"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult PasswordRemember()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordRemember(Users e)
        {
            if (e.Email == null)
            {
                ViewBag.Remember = "Email girmeniz gerekmektedir";
            }
            else
            {
                sipahiibEntities sipahiibEntity = new sipahiibEntities();

                var existingEmail = sipahiibEntity.Users.FirstOrDefault(m => m.Email == e.Email);
                if (existingEmail != null && existingEmail.Email == e.Email)
                {
                    StringBuilder message = new StringBuilder();
                    MailAddress fromAdd = new MailAddress("info@ibrahimsipahi.com");

                    message.AppendLine("Şifreniz: " + existingEmail.Sifre);

                    MailMessage mail = new MailMessage();

                    mail.From = fromAdd;
                    mail.To.Add(existingEmail.Email.ToString());
                    mail.Subject = "Kayıtlı Şifreniz";
                    mail.Body = message.ToString();
                    mail.IsBodyHtml = true;

                    SendMail(mail);
                }
                else
                {
                    ViewBag.Remember = "Kayıtlı olmayan bir email girdiniz";
                }
            }
            ModelState.Clear();
            return View();
        }

        private void SendMail(MailMessage mail)
        {
            try                
            {
                string password = WebConfigurationManager.AppSettings["MailPassword"];

                using (var smtpClient = new SmtpClient("smtp.ibrahimsipahi.com", 587))
                {
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = false;
                    NetworkCredential cred = new NetworkCredential("info@ibrahimsipahi.com", password);
                    cred.Domain = "ibrahimsipahi.com";
                    smtpClient.Credentials = cred;

                    smtpClient.Send(mail);
                    ViewBag.Remember = "Şifreniz mail adresinize gönderilmiştir";
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
                        ViewBag.Remember = "Şifreniz gönderilemedi!";
                        Response.Write("Failed to deliver message ");
                    }
                }
            }
            catch (SmtpException Se)
            {
                ViewBag.Remember = "Şifreniz gönderilemedi!";
                Response.Write(Se.ToString());
            }

            catch (Exception ex)
            {
                ViewBag.Remember = "Şifreniz gönderilemedi!";
                Response.Write(ex.ToString());
            }
        }

    }
}