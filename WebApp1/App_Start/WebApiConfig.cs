using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApp1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Add default route using convention-based routing
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}

