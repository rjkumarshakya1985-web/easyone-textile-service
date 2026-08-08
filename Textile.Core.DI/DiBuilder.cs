using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Infrastructure.Repository;
using Textile.Core.Infrastructure.Services;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;
using Textile.Core.Interfaces.Services.Tally;
using Textile.Core.Managers.DI;
using Textile.Core.Managers.Services;
using Textile.Core.Managers.Services.Sales;
using Textile.Core.Managers.Services.Tally;

namespace Textile.Core.DI
{
    public static class DiBuilder
    {
        public static void AddUnitOfWork(this IServiceCollection collection)
        {
            /*ollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();*/
            collection.AddInfrastructureServices();
            collection.AddRepository();
            collection.AddBusinessServices();
            collection.AddSalesServices();
            collection.AddCQRS();
            collection.AddScoped<IUserContextService, UserContextService>();
            collection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
        }


        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();

        }



        public static void AddTextileDb(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config["ConnectionString:TextileDb"]
                                     ?? throw new Exception("Connection string for TextileDb is not configured");

            services.AddDbContext<TextileDbContext>(options =>
            {
                var migrationAssembly = typeof(TextileDbContext).Assembly.GetName().Name;

                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(migrationAssembly);
                });
            });
        }

        /// <summary>
        /// Registers business/application services
        /// </summary>
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IMasterDataService, MasterDataService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminMenuService, AdminMenuService>();
            services.AddScoped<ISupplierContextService, SupplierContextService>();
            services.AddScoped<IStockGroupService, StockGroupService>();
            services.AddScoped<ISupplierProductService, SupplierProductService>();
            services.AddScoped<ISupplierStockGroupService, SupplierStockGroupService>();
            services.AddScoped<IHsnCodeService, HsnCodeService>();
            services.AddScoped<ISaleVoucherService, SaleVoucherService>();
            services.AddScoped<ISaleVoucherStatusService, SaleVoucherStatusService>();
            services.AddScoped<IGstRuleService, GstRuleService>();
            services.AddScoped<IPrintService, PrintService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ISupplierHsnCodeService, SupplierHsnCodeService>();
            services.AddScoped<IParcelService, ParcelService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
            services.AddScoped<ITallyConfigService, TallyConfigService>();
            services.AddScoped<ITallyNameService, TallyNameService>();
            

        }

       


        public static void AddSalesServices(this IServiceCollection services)
        {

            services.AddScoped<IPackingSlipService, PackingSlipService>();
            services.AddScoped<IVisitorService, VisitorService>();
            services.AddScoped<IFinanceYearService, FinanceYearService>();
            services.AddScoped<IDeliveryChallanService, DeliveryChallanService>();
            services.AddScoped<IDeliveryChallanReturnService, DeliveryChallanReturnService>();
            services.AddScoped<ISalesPersonService,SalesPersonService>();
            services.AddScoped<IDeliveryChallanToInvoiceService, DeliveryChallanToInvoiceService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IBillingPrintService, BillingPrintService>();
        }
    }
}
