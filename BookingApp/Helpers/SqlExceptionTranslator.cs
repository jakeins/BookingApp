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
            if(ex.Number == 50001)
            {
                switch(ex.State)
                {
                    case 1:
                        //Can not book when StartTime in past
                        throw new Exceptions.InvalidTimeFieldException(ex.Message + " " + Message);
                    case 2:
                        //Invalid user id
                        throw new Exceptions.RelatedEntryNotFoundException(ex.Message + " " + Message);
                    case 3:
                        //StartTime must be lower than EndTime
                        throw new Exceptions.InvalidTimeFieldException(ex.Message + " " + Message);
                    case 4:
                        //Invalid resource id passed
                        throw new Exceptions.RelatedEntryNotFoundException(ex.Message + " " + Message);
                    case 5:
                        //Resorce is disable and can not book
                        throw new Exceptions.RelatedEntryNotFoundException(ex.Message + " " + Message);
                    case 6:
                        //Rule is disabled for this resource
                        throw new Exceptions.RelatedEntryNotFoundException(ex.Message + " " + Message);
                    case 7:
                        //Booking duration less than min valid for this resource
                        throw new Exceptions.InvalidTimeFieldException(ex.Message + " " + Message);
                    case 8:
                        //Booking duration more than max valid for this resource
                        throw new Exceptions.InvalidTimeFieldException(ex.Message + " " + Message);
                    case 9:
                        //The duration of the reservation must be a multiple step of the booking for this resource
                        throw new Exceptions.InvalidTimeFieldException(ex.Message + " " + Message);
                    case 10:
                        //Booking time is too early
                        throw new Exceptions.OperationFailedException(ex.Message + " " + Message);
                    case 11:
                        //Time range alredy booked
                        throw new Exceptions.OperationFailedException(ex.Message + " " + Message);
                    case 12:
                        //Invalid BookingID
                        throw new Exceptions.CurrentEntryNotFoundException(ex.Message + " " + Message);
                    case 13:
                        //Can not edit termіnated booking
                        throw new Exceptions.OperationFailedException(ex.Message + " " + Message);
                    case 14:
                        //Can not edit ended booking
                        throw new Exceptions.OperationFailedException(ex.Message + " " + Message);
                    case 15:
                        //Can not change starttime of alredy started booking
                        throw new Exceptions.OperationFailedException(ex.Message + " " + Message);
                    default:
                        //If exception uknown to throw InvalidProgramException with this exception
                        throw new InvalidProgramException("Uknown SQL Exception catched " + Message, ex);
                }
            }
            else if(ex.Number == 547)
            {
                //Relations block deleting
                throw new Exceptions.OperationRestrictedException("Relation block this operation: " + Message);
            }

            //If exception uknown to throw InvalidProgramException with this exception
            throw new InvalidProgramException("Uknown SQL Exception catched ", ex);
        }
    }
}
