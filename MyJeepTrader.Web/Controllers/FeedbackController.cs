using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyJeepTrader.Web.ViewModels;
using MyJeepTrader.Data;


namespace MyJeepTrader.Web.Controllers
{
    public class FeedbackController : Controller
    {
        private ApplicationUserManager _userManager;

        public FeedbackController()
        {

        }

        public FeedbackController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        // GET: Feedback
        public ActionResult Index()
        {
            return View();
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


        public ActionResult SendFeedback(FormCollection collection)
        {

            var user = UserManager.FindByName(User.Identity.Name);

            Service service = new Service();

            var scale = collection["Scale"].ToString();
            var overall = collection["Overall"].ToString();
            var description = collection["Description"].ToString();

            if (scale.Length == 0)
            {
                ModelState.AddModelError("Scale", "Please add a scale.");
            }
            else if (overall.Length == 0)
            {
                ModelState.AddModelError("Overall", "Please add an overall rating.");
            }
            else if (description.Length == 0)
            {
                ModelState.AddModelError("Description", "Please add a reason for your results. ");
            }
            else
            {
                service.CreateFeedback(Convert.ToInt32(scale), Convert.ToInt32(overall), description, user.Id);
            }

            if (ModelState.IsValid)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}