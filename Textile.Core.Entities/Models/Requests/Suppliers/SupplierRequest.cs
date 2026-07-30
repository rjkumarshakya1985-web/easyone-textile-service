namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class SupplierRequest
    {
        public Guid? Id { get; set; }
        public string? UserName { get; set; }
        public int? SubDepartmentId { get; set; }

        public Guid? AgentId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Alias { get; set; }

        public string? GstIn { get; set; }
        public string? PAN { get; set; }
        public int? RegType { get; set; }
        public string? Address { get; set; }
        public int? CityId { get; set; }

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
        public int? TransactionType { get; set; }

        public decimal WholeSalesMargin { get; set; }
        public decimal RetailMargin { get; set; }

        public decimal MrpMargin { get; set; }

        public decimal? BillDiscount { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public decimal? AnnualIncentive { get; set; }

        public string ? PinCode { get; set; }

        /// Mapping 
        /// 

        public int? StockGroupId { get; set; }
        public List<int>? TransportIds { get; set; }
        public Guid? HsnCodeId { get; set; }


    }



}
