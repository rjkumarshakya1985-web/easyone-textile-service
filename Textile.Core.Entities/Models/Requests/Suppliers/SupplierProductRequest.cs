namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class SupplierProductRequest
    {
        public Guid? Id { get; set; }
        public Guid SupplierId { get; set; }
        public int StockGroupId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string PrintName { get; set; }

        public string HsnCode { get; set; }
        public string Barcode { get; set; }

        // Yes, No
        public bool GstApplicable { get; set; }

        // Goods, Services
        public int GSTNature { get; set; }

        // 1 = Taxable, 2 = Exempt, 3 = NilRated

        public int GSTTaxability { get; set; }
        public decimal PurchaseRate { get; set; }

        public decimal? ManualWholeSaleRate { get; set; }
        public int Discount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
