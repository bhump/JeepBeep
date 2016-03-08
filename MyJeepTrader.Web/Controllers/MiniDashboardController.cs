using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using MyJeepTrader.Web.Helpers;
using MyJeepTrader.Web.Models;
using MyJeepTrader.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace MyJeepTrader.Web.Controllers
{
    public class MiniDashboardController : Controller
    {
        // GET: MiniDashboard
        public ActionResult MiniDashboardPartial()
        {
            Service service = new Service();
            PostCreateViewModel model = new PostCreateViewModel
            {
                MiniDashModel = service.GetAllModels(),
                Years = service.GetAllYears(),
                PostTypes = service.GetAllPostTypes(),
                States = service.GetAllStates()
            };

            //return PartialView("~/Views/Shared/_MiniDashboardPartial.cshtml", model);
            return View("~/Views/Shared/_MiniDashboardPartial.cshtml", model);
        }

        public ActionResult TimelineSettingsPartial()
        {
            Service service = new Service();

            var userId = User.Identity.GetUserId();

            LiveStreamViewModel model = new LiveStreamViewModel
            {
                Settings = service.GetSettings(userId),
                Models = service.GetAllJeepModels()
            };

            return View("~/Views/Shared/_TimelineSettingsPartial.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateSettings(string posts, string status)
        {
            try
            {
                Service service = new Service();

                var userId = User.Identity.GetUserId();

                service.UpdateSettings(userId, Convert.ToBoolean(posts),Convert.ToBoolean(status));

                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Settings Saved Successfully!";

                    return RedirectToAction("LiveStream", "Home");
                }
            }
            catch
            {
                TempData["Message"] = "Something went wrong! Unable to save settings! ";
                return RedirectToAction("LiveStream", "Home");
            }

            TempData["Message"] = "Something went wrong! Unable to save settings! ";
            return RedirectToAction("LiveStream", "Home");
        }
    }
}