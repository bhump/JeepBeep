﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyJeepTrader.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Details",
                url: "UserProfile/Details/{id}",
                defaults: new { controller = "UserProfile", action = "Details", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Dashboard",
                url: "Dashboard/Index/{id}",
                defaults: new { controller = "UserProfile", action = "Details", id = UrlParameter.Optional }
                );
        }
    }
}
