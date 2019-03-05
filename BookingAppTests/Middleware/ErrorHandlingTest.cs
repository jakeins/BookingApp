using Xunit;
using Moq;
using BookingApp.Middlewares;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using BookingApp.Exceptions;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System;

namespace BookingAppTests.Middleware
{
    public class ErrorHandlingTest
    {
        private readonly Mock<ILogger<ErrorHandlingMiddleware>> loggerMock;
        private readonly bool isDevelopment;

        public ErrorHandlingTest()
        {
            loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            isDevelopment = true;
        }


        #region ErrorsWithBadRequestStatusCode
        [Fact]
        public async Task FieldValueTimeInvalidExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new FieldValueTimeInvalidException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task RelatedEntryNotFoundExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new RelatedEntryNotFoundException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task OperationFailedExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new OperationFailedException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task UserExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new UserException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        }

        #endregion

        #region ErrorsWithNotFoundStatusCode
        [Fact]
        public async Task CurrentEntryNotFoundExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new CurrentEntryNotFoundException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        }

        [Fact]
        public async Task EntryNotFoundExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new EntryNotFoundException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        }

        [Fact]
        public async Task NullReferenceExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new NullReferenceException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        }
        #endregion

        #region ErrorsWithForbiddenStatusCode

        [Fact]
        public async Task OperationRestrictedRelationExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new OperationRestrictedRelationException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        }

        #endregion

        #region ErrorsWithUnauthorizedStatusCode

        [Fact]
        public async Task OperationRestrictedExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new OperationRestrictedException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task UnauthorizedAccessExceptionHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new UnauthorizedAccessException(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        #endregion

        #region OtherErrors

        [Fact]
        public async Task OtherExceptionsHandling()
        {
            //Arrange
            var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new Exception(),
            logger: loggerMock.Object,
            IsDevelopment: isDevelopment);

            var context = new DefaultHttpContext();

            //Act
            await middleware.Invoke(context);

            //Asert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        }

        #endregion






    }
}
