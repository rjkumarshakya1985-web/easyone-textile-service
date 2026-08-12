using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Views;

namespace Textile.Core.Infrastructure.Context
{
    public class TextileDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TextileDbContext(
            DbContextOptions<TextileDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<Gst> Gsts { get; set; }
        public DbSet<ProductHsnCode> HsnCodes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SubDepartment> SubDepartments { get; set; }
        public DbSet<Transport> Transports { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StockGroup> StockGroups { get; set; }
        public DbSet<GstRule> GstRules { get; set; }
        public DbSet<SupplierProduct> SupplierProducts { get; set; }
        public DbSet<SupplierProductPriceHistory> SupplierProductPriceHistories { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AdminMenuSetting> AdminMenuSettings { get; set; }
        public DbSet<StickerPrintSetting> StickerPrintSettings { get; set; }
        public DbSet<StickerPrintFieldSetting> StickerPrintFieldSettings { get; set; }

        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAgent> CustomerAgents { get; set; }
        public DbSet<SupplierTransport> SupplierTransports { get; set; }
        public DbSet<SupplierStockGroup> SupplierStockGroups { get; set; }

        public DbSet<SupplierHsnCode> SupplierHsnCodes { get; set; }
        public DbSet<SaleVoucher> SaleVouchers { get; set; }

        public DbSet<SaleVoucherDetail> SaleVoucherDetails { get; set; }
        public DbSet<SaleVoucherStatus> SaleVoucherStatus { get; set; }

        

        public DbSet<SaleVoucherPrintDetail> SaleVoucherPrintDetails { get; set; }
        public DbSet<Agent> Agents { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }


        public DbSet<SupplierProductView> SupplierProductViews { get; set; }
        public DbSet<StockLedgerViews> StockLedgerViews { get; set; }

        public DbSet<CurrentStockView> CurrentStockViews { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<SalePerson> SalesPersons { get; set; }
        public DbSet<FinanceYear> FinanceYears { get; set; }
        public DbSet<PackingSlip> PackingSlips { get; set; }
        public DbSet<PackingSlipItem> PackingSlipItems { get; set; }

        public DbSet<VoucherType> VoucherTypes { get; set; }
        public DbSet<VoucherNumberSeries> VoucherNumberSeries { get; set; }

        public DbSet<DeliveryChallan> DeliveryChallans { get; set; }
        public DbSet<DeliveryChallanItem> DeliveryChallanItems { get; set; }
        public DbSet<DeliveryChallanReturn> DeliveryChallanReturns { get; set; }
        public DbSet<DeliveryChallanReturnItem> DeliveryChallanReturnItems { get; set; }
        public DbSet<DeliveryChallanPackingSlipMap> DeliveryChallanPackingSlipMaps { get; set; }




        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDeliveryChallanMap> InvoiceDeliveryChallanMaps { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<InvoicePackingSlipMap> InvoicePackingSlipMaps { get; set; }

        public DbSet<TallyConfigs> TallyConfigs { get; set; }
        public DbSet<TallyCompanies> TallyCompanies { get; set; }
        public DbSet<TallyProcessLogs> TallyProcessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<AdminMenuSetting>(entity =>
            {
                entity.HasIndex(x => x.MenuKey).IsUnique();
                entity.Property(x => x.MenuKey).HasMaxLength(120).IsRequired();
                entity.Property(x => x.Label).HasMaxLength(150).IsRequired();
                entity.Property(x => x.IsEnabled).HasDefaultValue(true);
            });

            modelBuilder.Entity<StickerPrintSetting>(entity =>
            {
                entity.Property(x => x.CompanyShortName).HasMaxLength(30).IsRequired();
                entity.Property(x => x.WholeSaleRatePrefix).HasMaxLength(20);
                entity.Property(x => x.WholeSaleRatePostfix).HasMaxLength(20);
                entity.Property(x => x.WholeSaleRateAddAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.WholeSaleRateCodeDigitCount).HasDefaultValue(2);
                entity.Property(x => x.WholeSaleRateCode0).HasMaxLength(10).IsRequired().HasDefaultValue("A");
                entity.Property(x => x.WholeSaleRateCode1).HasMaxLength(10).IsRequired().HasDefaultValue("B");
                entity.Property(x => x.WholeSaleRateCode2).HasMaxLength(10).IsRequired().HasDefaultValue("C");
                entity.Property(x => x.WholeSaleRateCode3).HasMaxLength(10).IsRequired().HasDefaultValue("D");
                entity.Property(x => x.WholeSaleRateCode4).HasMaxLength(10).IsRequired().HasDefaultValue("E");
                entity.Property(x => x.WholeSaleRateCode5).HasMaxLength(10).IsRequired().HasDefaultValue("F");
                entity.Property(x => x.WholeSaleRateCode6).HasMaxLength(10).IsRequired().HasDefaultValue("G");
                entity.Property(x => x.WholeSaleRateCode7).HasMaxLength(10).IsRequired().HasDefaultValue("H");
                entity.Property(x => x.WholeSaleRateCode8).HasMaxLength(10).IsRequired().HasDefaultValue("I");
                entity.Property(x => x.WholeSaleRateCode9).HasMaxLength(10).IsRequired().HasDefaultValue("J");
                entity.Property(x => x.ShowSupplierCode).HasDefaultValue(true);
                entity.Property(x => x.ShowCompanyShortName).HasDefaultValue(true);
                entity.Property(x => x.ShowWholeSaleRate).HasDefaultValue(true);
                entity.Property(x => x.ShowProductName).HasDefaultValue(true);
                entity.Property(x => x.ShowPrintDate).HasDefaultValue(true);
                entity.Property(x => x.ShowRetailRate).HasDefaultValue(true);
                entity.Property(x => x.ShowBarcode).HasDefaultValue(true);
                entity.Property(x => x.ShowBarcodeText).HasDefaultValue(true);
                entity.Property(x => x.ApplyWholeSaleRateFormula).HasDefaultValue(true);
                entity.Property(x => x.ApplyWholeSaleRateCode).HasDefaultValue(false);
                entity.Property(x => x.CompanyShortName).HasDefaultValue("SSBD");
                entity.Property(x => x.WholeSaleRatePrefix).HasDefaultValue("5");
                entity.Property(x => x.WholeSaleRateAddAmount).HasDefaultValue(500);
            });

            modelBuilder.Entity<StickerPrintFieldSetting>(entity =>
            {
                entity.HasIndex(x => new { x.StickerPrintSettingId, x.FieldKey }).IsUnique();
                entity.Property(x => x.FieldKey).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Label).HasMaxLength(80).IsRequired();
                entity.Property(x => x.FontWeight).HasMaxLength(20).IsRequired();
                entity.Property(x => x.TextAlign).HasMaxLength(20).IsRequired();
                entity.Property(x => x.X).HasColumnType("decimal(8,2)");
                entity.Property(x => x.Y).HasColumnType("decimal(8,2)");
                entity.Property(x => x.Width).HasColumnType("decimal(8,2)");
                entity.Property(x => x.Height).HasColumnType("decimal(8,2)");
                entity.Property(x => x.IsVisible).HasDefaultValue(true);
                entity.HasOne(x => x.StickerPrintSetting)
                    .WithMany(x => x.FieldSettings)
                    .HasForeignKey(x => x.StickerPrintSettingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SaleVoucherDetail>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));

              
                entity.Property(p => p.RetailPrice)
                      .ValueGeneratedOnAddOrUpdate()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.MrpRate)
                      .ValueGeneratedOnAddOrUpdate()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);


            });


            modelBuilder.Entity<DeliveryChallan>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));

                entity.Property(p => p.TotalEffectiveQty)
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<DeliveryChallanItem>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));

                entity.Property(p => p.EffectiveQty)
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);



                entity.Property(p => p.TaxableAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.DiscountAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.NetAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.GstAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.TotalAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });



            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));

                entity.Property(p => p.GrandTotal)
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));

                entity.Property(p => p.TaxableAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.DiscountAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.NetAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.GstAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.TotalAmount)
                    .ValueGeneratedOnAddOrUpdate()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });



            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));


                entity.Property(p => p.TotalQty)
                      .ValueGeneratedOnAddOrUpdate()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                entity.Property(p => p.AvailableQty)
                      .ValueGeneratedOnAddOrUpdate()
                      .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

              

            });

            modelBuilder.Entity<StockLedgerViews>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("StockLedgerViews"); 
            });


            base.OnModelCreating(modelBuilder);
        }

    }
}
