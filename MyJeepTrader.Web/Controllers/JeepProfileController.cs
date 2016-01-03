using System;
using System.Collections.Generic;
using System.IO;
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
    public class JeepProfileController : Controller
    {
        private ApplicationUserManager _userManager;

        public JeepProfileController()
        {

        }

        public JeepProfileController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        // GET: JeepProfile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllJeepProfiles()
        {
            Service service = new Service();

            var user = UserManager.FindByName(User.Identity.Name);

            JeepProfileAllJeepProfilesViewModel model = new JeepProfileAllJeepProfilesViewModel();
            model.jeepProfiles = service.GetAllJeepProfiles(user.Id);

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

        [HttpPost]
        public ActionResult CreateJeepProfile(FormCollection collection, HttpPostedFileBase fileUpload)
        {
            Service service = new Service();

            var user = UserManager.FindByName(User.Identity.Name);

            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    imageData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }

                var manufactuer = collection["Manufactuer"].ToString();
                var make = collection["Make"].ToString();
                var model = collection["Model"].ToString();
                var year = Convert.ToInt16(collection["Year"].ToString());
                var jeepImage = imageData;
                var jeepDescription = collection["JeepDescription"].ToString();
                bool primaryJeep = Convert.ToBoolean(collection["PrimaryJeep"].Split(',')[0]);

                if (primaryJeep == true)
                {
                    service.ChangePrimaryJeep(user.Id, false);
                    service.CreateJeepProfile(user.Id, manufactuer, make, model, year, jeepImage, jeepDescription, true);
                }
                else
                {
                    service.CreateJeepProfile(user.Id, manufactuer, make, model, year, jeepImage, jeepDescription, false);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ShowJeepImage(int JeepProfileId)
        {
            Service service = new Service();
            var getImage = service.GetJeepImageByJeepProfileId(JeepProfileId);

            if(getImage != null)
            {
                var stream = new MemoryStream(getImage.ToArray());
                return new FileStreamResult(stream, "image/jpg");
            }

            return View();
        }
    }
}