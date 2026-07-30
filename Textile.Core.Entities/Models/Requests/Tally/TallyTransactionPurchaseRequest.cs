using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Entities.Models.Requests.Tally
{
    public class TallyTransactionPurchaseRequest
    {

        public SupplierResponse SupplierResponse { get; set; }
        public SaleVoucherPrint SaleVoucherPrint { get; set; }
        public List<StockCategoryResponse> StockCategoryResponse { get; set; }
        public List<StockGroupResponse> StockGroupResponse { get; set; }
        public List<StockitemResponse> StockitemResponse { get; set; }
    }
    public class SaleVoucherPrint
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? InVoiceNo { get; set; }
        public string? VoucherForeignkey { get; set; }
        public string? TransportName { get; set; }
        public string? SupplierBillNumber { get; set; }
        public decimal Discount { get; set; }
        public DateTime Date { get; set; }
        public string? GstIn { get; set; }
    }
    public class SupplierResponse
    {
        public string? Department { get; set; }
        public string? SubDepartment { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public string? TallyLedgerName { get; set; }
        public string? Alias { get; set; }

        public string? GstIn { get; set; }
        public string? PAN { get; set; }

        public int? RegType { get; set; }
        public string? Address { get; set; }

        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string StateCode { get; set; }
        public string? City { get; set; }

        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }

        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSC { get; set; }
        public string? UPID { get; set; }

        public int? CreditDays { get; set; }
        public decimal? CreditLimit { get; set; }

        public DateTime? GstRegistrationDate { get; set; }

        public string? MSMENumber { get; set; }
        public string? ECCNumber { get; set; }
        public string? Remarks { get; set; }

        public int DiscountType { get; set; }

        public decimal Discount { get; set; }




        public AgentTableResponse? AgentObj { get; set; }
    }
    public class StockCategoryResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TallyLedgerName { get; set; }
        public int GstValue { get; set; }
        public string? Description { get; set; }
        public string? HsnCode { get; set; }

    }
    public class StockGroupResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TallyLedgerName { get; set; }
        public int GstValue { get; set; }
        public string? Description { get; set; }
        public string? HsnCode { get; set; }
        public bool IsGstRule { get; set; }
        public decimal StartRange { get; set; }
        public decimal EndRange { get; set; }

    }
    public class StockitemResponse
    {

        public string ProductName { get; set; }
        public string? TallyLedgerName { get; set; }

        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal WholeSaleRate { get; set; }
        public decimal Discount { get; set; }
        public decimal RetailPrice { get; set; }
        public string? HsnCode { get; set; }
        public decimal Gst { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal Total { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal MrpRate { get; set; }
        public bool IsGstRule { get; set; }
        public decimal StartRange { get; set; }
        public decimal EndRange { get; set; }
    }

}
