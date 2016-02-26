using System;
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

//test comment.
namespace MyJeepTrader.Web.Controllers
{
    public class PostController : Controller
    {
        private ApplicationUserManager _userManager;

        public PostController()
        {

        }

        public PostController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        public ActionResult Index()
        {
            Service service = new Service();
            PostIndexViewModel model = new PostIndexViewModel
            {
                Posts = service.GetAllPosts()
            };

            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"]; ViewBag.Header = "Success!";
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchPosts(string search)
        {
            Service service = new Service();
            PostIndexViewModel model = new PostIndexViewModel
            {
                SearchedPosts = service.SearchAllPosts(search)
            };

            return View("Index", model);
        }

        public ActionResult GetPostImages(int postId)
        {
            Service service = new Service();

            var images = service.GetPostImages(postId);

            foreach (var image in images)
            {
                var stream = new MemoryStream(image.ToArray());

                return File(stream, "image/jpg");
            }

            return null;
        }

        public ActionResult Details(int postId)
        {
            Service service = new Service();
            var post = service.GetPostByPostId(postId);
            post.Images = service.GetPostImages(postId);

            return View(post);
        }

        [Authorize]
        public ActionResult Create()
        {
            Service service = new Service();
            PostCreateViewModel model = new PostCreateViewModel
            {
                Models = service.GetAllModels().ToModelWithSelected(),
                Years = service.GetAllYears(),
                PostTypes = service.GetAllPostTypes(),
                States = service.GetAllStates()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GetCityByStateId(int StateId)
        {
            Service service = new Service();
            var cities = service.GetCityByStateId(StateId);
            var json = Json(cities);

            return json;
        }

        [HttpPost]
        public ActionResult GetStateNameById(int StateId)
        {
            Service service = new Service();
            var state = service.GetStateById(StateId);

            var json = Json(state);

            return json;
        }

        [HttpPost]
        public ActionResult GetYearById(int YearId)
        {
            Service service = new Service();
            var year = service.GetYearById(YearId);

            var json = Json(year);

            return json;
        }

        [HttpPost]
        public ActionResult GetModelNameById(List<int> modelIds)
        {
            Service service = new Service();

            foreach (int id in modelIds)
            {
                var models = service.GetModelById(id);

                var json = Json(models);

                return json;
            }

            return null;
        }

        [HttpPost]
        public ActionResult Create(PostCreateViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var userId = User.Identity.GetUserId();

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

                    foreach (var selectedModel in model.Models.Where(x => x.IsSelected))
                    {
                        service.AddModelPost(selectedModel.TModel.ModelId, newPostId);
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

                    return RedirectToAction("Index");
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

        [HttpPost]
        public ActionResult CreateMessage(FormCollection collection)
        {
            var user = UserManager.FindByName(User.Identity.Name);

            try
            {
                Service service = new Service();

                var postId = collection["PostId"].ToString();
                var to = collection["UserName"].ToString();
                var subject = collection["PostTitle"].ToString();
                var message = collection["Message"].ToString();
                var isIm = false;

                service.CreateMessage(to, user.UserName, subject, message, isIm);

                return RedirectToAction("Details", new { PostId = postId });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

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

        public ActionResult Delete(int id)
        {
            return View();
        }

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
