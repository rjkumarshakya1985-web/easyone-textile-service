namespace Textile.Core.Entities.Views
{
    public class ParcelView
    {
        public int SaleVoucherId { get; set; }
        public required string  SupplierName { get; set; }
        public required string TransporterName { get;set; }
        public required string SupplierBillNumber { get; set; }

        public required int Quantity { get; set; }

        public required string Products { get; set; }
        public required string Status { get; set; }

    }
}
