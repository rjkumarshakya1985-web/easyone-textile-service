namespace Textile.Core.Entities.DbEnitites
{
    public class Stock : DatabaseEntity<Guid>
    {

        public Guid ProductId { get; set; }

        public decimal OpeningQty { get; set; } = 0;
        public decimal InwardQty { get; set; } = 0;
        public decimal OutwardQty { get; set; } = 0;

        public decimal ReservedQty { get; set; } = 0;
        public decimal DamagedQty { get; set; } = 0;


        public decimal TotalQty { get; private set; }

        public decimal AvailableQty { get; private set; }

        public decimal? PurchaseRate { get; set; }
        public decimal? Discount { get; set; }

        public decimal? WholeSaleMargin { get; set; }
        public decimal? RetailMargin { get; set; }
        public decimal? MrpMargin { get; set; }

        public decimal? WholeSaleRate { get; set; }
        public decimal? RetailRate { get; set; }
        public decimal? MrpRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public SupplierProduct Product { get; set; }
    }
}
