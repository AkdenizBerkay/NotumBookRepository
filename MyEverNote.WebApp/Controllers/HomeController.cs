using MyEverNote.BusinessLayer;
using MyEverNote.Common.Helper;
using MyEverNote.Entities;
using MyEverNote.WebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages.Html;

namespace MyEverNote.WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Home(int? id, bool lastnotes = false, bool mostlike = false)
        {
            Manager<Note> nm = new Manager<Note>();
            List<Note> NoteList = new List<Note>();
            if (id == 0 || id == null)
            {
                NoteList = nm.GetAll().ToList();
            }
            else
            {
                NoteList = nm.GetAll(k => k.CategoryId == id).ToList();
            }

            if (lastnotes)
            {
                NoteList = NoteList.OrderByDescending(k => k.CreateOn).ToList();
            }

            if (mostlike)
            {
                NoteList = NoteList.OrderByDescending(k => k.LikeCount).ToList();
            }
            return View(NoteList);
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            CurrentSession.Clear("loginuser");
            return RedirectToAction("Home");
        }
        public ActionResult Login()
        {
            

            return View();
        }
        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            Validation<EverNoteUser> validate = new Validation<EverNoteUser>();
            EverNoteUser loginuser = new EverNoteUser();
            loginuser.Username = Username;

            if (!validate.IsUnique(loginuser, "username"))
            {
                ViewBag.username = Username;
                Manager<EverNoteUser> manager = new Manager<EverNoteUser>();
                loginuser = manager.Get(k => k.Username.Equals(loginuser.Username));
                if (Password.Equals(loginuser.Password))
                {
                    if (loginuser.IsActive)
                    {
                        CurrentSession.Set<EverNoteUser>("loginuser", loginuser);
                        return RedirectToAction("Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lütfen hesabınızı aktivasyon mailinizdeki linkten aktive ediniz.");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Yanlış şifre.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Yanlış kullanıcı adı.");
                return View();
            }

        }
        public ActionResult UserActivate(Guid id)
        {
            Manager<EverNoteUser> mn = new Manager<EverNoteUser>();
            EverNoteUser actuser = mn.Get(k => k.ActivateGuid.Equals(id.ToString()));
            if (actuser != null && actuser.IsActive == false)
            {
                actuser.IsActive = true;
                mn.Update(actuser);
                return RedirectToAction("Login");
            }

            else if (actuser.IsActive)
            {
                return RedirectToAction("UserActivateCancel", new { user = actuser });
            }
            else if (actuser == null)
            {
                return RedirectToAction("UserActivateCancel", new { user = actuser });
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult UserActivateCancel(EverNoteUser user)
        {
            if (user == null)
            {

            }
            else if (user.IsActive)
            {

            }
            return View();
        }

        public ActionResult UserProfile(int? id)
        {
            EverNoteUser user = null;
            if (id == null || id == 0)
            {
                user = CurrentSession.User;
            }
            else {
                Manager<EverNoteUser> UserBusiness = new Manager<EverNoteUser>();
                user = UserBusiness.GetById(id.Value);
                ViewBag.YazarId = id;
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult UserProfile(EverNoteUser user, HttpPostedFileBase profileImage)
        {
            Manager<EverNoteUser> mn = new Manager<EverNoteUser>();
            EverNoteUser dbuser = mn.Get(k => k.Id == user.Id);
            if (ModelState.IsValid)
            {
                Validation<EverNoteUser> validate = new Validation<EverNoteUser>();

                if (!validate.IsUnique(user, "username", true))
                {
                    ModelState.AddModelError("", "Kullanıcı adı mevcut");

                    return View(dbuser);
                }
                else if (!validate.IsUnique(user, "email", true))
                {
                    ModelState.AddModelError("", "Email adresi mevcut");
                    return View(dbuser);
                }
                else
                {
                    if (profileImage!=null && (profileImage.ContentType.Equals("image/jpeg") || profileImage.ContentType.Equals("image/png") || profileImage.ContentType.Equals("image/jpg")))
                    {
                        string filename = $"user_{user.Id}.{profileImage.ContentType.Split('/')[1]}";
                        profileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                        user.ProfileImage = filename;
                    }

                    dbuser.Email = user.Email;
                    dbuser.Name = user.Name;
                    dbuser.Password = user.Password;
                    dbuser.ProfileImage = profileImage!=null ? user.ProfileImage : dbuser.ProfileImage;
                    dbuser.Surname = user.Surname;
                    dbuser.Username = user.Username;

                    mn.Update(dbuser);

                    ViewBag.calljavascriptfunction = "Popup()";
                    CurrentSession.Set<EverNoteUser>("loginuser", dbuser);

                    return View(dbuser);
                }

            }
            else
            {
                return View(dbuser);
            }
        }
        public ActionResult Register()
        {
            EverNoteUser user = new EverNoteUser();
            return View(user);
        }
        [HttpPost]
        public ActionResult Register(EverNoteUser user, string RePassword)
        {
            Validation<EverNoteUser> validate = new Validation<EverNoteUser>();

            if (!user.Password.Equals(RePassword))
            {
                ModelState.AddModelError("RePassword", "Password alanı ile eşleşmiyor");
                return View();
            }
            else if (!validate.IsUnique(user, "username"))
            {
                ModelState.AddModelError("", "Kullanıcı adı mevcut");
                return View();
            }
            else if (!validate.IsUnique(user, "email"))
            {
                ModelState.AddModelError("", "Email adresi mevcut");
                return View();
            }
            else
            {
                EverNoteUser newuser = new EverNoteUser();
                newuser.IsAdmin = false;
                newuser.IsActive = false;
                newuser.ActivateGuid = Guid.NewGuid().ToString();
                newuser.Name = user.Name;
                newuser.Surname = user.Surname;
                newuser.Username = user.Username;
                newuser.Password = user.Password;
                newuser.Email = user.Email;

                Manager<EverNoteUser> manager = new Manager<EverNoteUser>();
                if (ModelState.IsValid)
                {
                    manager.Add(newuser);
                    ViewBag.Name = newuser.Username;
                    ViewBag.calljavascriptfunction = "Popup()";
                    List<string> mailaddress = new List<string>();
                    mailaddress.Add(newuser.Email);

                    string SiteUri = ConfigHelper.Get<string>("SiteUri");
                    string ActivateUri = $"{SiteUri}/Home/UserActivate/{newuser.ActivateGuid}";
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
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Kaydınız başarıyla gerçekleşmiştir. Aktivasyon mailiniz biraz geçikebilir");
                        return View();
                    }
                }
                else
                {
                    return View(user);
                }
            }

        }

    }
}