namespace BookingApp.DTOs.TreeGroup
{
    public class TreeGroupListTdo
    {
        public int TreeGroupId { get; set; }
        public string Title { get; set; }
        public int? ParentTreeGroupId { get; set; }
        public int? DefaultRuleId { get; set; }
        public bool? IsActive { get; set; }
    }
}
