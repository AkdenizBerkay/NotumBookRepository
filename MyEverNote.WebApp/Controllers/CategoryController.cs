using MyEverNote.BusinessLayer;
using MyEverNote.Entities;
using MyEverNote.WebApp.Filters;
using MyEverNote.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEverNote.WebApp.Controllers
{
    [MustAdmin]
    public class CategoryController : Controller
    {
        Manager<Category> CategoryBusiness = new Manager<Category>();
        
        public ActionResult CategoryIslemleri()
        {
            List<Category> Categories = CategoryBusiness.GetAll().ToList();
            return View(Categories);
        }

        public ActionResult AddorEdit(int? id)
        {
            Category category = new Category();

            if (id != null)
            {
                category = CategoryBusiness.GetById(id.Value);

                if (category == null)
                {
                    return HttpNotFound();
                }
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddorEdit(Category category)
        {
            Category dbcategory = new Category();

            if (ModelState.IsValid)
            {
                if (category.Id != 0)
                {
                    dbcategory = CategoryBusiness.Get(k => k.Id == category.Id);

                    dbcategory.Description = category.Description;
                    dbcategory.Title = category.Title;

                    CategoryBusiness.Update(dbcategory);
                    CacheHelper.RemoveCategoriesFromCache();
                }
                else
                {
                    dbcategory.Description = category.Description;
                    dbcategory.Title = category.Title;

                    CategoryBusiness.Add(dbcategory);
                    CacheHelper.RemoveCategoriesFromCache();
                }

                ViewBag.calljavascriptfunction = "Popup()";
                return View(dbcategory);

            }
            else
            {
                return View(category);
            }
        }

        public ActionResult Delete(int? id)
        {
            ViewBag.id = id.Value;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category cat = CategoryBusiness.GetById(id.Value);
            if (cat == null)
            {
                return HttpNotFound();
            }
            return View(cat);
        }

        public ActionResult DeletePost(string id)
        {
            Category dcat = CategoryBusiness.GetById(Convert.ToInt32(id));

            CategoryBusiness.Delete(dcat);

            CacheHelper.RemoveCategoriesFromCache();

            TempData["calljavascriptfunction"] = "Popup()";

            return RedirectToAction("CategoryIslemleri");
        }
    }
}