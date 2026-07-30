namespace Textile.Core.Entities.Models.Response.Tally
{
    public class TallyConfigResponse
    {
        public int CompanyId { get; set; }  
        public CompanyDto Company { get; set; }

        public LedgerGroupDto Purchase { get; set; }
        public LedgerGroupDto Sale { get; set; }
    }
    public class LedgerGroupDto
    {
        public string MainLedger { get; set; }
        public string CGST { get; set; }
        public string SGST { get; set; }
        public string IGST { get; set; }
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        public string GSTIN { get; set; }
        public string StateName { get; set; }
        public string GSTRegistrationType { get; set; }
        public string Consignee { get; set; }

        public string ConsigneeAddress { get; set; }
        public string PINCode { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
