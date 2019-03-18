using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BookingApp.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly bool IsDevelopment;
        private readonly ILogger<ErrorHandlingMiddleware> logger;

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
                case Exceptions.FieldValueTimeInvalidException _:
                case Exceptions.RelatedEntryNotFoundException _:
                    code = HttpStatusCode.BadRequest;
                    break;

                case Exceptions.CurrentEntryNotFoundException _:
                case Exceptions.EntryNotFoundException _:
                case NullReferenceException _:
                    code = HttpStatusCode.NotFound;
                    break;

                case Exceptions.OperationRestrictedRelationException _:
                    code = HttpStatusCode.Forbidden;
                    break;

                case Exceptions.OperationRestrictedException _:
                case UnauthorizedAccessException _:
                    code = HttpStatusCode.Unauthorized;
                    break;

                case Exceptions.OperationFailedException _:
                case Exceptions.UserException _:
                    code = HttpStatusCode.BadRequest;
                    break;
                case SecurityTokenException detailed:
                    code = HttpStatusCode.Unauthorized;
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
                logger.LogDebug($"Catched {exception.GetType()} exception. Message: {exception.Message}. Stacktrace: {exception.StackTrace}");
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

        private class ExceptionInfo
        {
            public string Message { get; set; }

            public ExceptionInfo(string message)
            {
                Message = message;
            }
        }

        private class DevelopmentExceptionInfo : ExceptionInfo
        {
            public string StackTrace { get; set; }

            public DevelopmentExceptionInfo(string message, string stackTrace) : base(message)
            {
                StackTrace = stackTrace;
            }
        }
    }
}