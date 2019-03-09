namespace BookingApp.DTOs.Resource
{
    /// <summary>
    /// Suitable for reading into the concise list. Has ID.
    /// </summary>
    public class ResourceBriefDto : ResourceBaseDto
    {
        public int Id { get; set; }
    }
}
