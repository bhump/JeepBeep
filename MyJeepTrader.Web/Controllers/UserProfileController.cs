using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyJeepTrader.Data;
using System.IO;
using MyJeepTrader.Web.ViewModels;

namespace MyJeepTrader.Web.Controllers
{
    public class UserProfileController : Controller
    {
        private ApplicationUserManager _userManager;

        // GET: UserProfile
        public ActionResult Index()
        {
            Service service = new Service();
            var userProfiles = service.GetAllUserProfiles();
            
            return View(userProfiles);
        }

        // GET: UserProfile/Details
        public ActionResult Details(int UserProfileId, int JeepProfileId)
        {
            var user = UserManager.FindByName(User.Identity.Name);

            Service service = new Service();
            var userProfile = service.GetProfileInfoByProfileId(UserProfileId);
            var jeepProfile = service.GetPrimaryJeepInfoByProfileId(JeepProfileId);
            service.UpdateViewCount(UserProfileId);

            UserProfileDetailsViewModel model = new UserProfileDetailsViewModel();
            model.FirstName = userProfile.FirstName;
            model.LastName = userProfile.LastName;
            model.BirthDate = userProfile.BirthDate;
            model.Facebook = userProfile.Facebook;
            model.GooglePlus = userProfile.GooglePlus;
            model.Ello = userProfile.Ello;
            model.Website = userProfile.Website;
            model.Description = userProfile.Description;
            model.ViewCount = userProfile.ViewCount == null ? 0 : userProfile.ViewCount;

            model.UserPosts = service.GetAllPostsForUser(UserProfileId);
            model.RecentPost = service.GetUsersMostRecentPost(UserProfileId);

            model.Year = jeepProfile.Year;
            model.JeepDescription = jeepProfile.Description;
            model.Make = jeepProfile.Make;
            model.Model = jeepProfile.Model;

            return View(model);
        }

        public ActionResult ShowAvatar(int UserProfileId)
        {
            Service service = new Service();
            var getAvatar = service.GetAvatarImage(UserProfileId);

            var stream = new MemoryStream(getAvatar.ToArray());

            return new FileStreamResult(stream, "image/jpg");
        }

        public ActionResult ShowJeepImage(int JeepProfileId)
        {
            Service service = new Service();
            var getImage = service.GetJeepImage(JeepProfileId);

            var stream = new MemoryStream(getImage.ToArray());

            return new FileStreamResult(stream, "image/jpg");
        }

        // GET: UserProfile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfile/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserProfile/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserProfile/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserProfile/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserProfile/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
    }
}
