using System;

namespace BookingApp.DTOs.Folder
{
    /// <summary>
    /// Helper object, lists all update properties.
    /// </summary>
    public class FolderUpdateSubsetDto : FolderMinimalDto
    {
        public DateTime UpdatedTime { get; set; }
        public string UpdatedUserId { get; set; }
    }
}
