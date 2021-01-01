
using IbrahimSipahi.UI.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;

namespace PersonelMVCUI.Controllers
{
    public class SecurityController : Controller
    {
        sipahiibEntities sipahiibEntity = new sipahiibEntities();

        //[AllowAnonymous] //global.asax dosyasına "GlobalFilters" satırı eklendi.
        [HttpGet]
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
    }
}