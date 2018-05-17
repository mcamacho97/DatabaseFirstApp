using DatabaseFirstApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DatabaseFirstApp.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseFirstDbEntities db = new DatabaseFirstDbEntities();
        public ActionResult Index()
        {
            return View(db.Registrations.ToList());
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register (Registration reg)
        {
            if (ModelState.IsValid)
            {
                db.Registrations.Add(reg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Registration user)
        {
            if (ModelState.IsValid)
            {
                if (ValidateUser (user.UserName, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("UserCP", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "");
                }
            }
            return View();
        }

        private bool ValidateUser(string userName, string password)
        {
            bool isValid = false;

            using (var db = new DatabaseFirstDbEntities())
            {
                var user = db.Registrations.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    if (user.Password == password)
                    {
                        Session["UserId"] = user.UserId;
                        Session["FirstName"] = user.FirstName;
                        Session["LastName"] = user.LastName;
                        Session["UserName"] = user.UserName;
                        Session["Email"] = user.Email;
                        Session["Mobile"] = user.Mobile;

                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        public ActionResult UserCP()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}