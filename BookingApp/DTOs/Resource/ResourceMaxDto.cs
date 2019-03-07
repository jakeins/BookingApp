namespace BookingApp.DTOs.Resource
{
    /// <summary>
    /// Suitable for detailed read. Has ID.
    /// </summary>
    public class ResourceMaxDto : ResourceDetailedDto
    {
        public int Id { get; set; }
    }
}
