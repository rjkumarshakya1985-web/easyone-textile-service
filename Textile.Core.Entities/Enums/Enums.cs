namespace Textile.Core.Entities.Enums
{

    public enum ClientType
    {
        Web,
        Mobile,
        Windows
    }

    public enum SupplierStatusActionType
    {
        Delete,
        Activate,
        Deactivate
    }
    public enum AgentStatusActionType
    {
        Delete,
        Activate,
        Deactivate
    }
    public enum CustomerStatusActionType
    {
        Delete,
        Activate,
        Deactivate
    }

    public enum RoleEnum
    {
        SuperAdmin = 1,
        Supplier = 2,
        Cashier = 3,
        PackingSlipOperator = 4,
        StockIncharge = 5
    }

    public enum TransportTypeEnum
    {
        Purchase = 1,
        Sales = 2,
        Both = 3
    }
    public static class GstNature
    {
        public const string Goods = "Goods";
        public const string Services = "Services";
    }

    public static class GstTaxAbility
    {
        public const string Taxable = "Taxable";
        public const string Exempt = "Exempt";
        public const string NilRated = "NilRated";
    }

    public enum ParcelStatusEnum
    {
        Ready = 1,
        Packed = 2,
        InTransit = 3,
        Transport = 4,
        PackedAtLocation = 5,
        Opened = 6,
        Returned = 7,
        Cancelled = 9,
        Other = 10,
        TallySynced = 11
    }



    public enum VoucherTypeEnum
    {

        PackingSlip = 1,
        DeliveryChallan = 2,
        DeliveryChallanReturn = 3,
        SaleInvoice = 4,
        CreditNote = 5,
        StockAdjustment = 6
    }

    public enum StockTranscationStatusEnum
    {
        SupplierParcel = 0,
        PackingSlip = 1,
        DeliveryChallan = 2,
        Invoice = 3,
        SaleReturn = 4,
        StockAdjustment = 5
    }

    public enum DeliveryChallanStatusEnum
    {
        Created = 0,
        Dispatched = 1,
        PartiallyReturned = 2,
        FullyReturned = 3,
        Invoiced = 4,
        Cancelled = 6,
        TallySynced = 7
    }

   
    public enum CustomerTypeEnum
    {
        Retail = 1,
        WholeSale = 2

    }

    public enum PackingSlipStatusEnum
    {
        Created = 0,
        Invoice = 1,
        DeliveryChallan = 2,
        Cancelled = 3
    }

    public enum InvoiceStatusEnum
    {
        Created = 0,
        DeliveryChallanInvoice = 1,
        Cancelled = 2,
        TallySynced = 3
    }
    public enum TallyType
    {
        Agent = 0,
        Supplier = 1,
        StockCategory=2,
        StockGroup = 3,
        StockItem = 4,
        Other = 5
    }
}
