namespace Textile.Core.Entities.Models.Requests
{
    public class TableDataRequest
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? Search { get; set; }

        public string? SortField { get; set; }   // e.g. "supplierName"
        public int SortOrder { get; set; } = 1;

        public Dictionary<string, string>? Filters { get; set; }
    }
}
