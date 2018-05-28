using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // declare list of pages
            List<PageViewModel> pagesList;

            // init list
            using(CmsShoppingCartDb db = new CmsShoppingCartDb())
            {
                // init list
                pagesList = db.Pages.ToArray()
                    .OrderBy(p => p.Sorting)
                    .Select(p => new PageViewModel(p)).ToList();
            }

            // return
            return View(pagesList);
        }

        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PageViewModel model)
        {
            // check model state
            if( !ModelState.IsValid)
            {
                return View(model);
            }

            
            using(CmsShoppingCartDb db = new CmsShoppingCartDb())
            {
                // declare slug
                string slug;

                // init pageDTO
                PageDTO dto = new PageDTO();

                // dto title
                dto.Title = model.Title;

                // check for and set slug if need be
                if(string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The title or slug already exists");
                    return View(model);
                }

                // dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                // save dto
                db.Pages.Add(dto);
                db.SaveChanges();

                // set tempdate message
                TempData["SuccessMessage"] = "You have added a new page";

                // redirect
                return RedirectToAction("AddPage");
            }
            
        }

        // GET: admin/pages/editpage/id
        public ActionResult EditPage(int id)
        {
            // declare pagevm
            PageViewModel model;

            using(CmsShoppingCartDb db = new CmsShoppingCartDb())
            {
                // get the page
                PageDTO dto = db.Pages.Find(id);

                //confirm page exists
                if(dto == null)
                {
                    return Content("The page does not exist.");
                }

                // init page
                model = new PageViewModel(dto);
            }

             // return vm
            return View(model);
        }

        // POST: admin/pages/editpage/id
        [HttpPost]
        public ActionResult EditPage(PageViewModel vm)
        {
            // get page id
            int id = vm.Id;

            // check model state
            if(! ModelState.IsValid)
            {
                return View(vm);
            }

            using(CmsShoppingCartDb db = new CmsShoppingCartDb())
            {
                string slug = "";

                // get the page
                PageDTO dto = db.Pages.Find(id);

                dto.Title = vm.Title;

                if(vm.Slug != "home")
                {
                    if(string.IsNullOrWhiteSpace(vm.Slug))
                    {
                        slug = vm.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = vm.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // title and slug are unique
                if(db.Pages.Where(x => x.Id != id).Any(x => x.Title == vm.Title) || 
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == vm.Slug))
                {
                    ModelState.AddModelError("", "The Title or Slug already exists");
                    return View(vm);
                }

                // update dto
                dto.Slug = slug;
                dto.Body = vm.Body;
                dto.Sorting = vm.Sorting;
                dto.HasSideBar = vm.HasSideBar;

                // save dto
                db.SaveChanges();
            }

            // set tempdate
            TempData["SuccessMessage"] = "page udated!";

            return RedirectToAction("EditPage", new { Id = id});
        }

        // GET: admin/pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            // db
            using(CmsShoppingCartDb db = new CmsShoppingCartDb())
            {
                // get page
                PageDTO dto = db.Pages.Find(id);

                // remove page
                db.Pages.Remove(dto);

                // save
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}