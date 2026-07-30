namespace Textile.Core.Entities.Models.Response.Suppliers
{
    public class SupplierProductDto
    {
        public Guid Id { get; set; }

        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
        public int StockGroupId { get; set; }

        public string StockGroupName { get; set; }
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
        
        /// <summary>
        ///  Gst column
        /// </summary>
        public int Discount { get; set; }


        public decimal? ManualWholeSaleRate { get; set; }


        public decimal? WholeSaleRate { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? MrpRate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }
        public string CreatedByUserName { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByUserName { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public HsnCodeResponse HsnCodeObj { get; set; }

        public SupplierTableResponse SupplierObj { get; set; }

    }
}
