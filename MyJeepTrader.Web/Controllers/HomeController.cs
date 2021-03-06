﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using MyJeepTrader.Web.Helpers;
using MyJeepTrader.Web.Models;
using MyJeepTrader.Web.ViewModels;
using System.IO;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyJeepTrader.Data.Models;

namespace MyJeepTrader.Web.Controllers
{

    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

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

        [RequireHttps]
        public ActionResult Index()
        {
            PopularPostsAndProfilesViewModel model = new PopularPostsAndProfilesViewModel();

            Service service = new Service();
            model.PopularPosts = service.GetPopularUserPost();

            return View(model);
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

        [Authorize]
        public ActionResult LiveStream()
        {
            Service service = new Service();

            var userId = User.Identity.GetUserId();

            ICollection<LivePost> livePosts = service.GetLivePosts();
            ICollection<LiveStream> liveStreams = service.GetLiveStream(userId);
            var settings = service.GetSettings(userId);
            var jeepModels = service.GetAllJeepModels();

            LiveStreamViewModel model = new LiveStreamViewModel(livePosts, liveStreams)
            {
                Settings = settings,
                Models = jeepModels
            };

            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"]; ViewBag.Header = "Success!";

            return View(model);
        }

        public ActionResult ShowAvatar(string UserName)
        {
            Service service = new Service();
            var getAvatar = service.GetAvatarImage(UserName);

            var stream = new MemoryStream(getAvatar.ToArray());

            return new FileStreamResult(stream, "image/jpg");
        }

        [Authorize]
        public ActionResult Discover()
        {
            Service service = new Service();

            ICollection<LiveStream> publicStream = service.GetPublicStream();

            LiveStreamViewModel model = new LiveStreamViewModel(publicStream);

            return View(model);
        }

        //[HttpPost]
        //public ActionResult AddFriend(string friendName)
        //{
        //    Service service = new Service();

        //    var friendId = UserManager.FindByName(friendName);
        //    var userId = User.Identity.GetUserId();

        //    service.AddFriend(userId, friendId.Id);

        //    return View();
        //}
    }
}