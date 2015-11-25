using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MyJeepTrader.Web.ViewModels;
using MyJeepTrader.Web.Models;
using System.Threading.Tasks;

namespace MyJeepTrader.Web.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public DashboardController()
        {
        }

        public DashboardController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            var user = UserManager.FindByName(User.Identity.Name);

            DashboardIndexViewModel model = new DashboardIndexViewModel
            {
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeEmail(DashboardIndexViewModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                //if (ModelState.IsValid)
                //{
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                user.Email = model.Email;
                await UserManager.UpdateAsync(user);
                //}
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(DashboardIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateUserProfile(FormCollection collection)
        {

            var user = UserManager.FindByName(User.Identity.Name);
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                var firstName = collection["FirstName"].ToString();
                var lastName = collection["LastName"].ToString();
                var birthDate = Convert.ToDateTime(collection["BirthDate"].ToString());
                var avatar = collection["Avatar"].ToString();
                var description = collection["Description"].ToString();
                var facebook = collection["Facebook"].ToString();
                var twitter = collection["Twitter"].ToString();
                var ello = collection["Ello"].ToString();
                var googlePlus = collection["GooglePlus"].ToString();
                var website = collection["Website"].ToString();

                Service service = new Service();
                service.CreateProfile(user.Id, firstName, lastName, birthDate, description, facebook, twitter, ello, googlePlus, website);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}