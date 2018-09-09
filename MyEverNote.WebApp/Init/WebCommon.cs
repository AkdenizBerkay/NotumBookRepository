using MyEverNote.Common;
using MyEverNote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEverNote.WebApp.Init
{
    public class WebCommon : ICommon
    {
        public string GetUserName()
        {
            if (HttpContext.Current.Session["loginuser"] != null)
            {
                EverNoteUser loginuser = (EverNoteUser)HttpContext.Current.Session["loginuser"];
                return loginuser.Username;
            }
            else
            {
                return "system";
            }
        }
    }
}