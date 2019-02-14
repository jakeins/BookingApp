using System;
using System.Data.SqlClient;

namespace BookingApp.Helpers
{
    public static class SqlExceptionTranslator
    {
        /// <summary>
        /// Generate approriate exception from SqlException
        /// </summary>
        /// <param name="ex">SqlException</param>
        /// <param name="message">Addtitional message contactane to default</param>
        public static void ReThrow(SqlException ex, string message = "")
        {
            switch (ex.Number)
            {
                case 207:// Invalid column name
                case 2812:// Could not find stored procedure
                    throw new InvalidProgramException("Operation failed because of a program error. " + ex.Message, ex);

                case 547:// Foreign key restriction
                    throw new Exceptions.OperationRestrictedRelationException($"Cannot perform {(string.IsNullOrEmpty(message) ? "the" : message)} operation due to the related entries restriction", ex);

                case 50001:
                    var exceptionDescription = ex.Message + " " + message;
                    switch (ex.State)
                    {
                        case 1:
                        case 3:
                        case 7:
                        case 8:
                        case 9:
                            throw new Exceptions.FieldValueTimeInvalidException(exceptionDescription);
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                            throw new Exceptions.RelatedEntryNotFoundException(exceptionDescription);
                        case 10:
                        case 11:
                        case 13:
                        case 14:
                        case 15:
                            throw new Exceptions.OperationFailedException(exceptionDescription);
                        case 12:
                            throw new Exceptions.CurrentEntryNotFoundException(exceptionDescription);
                        case 16:
                            throw new Exceptions.FieldValueAbsurdException(exceptionDescription);
                        default:
                            throw new InvalidProgramException("Uknown SQL Exception catched " + message, ex);
                    }
                default:
                    throw new InvalidProgramException("Uknown SQL Exception catched ", ex);
            }
        }
    }
}
