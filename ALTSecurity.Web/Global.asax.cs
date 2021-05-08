using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using ALTSecurity.Web.App_Start;
using ALTSecurity.Web.Utility;

namespace ALTSecurity.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Logger.Init();

            ProxyConfig.ProxyUrl = "102.129.249.120";
            ProxyConfig.ProxyPort = "8080";
        }
    }
}
