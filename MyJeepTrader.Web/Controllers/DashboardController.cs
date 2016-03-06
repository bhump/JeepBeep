using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MyJeepTrader.Web.ViewModels;
using MyJeepTrader.Web.Models;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using System.IO;

namespace MyJeepTrader.Web.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public DashboardController()
        {
        }

        public DashboardController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
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

        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            var user = UserManager.FindByName(User.Identity.Name);

            Service service = new Service();
            var profileInfo = service.GetProfileInfo(user.Id);
            var jeepInfo = service.GetPrimaryJeepInfo(user.UserName);
            var memberInfo = service.GetMembership(user.UserName);
            var subscriptionInfo = service.GetSubscription(memberInfo.MembershipId);
            var subscriptionType = service.GetSubscriptionType(subscriptionInfo.SubscriptionTypeId);
            var settings = service.GetSettings(user.Id);

            DashboardIndexViewModel model = new DashboardIndexViewModel();

            model.UserProfileId = profileInfo == null ? 0 : profileInfo.UserProfileId;
            model.Email = user.Email;
            model.FirstName = profileInfo == null ? "" : profileInfo.FirstName;
            model.LastName = profileInfo == null ? "" : profileInfo.LastName;
            model.Description = profileInfo == null ? "" : profileInfo.Description;
            model.BirthDate = profileInfo == null ? DateTime.MinValue : Convert.ToDateTime(profileInfo.BirthDate);
            model.Facebook = profileInfo == null ? "" : profileInfo.Facebook;
            model.Twitter = profileInfo == null ? "" : profileInfo.Twitter;
            model.GooglePlus = profileInfo == null ? "" : profileInfo.GooglePlus;
            model.Website = profileInfo == null ? "" : profileInfo.Website;
            model.Ello = profileInfo == null ? "" : profileInfo.Ello;
            model.Avatar = profileInfo == null ? null : profileInfo.Avatar;
            model.SubscriptionType = subscriptionType == null ? null : subscriptionType;
            model.ExpirationDate = subscriptionInfo == null ? null : subscriptionInfo.ExpireDate;
            model.MemberSince = memberInfo == null ? null : memberInfo.MemberSince;
            model.PrivateStatusUpdates = settings.PrivateStatus;

            if (jeepInfo != null)
            {
                model.JeepProfileId = jeepInfo.VehicleProfileId;
                model.Manufactuer = jeepInfo == null ? "" : jeepInfo.Manufacturer;
                model.Make = jeepInfo == null ? "" : jeepInfo.Make;
                model.Model = jeepInfo == null ? "" : jeepInfo.Model;
                model.Year = jeepInfo == null ? Convert.ToInt16(0) : Convert.ToInt16(jeepInfo.Year);
                model.JeepDescription = jeepInfo == null ? "" : jeepInfo.Description;
                model.PrimaryJeep = jeepInfo == null ? false : Convert.ToBoolean(jeepInfo.PrimaryJeep);
                if (jeepInfo != null)
                {
                    model.Image = jeepInfo.Image;
                }
            }

            model.SentMessages = service.GetSentMessages(user.Id);
            model.Inbox = service.GetInboxMessages(user.Id);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            Service service = new Service();
            DashboardIndexViewModel model = new DashboardIndexViewModel
            {
                Users = service.GetAllUsers()
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

        public ActionResult ShowJeepImage(string UserName)
        {
            Service service = new Service();
            var getImage = service.GetJeepImage(UserName);

            if (getImage != null)
            {
                var stream = new MemoryStream(getImage.ToArray());
                return new FileStreamResult(stream, "image/jpg");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangeEmail(DashboardIndexViewModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                //if (ModelState.IsValid)
                //{
                var user = await UserManager.FindByNameAsync(User.Identity.Name);
                user.Email = model.Email;
                await UserManager.UpdateAsync(user);
                //}
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(DashboardIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(FormCollection collection)
        {
            var userId = collection["Id"] == "" ? null : collection["Id"].ToString();
            var newPassword = collection["ResetPassword"] == "" ? null : collection["ResetPassword"].ToString();

            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(userId);

            var result = await UserManager.ResetPasswordAsync(userId, resetToken, newPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Admin", new { Message = ManageMessageId.SetPasswordSuccess });
            }
            else
            {
                AddErrors(result);
            }

            return RedirectToAction("Admin", new { Message = ManageMessageId.Error});
        }

        [HttpPost]
        public ActionResult CreateUserProfile(FormCollection collection, HttpPostedFileBase fileUpload)
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

                var firstName = collection["FirstName"] == "" ? null : collection["FirstName"].ToString();
                var lastName = collection["LastName"] == "" ? null : collection["LastName"].ToString();
                var birthDate = collection["BirthDate"] == " " ? null : collection["BirthDate"].ToString();
                var avatar = imageData.Length ==  0 ? null : imageData;
                var description = collection["Description"] == "" ? null : collection["Description"].ToString();
                var facebook = collection["Facebook"] == "" ? null : collection["Facebook"].ToString();
                var twitter = collection["Twitter"] == "" ? null : collection["Twitter"].ToString();
                var ello = collection["Ello"] == "" ? null : collection["Ello"].ToString();
                var googlePlus = collection["GooglePlus"] == "" ? null : collection["GooglePlus"].ToString();
                var website = collection["Website"] == "" ? null : collection["Website"].ToString();

                var convertBirthDate = birthDate == null ? DateTime.MinValue : Convert.ToDateTime(birthDate);

                if (service.CheckForProfile(user.Id) == false)
                {
                    service.CreateProfile(user.Id, firstName, lastName, convertBirthDate, avatar, description, facebook, twitter, ello, googlePlus, website);
                }
                else
                {
                    service.UpdateProfile(user.Id, firstName, lastName, convertBirthDate, avatar, description, facebook, twitter, ello, googlePlus, website);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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

                var manufactuer = collection["Manufactuer"] == "" ? null : collection["Manufactuer"].ToString();
                var make = collection["Make"] == "" ? null : collection["Make"].ToString();
                var model = collection["Model"] == "" ? null : collection["Model"].ToString();
                var year = collection["Year"] == " " ? null : collection["Year"];
                var jeepImage = imageData.Length == 0 ? null : imageData;
                var jeepDescription = collection["JeepDescription"].ToString();
                //bool primaryJeep = Convert.ToBoolean(collection["PrimaryJeep"].Split(',')[0]);

                var convertYear = year == null ? Convert.ToInt16(0) : Convert.ToInt16(year);

                if (service.CheckForPrimaryJeep(user.Id) == false)
                {
                    service.CreateJeepProfile(user.Id, manufactuer, make, model, convertYear, jeepImage, jeepDescription, true);
                }
                else
                {
                    service.UpdatePrimaryJeepProfile(user.Id, manufactuer, make, model, convertYear, jeepImage, jeepDescription, true);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateMessage(FormCollection collection)
        {
            var user = UserManager.FindByName(User.Identity.Name);

            try
            {
                Service service = new Service();

                var to = collection["To"].ToString();
                var subject = collection["subject"].ToString();
                var message = collection["Message"].ToString();
                var isIm = false;

                service.CreateMessage(to, user.UserName, subject, message, isIm);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateReadMessage(string MessageId)
        {
            try
            {
                Service service = new Service();
                int messageId = Convert.ToInt32(MessageId);

                service.MarkMessageAsRead(messageId);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateSettings(string privateOnly)
        {
            try
            {
                Service service = new Service();

                var userId = User.Identity.GetUserId();

                service.UpdateSettings(userId, Convert.ToBoolean(privateOnly));

                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Settings Saved Successfully!";

                    return View();
                }
            }
            catch
            {
                TempData["Message"] = "Something went wrong! Unable to save settings! ";
                return View();
            }

            TempData["Message"] = "Something went wrong! Unable to save settings! ";
            return View();
        }

        public ActionResult Cancel()
        {
            Service service = new Service();
            PayPalService payPalService = new PayPalService();

            var user = UserManager.FindByName(User.Identity.Name);
            var userInfo = service.GetProfileInfo(user.Id);
            var memberInfo = service.GetMembership(user.UserName);
            var subscriptionInfo = service.GetSubscription(memberInfo.MembershipId);
            DateTime startDate = DateTime.Now;

            if (subscriptionInfo.PayPalSubscriptionId != null)
            {
                payPalService.PayPalCancelSubscription(subscriptionInfo.PayPalSubscriptionId);
                service.InactivateSubscription(subscriptionInfo.SubscriptionId);
                service.CreateSubscription(memberInfo.MembershipId, startDate, startDate.AddYears(100), ConstantStrings.freeSubscription, null);
            }

            return RedirectToAction("Index");

        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}