using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Repairmen.ActionFilters
{
    public class ValidationResponseFilter : ActionFilterAttribute
    {
        //
        // Called when [action executing]
        //
        // The action context.
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if(!actionContext.ModelState.IsValid)
            {
                //actionContext.ModelState.Keys
                actionContext.Response = actionContext
                    .Request
                    .CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }

         }
    }
}

