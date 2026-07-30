namespace Textile.Core.Entities.Models.Requests.Customers
{
    public class CustomerRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Alias { get; set; }
        public string? LedgerName { get; set; }
        public string? PrintName { get; set; }
        public string? GroupName { get; set; }
        public string? GstIn { get; set; }
        public string? Pan { get; set; }
        public int? RegType { get; set; }

        public decimal? Discount { get; set; }
        public decimal? Mu { get; set; }
        public int? PaymentTerm { get; set; }
        public int? CustomerCategory { get; set; }
        public int? CustomerStatus { get; set; }
        public int? RateType { get; set; }
        public string? AlternateNo { get; set; }
        public decimal? CreditAlertLimit { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? Term { get; set; }
        public string? Reference { get; set; }
        public string? CustomerCode { get; set; }
        public int? TransportId { get; set; }
        public Guid? CustomerAgentId { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShippingAddress { get; set; }
        public int CityId { get; set; }
        public string? PinCode { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public decimal? OpeningBalance { get; set; }
        public int? CreditDays { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? PriceLevel { get; set; }
        public int? TallyLedgerType { get; set; }
        public int? TallyCategory { get; set; }
        public int CustomerType { get; set; }
        public string? Remarks { get; set; }
    }

}
