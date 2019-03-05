namespace BookingApp.DTOs
{
    public class BookingsPerResourceStatusDTO: BookingsPerResourceBaseDTO
    {
        public BookingsPerResourceStatusDTO(string resourceTitle, int numOfIntervals) : base(resourceTitle, numOfIntervals)
        {
            GoodBookingsPerInterval = new int[numOfIntervals];
            CancelledBookingsPerInterval = new int[numOfIntervals];
            EarlyTerminatedBookingsPerInterval = new int[numOfIntervals];
        }

        public int[] GoodBookingsPerInterval { get; set; }
        public int[] CancelledBookingsPerInterval { get; set; }
        public int[] EarlyTerminatedBookingsPerInterval { get; set; }
        public int GoodBookingsSum { get; set; }
        public int CancelledBookingsSum { get; set; }
        public int EarlyTerminatedBookingsSum { get; set; }        
    }
}
