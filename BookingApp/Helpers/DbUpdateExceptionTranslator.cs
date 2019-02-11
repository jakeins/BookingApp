using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
namespace BookingApp.Helpers
{
    /// <summary>
    /// <see cref="DbUpdateException"/> to local exceptions rethrowing.
    /// </summary>
    public static class DbUpdateExceptionTranslator
    {
        /// <summary>
        /// Convert known <see cref="DbUpdateException"/> exception to local exception.
        /// </summary>
        public static void ReThrow(DbUpdateException dbuException, string msg = "")
        {
            switch (dbuException)
            {
                case DbUpdateConcurrencyException concurrencyException:
                    string errorMessageTemplate = "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

                    if (concurrencyException.Message.Substring(0, errorMessageTemplate.Length) == errorMessageTemplate)
                        throw new OperationFailedException(msg + " failed due to DB concurrency issue", concurrencyException);
                    break;

                case var wrappedSqlException when wrappedSqlException.InnerException is SqlException sqlException:
                        SqlExceptionTranslator.ReThrow(sqlException, msg);
                    break;

                default:
                    throw new InvalidProgramException("Unexpected DbUpdateException " + msg, dbuException);
            }
        }
    }
}
