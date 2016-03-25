using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyJeepTrader.Data;
using MyJeepTrader.Web.ViewModels;

namespace MyJeepTrader.Web.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            Service service = new Service();

            NotificationViewModel model = new NotificationViewModel();
            model.Notifications = service.GetNotifications(User.Identity.GetUserId());
            return View(model);
        }
    }
}