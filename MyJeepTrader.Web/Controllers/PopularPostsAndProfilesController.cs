using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using MyJeepTrader.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MyJeepTrader.Web.Controllers
{
    public class PopularPostsAndProfilesController : Controller
    {
        // GET: PopularPostsAndProfiles
        public ActionResult PopularPostsandProfiles()
        {
            Service service = new Service();

            var userId = User.Identity.GetUserId();
            var popularProfiles = service.GetPopularProfiles();
            var getFriends = service.GetFriends(userId);
            var pendingFriends = service.GetUsersPending(userId);

            PopularPostsAndProfilesViewModel model = new PopularPostsAndProfilesViewModel(popularProfiles, getFriends, pendingFriends);

            return View("~/Views/Shared/_PopularPostsandProfiles.cshtml", model);
        }
    }
}