namespace Textile.Core.Entities.DbEnitites
{
    public class SaleVoucherDetail : DatabaseEntity<Guid>
    {
        public int SaleVoucherId { get; set; }
        public Guid ProductId { get; set; }
        public decimal PurchaseRate { get; set; }
        public int Quantity { get; set; }

        /// <summary>
        /// Gst Coumn
        /// </summary>
        public decimal Discount { get; set; }

        public decimal WholeSalesMargin { get; set; }
        public decimal RetailMargin { get; set; }
        public decimal MrpMargin { get; set; }

        public decimal ? ManualWholeSaleRate { get; set; }
        /// <summary>
        /// Computed Column but It gives exception 
        /// </summary>
        public int WholeSaleRate { get; set; }
        public int RetailPrice { get; set; }
        public int MrpRate { get; set; }

        public decimal SupplierDiscount { get; set; }

        public bool IsSupplierDiscount { get; set; }

        public SupplierProduct Product { get; set; }
    }
}
