using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyJeepTrader.Data;
using MyJeepTrader.Web.Helpers;
using MyJeepTrader.Web.Models;
using MyJeepTrader.Web.ViewModels;
using System.IO;

//test comment.
namespace MyJeepTrader.Web.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
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


        // GET: Post/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Post/Create
        [Authorize]
        public ActionResult Create()
        {
            Service service = new Service();
            PostCreateViewModel model = new PostCreateViewModel
            {
                Models = service.GetAllModels().ToModelWithSelected(),
                Years = service.GetAllYears(),
                PostTypes = service.GetAllPostTypes()

            };

            return View(model);
        }

        // POST: Post/Create
        [HttpPost]
        public ActionResult Create(PostCreateViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var userId = User.Identity.GetUserId();

                Service service = new Service();
                //model.Post.MakeID = 
                //model.Post.YearID = model.Years.Where(x => x.IsSelected)
                model.Post.PostTypeId = model.SelectedPostTypeId;
                model.Post.YearId = model.SelectedYearId;
                model.Post.IsVehicle = model.IsJeep;
                model.Post.Active = true;
                model.Post.MakeId = 1; //this site is only for jeep right now

                model.Post.Id = userId;

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

                TempData["Message"] = "Post Created Successfully!";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Post/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Post/Edit/5
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

        // GET: Post/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Post/Delete/5
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
