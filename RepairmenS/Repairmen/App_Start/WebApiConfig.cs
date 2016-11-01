using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Repairmen.Helpers;
using Microsoft.Owin.Security.Infrastructure;
using System.Web.Http.Cors;

namespace Repairmen
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
     

            // Enabling cors - for enabling cross-origin resource sharing (in example, this service can be called from application from different domain).
            config.EnableCors();            
            
            // Attribute routing.
            config.MapHttpAttributeRoutes();

            // Convention-based routing.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                 name: "ActionApi",
                 routeTemplate: "api/{controller}/UserExists/{username}",
                 defaults: new { username = RouteParameter.Optional }
               );

           var json = config.Formatters.JsonFormatter;
           json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
           config.Formatters.Remove(config.Formatters.XmlFormatter);
           config.Filters.Add(new ActionFilters.ValidationResponseFilter());
          // config.MessageHandlers.Add(new CustomMsgHandler());
           
        }


    }
} 
