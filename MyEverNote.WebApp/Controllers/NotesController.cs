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
    public class NotesController : Controller
    {
        Manager<Note> NoteBusiness = new Manager<Note>();
        Manager<EverNoteUser> UserBusiness = new Manager<EverNoteUser>();
        Manager<Category> CategoryBusiness = new Manager<Category>();
        Manager<Comment> CommentBusiness = new Manager<Comment>();
        Manager<NoteLike> LikeBusiness = new Manager<NoteLike>();

        [MustAdmin][MustLogin]
        public ActionResult NoteIslemleri(int? userid, int? catid)
        {
            ViewBag.calljavascriptfunction = TempData["calljavascriptfunction"];

            List<Note> Notes = new List<Note>();
            if (userid == null && catid == null)
            {
                Notes = NoteBusiness.GetAll().ToList();
            }
            else if (catid == null)
            {
                EverNoteUser owner = UserBusiness.Get(k => k.Id == userid.Value);
                Notes = NoteBusiness.GetAll(k => k.OwnerId == owner.Id).ToList();
            }
            else
            {
                Category category = CategoryBusiness.Get(k => k.Id == catid.Value);
                Notes = NoteBusiness.GetAll(k => k.Category.Id == category.Id).ToList();
            }

            return View(Notes);
        }

        [MustLogin]
        public ActionResult AddorEdit(int? id, bool? adminpanel)
        {
            Note note = new Note();

            if (id != null)
            {
                note = NoteBusiness.GetById(id.Value);

                if (note == null)
                {
                    return HttpNotFound();
                }
            }

            List<SelectListItem> CategoryList = new List<SelectListItem>();
            List<Category> Categories = CacheHelper.GetCategoriesFromCache();
            foreach (Category c in Categories)
            {
                CategoryList.Add(new SelectListItem { Text = c.Title, Value = c.Id.ToString() });
            }
            ViewBag.Kategoriler = CategoryList;
            if (adminpanel != null)
            {
                ViewBag.adminpanel = adminpanel.Value;
            }
            TempData["Kategoriler"] = ViewBag.Kategoriler;

            List<SelectListItem> UserList = new List<SelectListItem>();
            List<EverNoteUser> Users = UserBusiness.GetAll().ToList();
            foreach (EverNoteUser c in Users)
            {
                UserList.Add(new SelectListItem { Text = c.Username, Value = c.Id.ToString() });
            }
            ViewBag.Userlar = UserList;
            TempData["Userlar"] = ViewBag.Userlar;

            return View(note);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MustLogin]
        public ActionResult AddorEdit(Note not)
        {
            Category nc = new Category();

            if (not.CategoryId != 0)
            {
                nc = CategoryBusiness.GetById(not.CategoryId);
                not.Category = nc;
            }

            EverNoteUser nu = new EverNoteUser();
            if (not.OwnerId == 0)
            {
                nu = CurrentSession.User;

            }

            else
            {
                nu = UserBusiness.GetById(not.OwnerId);
            }
            Note dbnote = new Note();
            not.OwnerId = nu.Id;

            //ModelState.Remove("Category.Title");
            //ModelState.Remove("Category.Description");
            if (not.Id == 0)
            {

                if (ModelState.IsValid)
                {
                    dbnote.Category = not.Category;
                    dbnote.CategoryId = not.Category.Id;

                    dbnote.LikeCount = not.LikeCount;
                    dbnote.OwnerId = nu.Id;
                    dbnote.Text = not.Text;
                    dbnote.Title = not.Title;


                    NoteBusiness.Add(dbnote);
                    ViewBag.calljavascriptfunction = "Popup()";
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    dbnote = NoteBusiness.Get(k => k.Id == not.Id);
                    dbnote.Category = nc;
                    dbnote.CategoryId = nc.Id;

                    dbnote.LikeCount = not.LikeCount;
                    dbnote.OwnerId = nu.Id;
                    dbnote.Text = not.Text;
                    dbnote.Title = not.Title;

                    NoteBusiness.Update(dbnote);
                    ViewBag.calljavascriptfunction = "Popup()";
                }
            }

            ViewBag.Kategoriler = TempData["Kategoriler"];
            ViewBag.Userlar = TempData["Userlar"];
            TempData["Kategoriler"] = ViewBag.Kategoriler;
            TempData["Userlar"] = ViewBag.Userlar;

            return View(dbnote);


        }

        [MustLogin]
        public ActionResult Delete(int? id)
        {
            List<SelectListItem> CategoryList = new List<SelectListItem>();
            List<Category> Categories = CacheHelper.GetCategoriesFromCache();
            foreach (Category c in Categories)
            {
                CategoryList.Add(new SelectListItem { Text = c.Title, Value = c.Id.ToString() });
            }
            ViewBag.Kategoriler = CategoryList;
            TempData["Kategoriler"] = ViewBag.Kategoriler;

            List<SelectListItem> UserList = new List<SelectListItem>();
            List<EverNoteUser> Users = UserBusiness.GetAll().ToList();
            foreach (EverNoteUser c in Users)
            {
                UserList.Add(new SelectListItem { Text = c.Username, Value = c.Id.ToString() });
            }
            ViewBag.Userlar = UserList;
            TempData["Userlar"] = ViewBag.Userlar;
            ViewBag.id = id.Value;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = NoteBusiness.GetById(id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [MustLogin]
        public ActionResult DeletePost(string id)
        {
            Note dnote = NoteBusiness.GetById(Convert.ToInt32(id));

            NoteBusiness.Delete(dnote);

            TempData["calljavascriptfunction"] = "Popup()";

            return RedirectToAction("NoteIslemleri");
        }

        public ActionResult ShowNote(int id)
        {
            ViewBag.calljavascriptfunction = TempData["calljavascriptfunction"];
            Note notedetail = new Note();

            if (id != 0)
            {
                notedetail = NoteBusiness.GetById(id);
                if (TempData["likeislemi"] == null)
                {
                    ViewBag.likeislemi = "false";
                }
                else
                {
                    ViewBag.likeislemi = TempData["likeislemi"];
                }
            }
            return View(notedetail);

        }

        [MustLogin]
        [HttpPost]
        public ActionResult NoteLiked(int id)
        {
            Note n = NoteBusiness.GetById(id);
            EverNoteUser suser = CurrentSession.User;
            if (n != null)
            {
                List<NoteLike> NoteLikes = LikeBusiness.GetAll(k => k.NoteId == n.Id).ToList();
                if (NoteLikes.Exists(k => k.UserId == suser.Id) == false)
                {
                    NoteLike like = new NoteLike();
                    like.Note = n;
                    like.UserId = suser.Id;
                    LikeBusiness.Add(like);
                    n.LikeCount++;
                    NoteBusiness.Update(n);
                }
                else
                {
                    NoteLike dlike = LikeBusiness.Get(k => k.UserId == suser.Id && k.Note.Id == n.Id);
                    LikeBusiness.Delete(dlike);
                    n.LikeCount--;
                    NoteBusiness.Update(n);
                }
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        [MustLogin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShowNote(Note not, string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {


                Note cnote = NoteBusiness.GetById(not.Id);
                if (cnote != null)
                {
                    Comment com = new Comment();
                    com.OwnerId = CurrentSession.User.Id;
                    com.Note = cnote;
                    com.Text = comment;
                    CommentBusiness.Add(com);
                    TempData["calljavascriptfunction"] = "Popup()";

                }
            }
            return RedirectToAction("ShowNote", new { id = not.Id });
        }

        [HttpPost]
        public ActionResult Likes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<NoteLike> notelist = LikeBusiness.GetAll(k => k.Note.Id == id).ToList();
            return Json(new { result = notelist.First() });
        }

        public ActionResult NoteLikedUsers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_PartialLikes", LikeBusiness.GetAll(k => k.Note.Id == id).ToList());
        }
    }
}