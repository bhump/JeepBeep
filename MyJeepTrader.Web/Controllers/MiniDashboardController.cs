using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyJeepTrader.Data;
using MyJeepTrader.Web.Helpers;
using MyJeepTrader.Web.Models;
using MyJeepTrader.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.IO;
using System.Data.Entity.Validation;
using MyJeepTrader.Web.ImageResize;

namespace MyJeepTrader.Web.Controllers
{
    public class MiniDashboardController : Controller
    {
        // GET: MiniDashboard
        public ActionResult MiniDashboardPartial()
        {
            Service service = new Service();
            PostCreateViewModel model = new PostCreateViewModel
            {
                MiniDashModel = service.GetAllModels(),
                Years = service.GetAllYears(),
                PostTypes = service.GetAllPostTypes(),
                States = service.GetAllStates()
            };

            return View("~/Views/Shared/_MiniDashboardPartial.cshtml", model);
        }

        public ActionResult TimelineSettingsPartial()
        {
            Service service = new Service();

            var userId = User.Identity.GetUserId();

            LiveStreamViewModel model = new LiveStreamViewModel
            {
                Settings = service.GetSettings(userId),
                Models = service.GetAllJeepModels()
            };

            return View("~/Views/Shared/_TimelineSettingsPartial.cshtml", model);
        }

        [HttpPost]
        public ActionResult Create(PostCreateViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var userId = User.Identity.GetUserId();

                ResizeImage resizer = new ResizeImage();

                Service service = new Service();
                model.Post.PostTypeId = model.SelectedPostTypeId;
                model.Post.YearId = model.SelectedYearId;
                model.Post.PostTitle = model.Title;
                model.Post.Active = true;
                model.Post.MakeId = 1;
                model.Post.StateId = model.SelectedStateId;
                model.Post.CityId = model.SelectedCityId;
                model.Post.Id = userId;

                if (model.SelectedPostTypeId == 0)
                {
                    ModelState.AddModelError("", "Please select a post type. ");
                }
                else if (model.SelectedStateId == 0)
                {
                    ModelState.AddModelError("", "Please select a model/accessory year. ");
                }
                else if (model.Title == null)
                {
                    ModelState.AddModelError("", "Please add a post title. ");
                }
                else if (model.SelectedStateId == 0)
                {
                    ModelState.AddModelError("", "Please select a state. ");
                }
                else if (model.SelectedCityId == 0)
                {
                    ModelState.AddModelError("", "Please select a city. ");
                }
                else if (model.Post.PostDescription == null)
                {
                    ModelState.AddModelError("", "Please add a description.");
                }
                else if (userId == null)
                {
                    ModelState.AddModelError("", "Please log in.");
                }
                else
                {
                    var newPostId = service.CreateNewPost(model.Post);

                    if (model.Models == null)
                    {
                        service.AddModelPost(model.SelectedModelId, newPostId);
                    }
                    else
                    {

                        foreach (var selectedModel in model.Models.Where(x => x.IsSelected))
                        {
                            service.AddModelPost(selectedModel.TModel.ModelId, newPostId);
                        }
                    }

                    if (files.Count() != 0)
                    {
                        foreach (var img in files)
                        {
                            if (img != null)
                            {
                                byte[] imageBytes = null;

                                using (var binaryReader = new BinaryReader(img.InputStream))
                                {
                                    imageBytes = binaryReader.ReadBytes(img.ContentLength);
                                }

                                service.AddImage(imageBytes, newPostId);
                            }
                        }
                    }

                }

                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Post Created Successfully!";

                    return RedirectToAction("LiveStream", "Home");
                }

                return View(model);
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

                return View();
            }
        }
    }
}