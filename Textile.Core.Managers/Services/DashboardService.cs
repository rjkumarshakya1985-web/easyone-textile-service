using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response.Dashboard;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;

        public DashboardService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<AdminDashboardResponse> GetAdminDashboard()
        {
            var response = new AdminDashboardResponse();

            response.SupplierCount = await _context.Suppliers.Where(x => !x.IsDeleted).CountAsync();

            response.InParcel = await _context.SaleVouchers.Where(x =>
                         x.Status == (int)ParcelStatusEnum.InTransit && !x.IsDeleted).CountAsync();
            response.OpenParcel = await _context.SaleVouchers.Where(x =>
                         x.Status == (int)ParcelStatusEnum.Opened && !x.IsDeleted).CountAsync();

            response.Transport = await _context.SaleVouchers.Where(x =>
                         x.Status == (int)ParcelStatusEnum.Transport && !x.IsDeleted).CountAsync();

            response.InTransitLatestSaleVouchers = await _context.SaleVouchers.Include(x => x.SaleVoucherDetails)
                .Where(x => !x.IsDeleted && x.Status == (int)ParcelStatusEnum.InTransit)
                .OrderByDescending(x => x.Id)
                .Select(x => new DashboardParcel
                {
                    SaleVoucherId = x.Id,
                    Date = x.Date,
                    TransportName = x.Transport.Name,
                    ProductQuantity = x.SaleVoucherDetails.Count,
                    Status = x.Status
                })
                .Take(5)
                .ToListAsync();


            response.InHouseLatestSaleVouchers = await _context.SaleVouchers.Include(x => x.SaleVoucherDetails)
                .Where(x => !x.IsDeleted && x.Status == (int)ParcelStatusEnum.Transport)
                .OrderByDescending(x => x.Id)
                .Select(x => new DashboardParcel
                {
                    SaleVoucherId = x.Id,
                    Date = x.Date,
                    TransportName = x.Transport.Name,
                    ProductQuantity = x.SaleVoucherDetails.Count,
                    Status = x.Status
                })
                .Take(5)
                .ToListAsync();


            return response;
        }

        public async Task<SupplierDashboardResponse> GetSupplierDashboard(Guid supplierId)
        {
            var response = new SupplierDashboardResponse();

            response.ProductCount = await _context.SupplierProducts
                .Where(x => x.SupplierId == supplierId)
                .CountAsync();

            response.InTransitParcelCount = await _context.SaleVouchers
                .Where(x => x.SupplierId == supplierId &&
                            x.Status == (int)ParcelStatusEnum.InTransit &&
                            !x.IsDeleted)
                .CountAsync();

            response.TransportParcelCount = await _context.SaleVouchers
                .Where(x => x.SupplierId == supplierId &&
                            x.Status == (int)ParcelStatusEnum.Transport &&
                            !x.IsDeleted)
                .CountAsync();

            response.AtLocationParcelCount = await _context.SaleVouchers
                .Where(x => x.SupplierId == supplierId &&
                            x.Status == (int)ParcelStatusEnum.PackedAtLocation &&
                            !x.IsDeleted)
                .CountAsync();

            response.OpenParcelCount = await _context.SaleVouchers
                .Where(x => x.SupplierId == supplierId &&
                            x.Status == (int)ParcelStatusEnum.Opened &&
                            !x.IsDeleted)
                .CountAsync();

            response.SaleVoucherCount = await _context.SaleVouchers
                .Where(x => x.SupplierId == supplierId)
                .CountAsync();

            response.LatestSaleVouchers = await _context.SaleVouchers.Include(x => x.SaleVoucherDetails)
                .Where(x => x.SupplierId == supplierId && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new DashboardParcel
                {
                    SaleVoucherId = x.Id,
                    Date = x.Date,
                    TransportName = x.Transport.Name,
                    ProductQuantity = x.SaleVoucherDetails.Count,
                    Status = x.Status
                })
                .Take(3)
                .ToListAsync();

            response.LatestOpenSaleVouchers = await _context.SaleVouchers.Include(x => x.SaleVoucherDetails)
                .Where(x => x.SupplierId == supplierId &&
                            x.Status == (int)ParcelStatusEnum.Opened &&
                            !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new DashboardParcel
                {
                    SaleVoucherId = x.Id,
                    Date = x.Date,
                    TransportName = x.Transport.Name,
                    ProductQuantity = x.SaleVoucherDetails.Count,
                    Status = x.Status
                })
                .Take(3)
                .ToListAsync();

            return response;
        }

    }

}
