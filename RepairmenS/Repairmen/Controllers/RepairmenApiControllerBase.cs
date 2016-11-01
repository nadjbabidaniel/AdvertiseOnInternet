﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Repairmen.Controllers
{
    public class RepairmenApiControllerBase : ApiController
    {
        //protected LogHandler logger = new LogHandler();

        protected HttpResponseMessage BadRequest(string message, Exception exception)
        {
            //logger.LogError(exception);
            return Request.CreateResponse(HttpStatusCode.BadRequest, new
            {
                Code = exception.GetType().Name,
                Message = message
            });
        }

        protected HttpResponseMessage OK(object payload)
        {
            return Request.CreateResponse(HttpStatusCode.OK, payload);
        }

        protected HttpResponseMessage Create()
        {
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        protected HttpResponseMessage Create(object payload)
        {
            return Request.CreateResponse(HttpStatusCode.Created, payload);
        }

        protected HttpResponseMessage NoContent()
        {
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        protected HttpResponseMessage NotFound(string message)
        {
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, message);
        }

        protected HttpResponseMessage PreconditionFailed(string message)
        {
            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, message);
        }

        protected HttpResponseMessage Forbidden()
        {
            return Request.CreateResponse(HttpStatusCode.Forbidden);
        }

        protected HttpResponseMessage InternalServerError(string message, Exception exception)
        {
            //logger.LogError(exception);
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message);
        }
    }
}