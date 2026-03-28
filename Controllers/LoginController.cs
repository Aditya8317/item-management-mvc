using System;
using System.Web.Mvc;
using Round.Models;
using Round.Helpers;

namespace Round.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Item");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                User user = DbHelper.ValidateUser(model.Email, model.Password);

                if (user != null)
                {
                    Session["UserId"] = user.UserId;
                    Session["UserEmail"] = user.Email;

                    return RedirectToAction("Index", "Item");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password. Please try again.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login: " + ex.Message);
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}
