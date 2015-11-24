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
        private ApplicationUserManager _userManager;

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
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            //AddErrors(result);
            return View(model);
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
    }
}