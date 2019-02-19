using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        readonly bool IsDevelopment;
        readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, bool IsDevelopment)
        {
            this.next = next;
            this.IsDevelopment = IsDevelopment;
            this.logger = logger;
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
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
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
                case Exceptions.OperationRestrictedRelationException detailed:
                    code = HttpStatusCode.Forbidden;
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
                case Exceptions.UserEmailException detailed:
                    code = HttpStatusCode.BadRequest;
                    break;
                case Exceptions.UserNameException detailed:
                    code = HttpStatusCode.BadRequest;
                    break;
                case Exceptions.UserPasswordException detailed:
                    code = HttpStatusCode.BadRequest;
                    break;

                default:
                    logger.LogError($"Catched internal exception. Message: {exception.Message}. Stacktrace: {exception.StackTrace}");
                    code = HttpStatusCode.InternalServerError;
                    break;
            }

            object data;

            if (IsDevelopment)
            {
                data = new DevelopmentExceptionInfo(exception.Message, exception.StackTrace);
                logger.LogDebug($"Catched exception. Message: {exception.Message}. Stacktrace: {exception.StackTrace}");
            }
            else
            {
                data = new ExceptionInfo(exception.Message);
                logger.LogInformation($"Catched exception. Message: {exception.Message}.");
            }

            var result = JsonConvert.SerializeObject(data, Formatting.Indented);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        class ExceptionInfo
        {
            public string Message { get; set; }

            public ExceptionInfo(string message)
            {
                Message = message;
            }
        }

        class DevelopmentExceptionInfo : ExceptionInfo
        {
            public string StackTrace { get; set; }

            public DevelopmentExceptionInfo(string message, string stackTrace) : base(message)
            {
                StackTrace = stackTrace;
            }
        }
    }
}
