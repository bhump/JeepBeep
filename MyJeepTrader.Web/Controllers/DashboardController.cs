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
            DasbhoardIndexViewModel model = new DasbhoardIndexViewModel
            {
                Username = User.Identity.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUserInfo(DasbhoardIndexViewModel model)
        {
            using(var context = new ApplicationDbContext())
            {
                if(ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(User.Identity.Name);
                    user.UserName = model.Username;
                    await UserManager.UpdateAsync(user);
                }
            }
            //i hate mvc.
            return RedirectToAction("Index");
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