using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookingApp.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke request and catch any errors
        /// </summary>
        /// <param name="context">Request context</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handler all exceptions in controlers and return to client aproriated http status code
        /// </summary>
        /// <param name="context">Http context for status code</param>
        /// <param name="exception">Exception which parse</param>
        /// <returns></returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;

            switch (exception)
            {
                case Exceptions.CurrentEntryNotFoundException detailed:
                    code = HttpStatusCode.NotFound;
                    break;
                case Exceptions.FieldValueTimeInvalidException detailed:
                    code = HttpStatusCode.BadRequest;
                    break;
                case Exceptions.RelatedEntryNotFoundException detailed:
                    code = HttpStatusCode.BadRequest;
                    break;
                case Exceptions.OperationRestrictedException detailed:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case Exceptions.EntryNotFoundException detailed:
                    code = HttpStatusCode.NotFound;
                    break;
                case Exceptions.OperationFailedException detailed:
                    code = HttpStatusCode.BadRequest;
                    break;
                case InvalidProgramException detailed:
                    code = HttpStatusCode.InternalServerError;
                    break;
                case NullReferenceException detailed:
                    code = HttpStatusCode.NotFound;
                    break;
                case UnauthorizedAccessException detailed:
                    code = HttpStatusCode.Unauthorized;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    break;

            }
            
            var result = JsonConvert.SerializeObject(new { error = exception.Message});
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
