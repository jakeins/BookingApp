using System;

namespace BookingApp.DTOs
{
    /// <summary>
    /// Helper object, lists all update properties.
    /// </summary>
    public class TreeGroupUpdateSubsetDto : TreeGroupMinimalDto
    {
        public DateTime UpdatedTime { get; set; }
        public string UpdatedUserId { get; set; }
    }
}
