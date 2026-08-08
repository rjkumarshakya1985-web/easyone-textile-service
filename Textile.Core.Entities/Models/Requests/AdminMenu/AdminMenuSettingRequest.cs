namespace Textile.Core.Entities.Models.Requests.AdminMenu
{
    public class AdminMenuSettingRequest
    {
        public List<AdminMenuSettingItemRequest> Items { get; set; } = new();
    }

    public class AdminMenuSettingItemRequest
    {
        public string MenuKey { get; set; }
        public string Label { get; set; }
        public bool IsEnabled { get; set; }
    }
}
