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
                dto.Sorting = model.Sorting;

                // save dto
                db.Pages.Add(dto);
                db.SaveChanges();

                // set tempdate message
                TempData["SuccessMessage"] = "You have added a new page";

                // redirect
                return RedirectToAction("AddPage");
            }
            
        }

    }
}