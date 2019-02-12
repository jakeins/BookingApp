namespace BookingApp.DTOs
{
    /// <summary>
    /// Suitable for detailed read. Has ID.
    /// </summary>
    public class ResourceMaxDto : ResourceDetailedDto
    {
        public int ResourceId { get; set; }
    }
}
