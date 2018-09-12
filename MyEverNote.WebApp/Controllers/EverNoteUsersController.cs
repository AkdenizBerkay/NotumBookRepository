using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MyEverNote.BusinessLayer;
using MyEverNote.Common.Helper;
using MyEverNote.Entities;
using MyEverNote.WebApp.Filters;

namespace MyEverNote.WebApp.Controllers
{
    public class EverNoteUsersController : Controller
    {
        Manager<Note> NoteBusiness = new Manager<Note>();
        Manager<EverNoteUser> UserBusiness = new Manager<EverNoteUser>();
        Manager<Category> CategoryBusiness = new Manager<Category>();
        Manager<Comment> CommentBusiness = new Manager<Comment>();
        Manager<NoteLike> LikeBusiness = new Manager<NoteLike>();

        [MustAdmin]
        public ActionResult UserIslemleri()
        {
            ViewBag.calljavascriptfunction = TempData["calljavascriptfunction"];

            List<EverNoteUser> Users = UserBusiness.GetAll().ToList();
            return View(Users);
        }

        [MustLogin]
        public ActionResult AddorEdit(int? id)
        {
            EverNoteUser user = new EverNoteUser();

            if (id != null)
            {
                user = UserBusiness.GetById(id.Value);

                if (user == null)
                {
                    return HttpNotFound();
                }
            }
            return View(user);
        }

        [MustLogin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddorEdit(EverNoteUser user, HttpPostedFileBase profileImage,bool SendMail=false)
        {
            EverNoteUser dbuser = new EverNoteUser();


            Validation<EverNoteUser> validate = new Validation<EverNoteUser>();

            if (ModelState.IsValid)
            {
                if (!validate.IsUnique(user, "username", true))
                {
                    ModelState.AddModelError("username", "Kullanıcı adı mevcut");

                    return View(user);
                }
                else if (!validate.IsUnique(user, "email", true))
                {
                    ModelState.AddModelError("email", "Email adresi mevcut");
                    return View(user);
                }
                else
                {
                    if (profileImage != null && (profileImage.ContentType.Equals("image/jpeg") || profileImage.ContentType.Equals("image/png") || profileImage.ContentType.Equals("image/jpg")))
                    {
                        string filename = $"user_{user.Username}.{profileImage.ContentType.Split('/')[1]}";
                        profileImage.SaveAs(Server.MapPath($"D:/vhosts/notumbook.site/httpdocs/Images/{filename}"));
                        user.ProfileImage = filename;
                    }
                    if (user.Id != 0)
                    {
                        dbuser = UserBusiness.Get(k => k.Id == user.Id);

                        dbuser.Email = user.Email;
                        dbuser.Name = user.Name;
                        dbuser.Password = user.Password;
                        dbuser.ProfileImage = profileImage != null ? user.ProfileImage : dbuser.ProfileImage;
                        dbuser.Surname = user.Surname;
                        dbuser.Username = user.Username;
                        dbuser.ActivateGuid = user.ActivateGuid;
                        dbuser.IsActive = user.IsActive;
                        dbuser.IsAdmin = user.IsAdmin;

                        UserBusiness.Update(dbuser);
                    }
                    else
                    {
                        dbuser.Email = user.Email;
                        dbuser.Name = user.Name;
                        dbuser.Password = user.Password;
                        dbuser.ProfileImage = profileImage != null ? user.ProfileImage : dbuser.ProfileImage;
                        dbuser.Surname = user.Surname;
                        dbuser.Username = user.Username;
                        dbuser.ActivateGuid = user.ActivateGuid;
                        dbuser.IsActive = user.IsActive;
                        dbuser.IsAdmin = user.IsAdmin;

                        UserBusiness.Add(dbuser);

                        if (SendMail)
                        {
                            List<string> mailaddress = new List<string>();
                            mailaddress.Add(dbuser.Email);

                            string SiteUri = ConfigHelper.Get<string>("SiteUri");
                            string ActivateUri = $"{SiteUri}/Home/UserActivate/{dbuser.ActivateGuid}";
                            string img = "src='C:/Users/acer/Documents/Visual Studio 2017/Projects/MyEverNoteSolution/MyEverNote.WebApp/Images/mail.jpg'";

                            //string htmlstring = MailBodyHtml.GetHtml(ActivateUri,img);

                            StreamReader reader = new StreamReader(Server.MapPath("~/HtmlPage1.html"));

                            var html = reader.ReadToEnd();

                            html = html.Replace("{Url}", ActivateUri);

                            html = Regex.Replace(html, "src=\".+\"\\s", img);
                            //html = html.Replace("{img1}", img);


                            bool actmailresult = MailHelper.SendMail(html, mailaddress, "MyEverNote Application Activation Link");
                            if (actmailresult)
                            {
                                ViewBag.calljavascriptfunction = "Popup()";
                                return View(dbuser);
                            }
                            else
                            {
                                ModelState.AddModelError("", "Kaydınız başarıyla gerçekleşmiştir. Aktivasyon mailiniz biraz geçikebilir");
                                return View(dbuser);
                            }
                        }

                    }
                    ViewBag.calljavascriptfunction = "Popup()";
                    return View(dbuser);
                }
            }
            else
            {
                return View(user);
            }
        }

        [MustAdmin]
        public ActionResult Delete(int? id)
        {
            ViewBag.id = id.Value;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EverNoteUser user = UserBusiness.GetById(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [MustAdmin]
        public ActionResult DeletePost(string id)
        {
            EverNoteUser duser = UserBusiness.GetById(Convert.ToInt32(id));

            UserBusiness.Delete(duser);

            TempData["calljavascriptfunction"] = "Popup()";

            return RedirectToAction("UserIslemleri");
        }

    }
}
