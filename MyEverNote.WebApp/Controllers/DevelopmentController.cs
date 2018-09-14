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
    }
}