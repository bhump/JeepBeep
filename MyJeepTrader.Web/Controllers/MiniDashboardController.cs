using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using MyJeepTrader.Web.Helpers;
using MyJeepTrader.Web.Models;
using MyJeepTrader.Web.ViewModels;

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
    }
}