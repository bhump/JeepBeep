using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Web.ViewModels;
using MyJeepTrader.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyJeepTrader.Data.Models;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MyJeepTrader.Web.Controllers
{
    public class StatusController : Controller
    {
        // GET: LiveStream
        public ActionResult StatusPostSearchPartial()
        {
            return View("~/Views/Shared/_StatusPostSearchPartial.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> CreateStatus(FormCollection collection, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var mentionedUsers = new List<string>();
                var images = new List<byte[]>();
                var userId = User.Identity.GetUserId();
                var status = collection["status"].ToString();


                string mentionPattern = @"@\w* ";
                MatchCollection matches = Regex.Matches(status, mentionPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

                Service service = new Service();

                if (matches != null)
                {
                    foreach (Match match in matches)
                    {
                        var mentionedUserName = match.ToString().Replace("@", "");
                        mentionedUsers.Add(mentionedUserName);
                    }
                }

                foreach (var img in files)
                {
                    if (img != null)
                    {
                        if (img.ContentLength > 500000)
                        {
                            TempData["Message"] = "Something went wrong! Image too big!";

                            return RedirectToAction("LiveStream", "Home");
                        }

                        byte[] imageBytes = null;

                        using (var binaryReader = new BinaryReader(img.InputStream))
                        {
                            imageBytes = binaryReader.ReadBytes(img.ContentLength);
                            images.Add(imageBytes);
                        }
                    }
                }

                var statusId = await service.CreateStatusAsync(userId, status, images, mentionedUsers);


                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Status Created Successfully!";

                    return RedirectToAction("LiveStream", "Home");
                }
            }
            catch (HttpException hex)
            {
                Debug.WriteLine("------------ " + hex);

                TempData["Message"] = "Something went wrong! Looks like the image is too big! Try a smaller image size!";

                return RedirectToAction("LiveStream", "Home");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("------------ " + ex);

                TempData["Message"] = "Something went wrong with posting your new status! Try again!";

                return RedirectToAction("LiveStream", "Home");
            }

            TempData["Message"] = "Something went wrong! Unable to post a new status!";
            return View(TempData);
        }
    }
}