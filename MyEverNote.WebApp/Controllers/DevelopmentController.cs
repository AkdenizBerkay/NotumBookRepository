using MyEverNote.BusinessLayer;
using MyEverNote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEverNote.WebApp.Controllers
{
    public class DevelopmentController : Controller
    {
        // GET: Development
        public ActionResult MvcGrid()
        {
            List<Note> Notes = new List<Note>();
            Manager<Note> NoteBusiness = new Manager<Note>();
            Notes = NoteBusiness.GetAll().ToList();
            return View(Notes);
        }

        public ActionResult AddorEdit(int? id, bool? adminpanel)
        {
            List<Note> Notes = new List<Note>();
            Manager<Note> NoteBusiness = new Manager<Note>();
            Notes = NoteBusiness.GetAll().ToList();
            return View(Notes);
        }

        public ActionResult MvcGrid2()
        {
            return View();
        }

        public JsonResult CategoryDropBox()
        {
            Manager<Category> CategoryBusiness = new Manager<Category>();
            List<Category> categories = CategoryBusiness.GetAll().ToList();
            List<string> titles = new List<string>();
            foreach (Category cat in categories)
            {
                titles.Add(cat.Title);
            }
            return Json(titles);
        }
    }
}