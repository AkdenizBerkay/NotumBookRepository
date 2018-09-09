using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEverNote.WebApp.UIHelper
{
    public class MailBodyHtml
    {
        public static string GetHtml(string link,string img="")
        {
            string html = string.Format("<a href='{0}'><img src = '{1}' alt = 'Click!' width = '300' height = '230' style = 'display: block;'/>Activation Link...</a>", link,img);
            return html;
        }
    }
}