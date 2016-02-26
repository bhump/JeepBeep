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
        public async Task<ActionResult> CreateStatus(FormCollection collection)
        {
            try
            {
                var userId = User.Identity.GetUserId();

                Service service = new Service();

                var status  = collection["status"].ToString();

                var statusId = await service.CreateStatusAsync(userId, status);

                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Status Created Successfully!";

                    return RedirectToAction("LiveStream", "Home");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine("------------ " + ex);

                TempData["Message"] = "Something went wrong with posting your new status! Try again!";

                return RedirectToAction("LiveStream", "Home");
            }

            TempData["Message"] = "Something went wrong! Unable to post a new status!";
            return View(TempData);
        }
    }
}