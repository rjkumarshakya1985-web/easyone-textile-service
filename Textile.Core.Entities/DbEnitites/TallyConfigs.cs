namespace Textile.Core.Entities.DbEnitites
{ 
    public class TallyConfigs : DatabaseEntity<int>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? TransactionType { get; set; } // e.g., "PURCHASE", "SALE"
        public string? TaxType { get; set; } // e.g., "MAIN", "CGST", "SGST", "IGST"
        public string? LedgerName { get; set; }      
  
     
    }
}
