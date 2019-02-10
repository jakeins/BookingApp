using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookingApp.Midlewares
{
    public class ErrorHandlingMiddleware
    {
        readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            if (exception is Exceptions.FieldValueTimeInvalidException)
            {
                code = HttpStatusCode.BadRequest;
            }
            else if(exception is Exceptions.RelatedEntryNotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if(exception is Exceptions.OperationFailedException)
            {
                code = HttpStatusCode.BadRequest;
            }
            else if(exception is Exceptions.CurrentEntryNotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if(exception is Exceptions.OperationRestrictedException)
            {
                code = HttpStatusCode.Unauthorized;
            }
            else if(exception is InvalidProgramException)
            {
                code = HttpStatusCode.InternalServerError;
            }
            else if(exception is NullReferenceException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if(exception is Exceptions.EntryNotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else
            {
                code = HttpStatusCode.InternalServerError;
            }
            
            var result = JsonConvert.SerializeObject(new { error = exception.Message, stacktrace = exception.StackTrace});
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
