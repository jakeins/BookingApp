using BookingApp.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Helpers
{
    public static class SqlExceptionTranslator
    {
        /// <summary>
        /// Generate approriate exception from SqlException
        /// </summary>
        /// <param name="ex">SqlException</param>
        /// <param name="Message">Addtitional message contactane to default</param>
        public static void ReThrow(SqlException ex, string Message = "")
        {
            switch (ex.Number)
            {
                case 50001:
                    var exceptionDescription = ex.Message + " " + Message;
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
                            throw new InvalidProgramException("Uknown SQL Exception catched " + Message, ex);
                    }
                case 547:
                    throw new Exceptions.OperationRestrictedException("Relation block this operation: " + Message);
                default:
                    throw new InvalidProgramException("Uknown SQL Exception catched ", ex);
            }
        }
    }
}
