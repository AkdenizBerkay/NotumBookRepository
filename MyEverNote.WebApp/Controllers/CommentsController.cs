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
    public class CommentsController : Controller
    {
        Manager<Comment> CommentBusiness = new Manager<Comment>();
        Manager<EverNoteUser> UserBusiness = new Manager<EverNoteUser>();
        Manager<CommentLike> CommentLikedBusiness = new Manager<CommentLike>();
        Manager<Note> NoteBusiness = new Manager<Note>();

        [MustAdmin]
        public ActionResult CommentIslemleri(int? userid, int? noteid)
        {
            ViewBag.calljavascriptfunction = TempData["calljavascriptfunction"];

            List<Comment> Comments = new List<Comment>();
            if (userid == null && noteid == null)
            {
                Comments = CommentBusiness.GetAll().ToList();
            }
            else if (noteid == null)
            {
                EverNoteUser owner = UserBusiness.Get(k => k.Id == userid.Value);
                Comments = CommentBusiness.GetAll(k => k.OwnerId == owner.Id).ToList();
            }
            else
            {
                Note note = NoteBusiness.Get(k=> k.Id == noteid.Value);
                Comments = CommentBusiness.GetAll(k => k.NoteId == note.Id).ToList();
            }

            return View(Comments);
        }

        [MustLogin]
        public ActionResult AddorEdit(int? id)
        {
            Comment com = new Comment();

            if (id != null)
            {
                com = CommentBusiness.GetById(id.Value);

                if (com == null)
                {
                    return HttpNotFound();
                }
            }

            List<SelectListItem> NoteList = new List<SelectListItem>();
            List<Note> Notes = NoteBusiness.GetAll().ToList();
            foreach (Note c in Notes)
            {
                NoteList.Add(new SelectListItem { Text = c.Title, Value = c.Id.ToString() });
            }
            ViewBag.Notelar = NoteList;
            TempData["Notelar"] = ViewBag.Notelar;

            List<SelectListItem> UserList = new List<SelectListItem>();
            List<EverNoteUser> Users = UserBusiness.GetAll().ToList();
            foreach (EverNoteUser c in Users)
            {
                UserList.Add(new SelectListItem { Text = c.Username, Value = c.Id.ToString() });
            }
            ViewBag.Userlar = UserList;
            TempData["Userlar"] = ViewBag.Userlar;

            return View(com);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MustAdmin]
        public ActionResult AddorEdit(Comment com)
        {
            Note cn = NoteBusiness.GetById(com.Note.Id);

            EverNoteUser nu = new EverNoteUser();
            if (com.OwnerId == 0)
            {
                nu = CurrentSession.User;
            }

            else
            {
                nu = UserBusiness.GetById(com.OwnerId);
            }
            Comment dbcom = new Comment();
            ModelState.Remove("Note.Text");
            ModelState.Remove("Note.Title");
            if (com.Id == 0)
            {
                
                if (ModelState.IsValid)
                {
                    dbcom.Note = cn;
                    dbcom.NoteId = cn.Id;

                    dbcom.LikeCount = com.LikeCount;
                    dbcom.OwnerId = nu.Id;
                    dbcom.Text = com.Text;


                    CommentBusiness.Add(dbcom);
                    ViewBag.calljavascriptfunction = "Popup()";
                }
            }
            else
            {

                if (ModelState.IsValid)
                {
                    dbcom = CommentBusiness.Get(k => k.Id == com.Id);
                    dbcom.NoteId = cn.Id;

                    dbcom.LikeCount = com.LikeCount;
                    dbcom.OwnerId = nu.Id;
                    dbcom.Text = com.Text;

                    CommentBusiness.Update(dbcom);
                    ViewBag.calljavascriptfunction = "Popup()";
                }
            }

            ViewBag.Notelar = TempData["Notelar"];
            ViewBag.Userlar = TempData["Userlar"];
            TempData["Notelar"] = ViewBag.Notelar;
            TempData["Userlar"] = ViewBag.Userlar;

            return View(dbcom);


        }
        [MustLogin]
        public ActionResult Delete(int? id)
        {
            List<SelectListItem> NoteList = new List<SelectListItem>();
            List<Note> Notes = NoteBusiness.GetAll().ToList();
            foreach (Note c in Notes)
            {
                NoteList.Add(new SelectListItem { Text = c.Title, Value = c.Id.ToString() });
            }
            ViewBag.Notelar = NoteList;
            TempData["Notelar"] = ViewBag.Notelar;

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
            Comment com = CommentBusiness.GetById(id.Value);
            if (com == null)
            {
                return HttpNotFound();
            }
            return View(com);
        }
        [MustLogin]
        public ActionResult DeletePost(string id)
        {
            Comment dcom = CommentBusiness.GetById(Convert.ToInt32(id));

            CommentBusiness.Delete(dcom);

            TempData["calljavascriptfunction"] = "Popup()";

            return RedirectToAction("CommentIslemleri");
        }


        public ActionResult ShowModalComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.id = TempData["commentid"];
            List<Comment> Comments = new List<Comment>();
            if (ViewBag.id != null)
            {
                Comment c = CommentBusiness.GetById(id.Value);
                Comments = CommentBusiness.GetAll(k => k.Note.Id == c.Note.Id).ToList();
                return PartialView("_PartialComments", Comments);
            }
            else
            {
                Comments = CommentBusiness.GetAll(k => k.Note.Id == id).ToList();
                return PartialView("_PartialComments", Comments);
            }
        }
       
        [HttpPost]
        [MustLogin]
        public ActionResult UnLikeComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentLike clike = CommentLikedBusiness.Get(k => k.Comment.Id == id.Value);
            Comment c = new Comment();
            if (clike != null)
            {
                CommentLikedBusiness.Delete(clike);
                c = CommentBusiness.GetById(id.Value);
                if (c != null)
                {
                    c.LikeCount--;
                    CommentBusiness.Update(c);
                    TempData["commentid"] = id;
                }
            }
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [MustLogin]
        public ActionResult LikeComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentLike clike = new CommentLike();
            Comment c = CommentBusiness.GetById(id.Value);
            clike.Comment = c;
            clike.EverNoteUser = CurrentSession.User;
            CommentLikedBusiness.Add(clike);
            c.LikeCount++;

            CommentBusiness.Update(c);

            TempData["commentid"] = id;
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        [MustLogin]
        public ActionResult CommentEdit(int id,string txt)
        {
            Comment dbcom = CommentBusiness.Get(k => k.Id == id);

            if (dbcom != null)
            {
                dbcom.Text = txt;
                CommentBusiness.Update(dbcom);
            }
            TempData["calljavascriptfunction"] = "CommentPopup()";
            return RedirectToAction("ShowNote", "Notes", new { id = dbcom.Note.Id });
        }
        [MustLogin]
        public ActionResult CommentDelete(int id)
        {
            Comment dbcom = CommentBusiness.Get(k => k.Id == id);
            int noteid = dbcom.Note.Id;
            if (dbcom != null)
            {
                CommentBusiness.Delete(dbcom);
            }
            TempData["calljavascriptfunction"] = "CommentPopup()";
            return RedirectToAction("ShowNote", "Notes", new { id = noteid });
        }

        public ActionResult CommentLikedUsers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_PartialCommentLikes", CommentLikedBusiness.GetAll(k => k.Comment.Id == id).ToList());
        }
    }
}