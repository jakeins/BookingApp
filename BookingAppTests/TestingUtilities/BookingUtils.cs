using BookingApp.Data.Models;
using System;
using System.Collections.Generic;

namespace TestingUtilities
{
    public class BookingUtils
    {
        public static string SoleBookingCreator => "SoleBookingCreator";

        static IEnumerable<Booking> testBookings;
        public static IEnumerable<Booking> TestSet => testBookings ?? FormTestBookings();
        static IEnumerable<Booking> FormTestBookings()
        {
            var sampleSize = 5;
            var newBookings = new Booking[sampleSize];
            var now = DateTime.Now;

            for (int i = 1; i <= sampleSize; i++)
            {
                var curBook = new Booking
                {
                    Id = i,
                    Note = "Note_" + i,
                    ResourceId = 1,
                    CreatedUserId = SoleBookingCreator,
                    UpdatedUserId = "Updater_" + i
                };
                curBook.CreatedTime = curBook.StartTime = curBook.EndTime = curBook.UpdatedTime = now;
                newBookings[i - 1] = curBook;
            }

            testBookings = newBookings;
            return testBookings;
        }
    }
}