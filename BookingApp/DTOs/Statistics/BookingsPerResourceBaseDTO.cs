namespace BookingApp.DTOs
{
    public class BookingsPerResourceBaseDTO
    {
        public BookingsPerResourceBaseDTO(string resourceTitle, int numOfIntervals)
        {
            ResourceTitle = resourceTitle;
            BookingsPerInterval = new int[numOfIntervals];
        }

        public string ResourceTitle { get; set; }
        public int[] BookingsPerInterval { get; set; }
        public int BookingsSum { get; set; }
    }
}
