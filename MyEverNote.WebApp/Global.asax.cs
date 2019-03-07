using MyEverNote.Common;
using MyEverNote.WebApp.App_Start;
using MyEverNote.WebApp.Init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyEverNote.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MVCGridConfig.RegisterGrids();
            App.Common = new WebCommon();
        }
    }
}
