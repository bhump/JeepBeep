using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using System.IO;
using MyJeepTrader.Web.ViewModels;

namespace MyJeepTrader.Web.Controllers
{
    public class UserProfileController : Controller
    {
        // GET: UserProfile
        public ActionResult Index()
        {
            Service service = new Service();
            var userProfiles = service.GetAllUserProfiles();
            
            return View(userProfiles);
        }

        // GET: UserProfile/Details/5
        public ActionResult Details(int UserProfileId)
        {
            Service service = new Service();
            var userProfile = service.GetProfileInfoByProfileId(UserProfileId);

            UserProfileDetailsViewModel model = new UserProfileDetailsViewModel();
            model.FirstName = userProfile.FirstName;
            model.LastName = userProfile.LastName;
            model.BirthDate = userProfile.BirthDate;
            model.Facebook = userProfile.Facebook;
            model.GooglePlus = userProfile.GooglePlus;
            model.Ello = userProfile.Ello;
            model.Website = userProfile.Website;
            model.Description = userProfile.Description;
            model.UserPosts = service.GetAllPostsForUser(UserProfileId);
            model.RecentPost = service.GetUsersMostRecentPost(UserProfileId);

            return View(model);
        }

        public ActionResult ShowAvatar(int UserProfileId)
        {
            Service service = new Service();
            var getAvatar = service.GetAvatarImage(UserProfileId);

            var stream = new MemoryStream(getAvatar.ToArray());

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
    }
}
