using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using MyJeepTrader.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MyJeepTrader.Web.Controllers
{
    public class HomeController : Controller
    {
        [RequireHttps]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View();
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

        public ActionResult AlphaTesting()
        {
            return View();
        }

        public ActionResult LiveStream()
        {
            Service service = new Service();
            LiveStreamViewModel model = new LiveStreamViewModel
            {
                LiveStreams = service.GetLiveStream(),
                LivePosts = service.GetLivePosts()
            };
            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"]; ViewBag.Header = "Success!";

            return View(model);
        }
    }
}