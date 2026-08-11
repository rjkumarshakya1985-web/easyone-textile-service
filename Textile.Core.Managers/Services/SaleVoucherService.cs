using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.SaleVouchers;
using Textile.Core.Entities.Models.Response.SaleVouchers;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SaleVoucherService : ISaleVoucherService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public SaleVoucherService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> DeleteAsync(int id, Guid userId, string userName)
        {
            var repo = _unitOfWork.Repository<SaleVoucher, int>();

            var voucher = await repo.GetSingleAsync(x => x.Id == id && !x.IsDeleted);

            if (voucher == null)
                throw new Exception("Sale voucher not found.");

            voucher.IsDeleted = true;
            voucher.CreatedBy = userId;
            voucher.CreatedByUserName = userName;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public Task<IEnumerable<SaleVoucherDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleVoucherDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TableResult<SaleVoucherTableResponse>> GetTableData(
      TableDataRequest dataRequest,
      Guid? supplierId = null)
        {
            var query = BuildBaseQuery();

            query = ApplySupplierFilter(query, supplierId);
            query = ApplySearch(query, dataRequest.Search);
            query = ApplyFilters(query, dataRequest.Filters);
            query = ApplySorting(query, dataRequest.SortField, dataRequest.SortOrder);

            int total = await query.CountAsync();

            var data = await ApplyPagination(query, dataRequest)
                .Select(MapToResponse())
                .ToListAsync();

            return new TableResult<SaleVoucherTableResponse>
            {
                TotalRows = total,
                Result = data
            };
        }

        public async Task<TableResult<SaleVoucherMobileResponse>> GetMobileTableData(
            TableDataRequest dataRequest,
            Guid? supplierId = null)
        {
            var query = BuildBaseQuery();

            query = ApplySupplierFilter(query, supplierId);
            query = ApplySearch(query, dataRequest.Search);
            query = ApplyFilters(query, dataRequest.Filters);
            query = ApplySorting(query, dataRequest.SortField, dataRequest.SortOrder);

            int total = await query.CountAsync();

            var data = await ApplyPagination(query, dataRequest)
                .Select(x => new SaleVoucherMobileResponse
                {
                    Id = x.Id,
                    Date = x.Date,
                    SupplierName = x.Supplier.Name,
                    SupplierInvoice = x.SupplierBillNumber,
                    CompanyName = x.Supplier.TallyLedgerName ?? x.Supplier.Name,
                    Floor = x.Supplier.SubDepartment.Name,
                    ParcelStatus = (ParcelStatusEnum)x.Status,
                    StatusDate = x.SaleVoucherStatuses.OrderByDescending(s => s.Date).FirstOrDefault() != null
                        ? x.SaleVoucherStatuses.OrderByDescending(s => s.Date).FirstOrDefault()!.Date
                        : x.Date,
                    TotalQuantity = x.SaleVoucherDetails.Sum(d => d.Quantity)
                })
                .ToListAsync();

            return new TableResult<SaleVoucherMobileResponse>
            {
                TotalRows = total,
                Result = data
            };
        }

        public async Task<List<SaleVoucherMobileProductResponse>> GetMobileProductsAsync(
            int saleVoucherId,
            Guid? supplierId = null)
        {
            var query = _context.SaleVouchers
                .Where(x => !x.IsDeleted && x.Id == saleVoucherId);

            query = ApplySupplierFilter(query, supplierId);

            return await query
                .SelectMany(x => x.SaleVoucherDetails)
                .OrderBy(d => d.Product.StockGroup!.Name)
                .ThenBy(d => d.Product.Name)
                .Select(d => new SaleVoucherMobileProductResponse
                {
                    CategoryName = d.Product.StockGroup!.Name,
                    ProductName = d.Product.Name,
                    Description = d.Product.PrintName,
                    Barcode = d.Product.Barcode,
                    Quantity = d.Quantity
                })
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<SaleVoucher> ApplyPagination(
    IQueryable<SaleVoucher> query,
    TableDataRequest request)
        {
            return query
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);
        }

        private static Expression<Func<SaleVoucher, SaleVoucherTableResponse>> MapToResponse()
        {
            return x => new SaleVoucherTableResponse
            {
                Id = x.Id,
                Date = x.Date,
                TranportName = x.Transport.Name,
                SupplierName = x.Supplier.Name,
                NumberOfParcel = x.NumberOfParcel,
                ParcelStatus = (ParcelStatusEnum)x.Status,
                BillNumber = x.SupplierBillNumber,
                IsExported = x.IsExported,
                DepartmentName =x.Supplier.SubDepartment.Department.Name,
                LrNumber= x.LrNumber,
                LrDate= x.LrDate,
                StatusDate= x.SaleVoucherStatuses.OrderByDescending(s => s.Date).FirstOrDefault() != null ? x.SaleVoucherStatuses.OrderByDescending(s => s.Date).FirstOrDefault()!.Date : x.Date,
                ProductDetails = string.Join(", ",
                    x.SaleVoucherDetails.Select(d => d.Product.Name + " (" + d.Quantity + ")"))
            };
        }
        private IQueryable<SaleVoucher> BuildBaseQuery()
        {
            return _context.SaleVouchers
                .Where(x => !x.IsDeleted)
                .Include(x=>x.SaleVoucherStatuses)
                .Include(x => x.SaleVoucherDetails)
                .Include(x => x.Transport)
                .Include(x => x.Supplier.SubDepartment.Department)
                .AsNoTracking();
        }

        private IQueryable<SaleVoucher> ApplySearch(
    IQueryable<SaleVoucher> query,
    string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var s = search.Trim().ToLower();

            return query.Where(x =>
                x.SupplierBillNumber.ToLower().Contains(s) ||
                x.Id.ToString().Contains(s) ||
                x.Supplier.Name.ToLower().Contains(s)
            );
        }


        private IQueryable<SaleVoucher> ApplyFilters(
    IQueryable<SaleVoucher> query,
    Dictionary<string, string>? filters)
        {
            if (filters == null || !filters.Any())
                return query;

            filters.TryGetValue("fromDate", out var fromDateStr);
            filters.TryGetValue("toDate", out var toDateStr);

            // ✅ 2. Apply date filter
            if (!string.IsNullOrWhiteSpace(fromDateStr) &&
                !string.IsNullOrWhiteSpace(toDateStr) &&
                DateTime.TryParseExact(fromDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var fromDate) &&
                DateTime.TryParseExact(toDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var toDate))
            {
                // 🔥 BEST PRACTICE (SQL optimized)
                query = query.Where(x =>
                    x.Date >= fromDate &&
                    x.Date < toDate.AddDays(1));
            }

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value))
                    continue;

                var value = filter.Value.Trim().ToLower();

                switch (filter.Key)
                {
                    case "department":
                        query = query.Where(x => x.Supplier.SubDepartment.Department.Name.ToLower().Contains(value));
                        break;
                    case "parcelStatus":
                        if (int.TryParse(value, out var status))
                            query = query.Where(x => x.Status == status);
                        break;

                    case "supplierName":
                        query = query.Where(x => x.Supplier.Name.ToLower().Contains(value));
                        break;

                    case "tranportName":
                        query = query.Where(x => x.Transport.Name.ToLower().Contains(value));
                        break;

                    case "billNumber":
                        query = query.Where(x => x.SupplierBillNumber.ToLower().Contains(value));
                        break;

                    case "product":
                        query = query.Where(x => x.SaleVoucherDetails.Any(y => y.Product.Name.ToLower().Contains (value)));
                        break;

                    case "export":
                        bool isExport = Boolean.Parse(filter.Value);
                        query = query.Where(x => x.IsExported== isExport);
                        break;

                    case "saleVoucherNumber":
                        if (int.TryParse(value, out var id))
                            query = query.Where(x => x.Id == id);
                        break;
                }
            }

            return query;
        }
        private IQueryable<SaleVoucher> ApplySupplierFilter(
    IQueryable<SaleVoucher> query,
    Guid? supplierId)
        {
            if (supplierId.HasValue)
            {
                query = query.Where(x => x.SupplierId == supplierId.Value);
            }

            return query;
        }
        private IQueryable<SaleVoucher> ApplySorting(
      IQueryable<SaleVoucher> query,
      string? sortField,
      int sortOrder)
        {
            bool isAsc = sortOrder == 1;

            return sortField switch
            {
                "department" => isAsc ? query.OrderBy(x => x.Supplier.SubDepartment.Department.Name) : query.OrderByDescending(x => x.Supplier.SubDepartment.Department.Name),
                "saleVoucherNumber" => isAsc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id),
                "supplierName" => isAsc ? query.OrderBy(x => x.Supplier.Name) : query.OrderByDescending(x => x.Supplier.Name),
                "tranportName" => isAsc ? query.OrderBy(x => x.Transport.Name) : query.OrderByDescending(x => x.Transport.Name),
                "billNumber" => isAsc ? query.OrderBy(x => x.SupplierBillNumber) : query.OrderByDescending(x => x.SupplierBillNumber),
                "date" => isAsc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date),
                "parcelStatus" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
                _ => query.OrderByDescending(x => x.Id)
            };
        }

        public async Task<SaleVoucherDto> IsExport(int id)
        {
            var repo = _unitOfWork.Repository<SaleVoucher, int>();

            var voucher = await repo.GetSingleAsync(x => x.Id == id && !x.IsDeleted, x => x.Supplier);

            if (voucher == null)
                throw new Exception("Sale voucher not found.");

            voucher.IsExported = !voucher.IsExported;

            await repo.UpdateAsync(voucher);

            return new SaleVoucherDto
            {
                Id = voucher.Id,
                SupplierBillNumber = voucher.SupplierBillNumber,
                SupplierName = voucher.Supplier.Name,
                Date = voucher.Date,
                IsExported = voucher.IsExported,
                LrNumber = voucher.LrNumber
            };
        }

        public async Task<IEnumerable<SaleVoucherDto>> GetAllExportAsync()
        {
            var repo = _unitOfWork.Repository<SaleVoucher, int>();

            var vouchers = await repo.GetAllAsync(x => !x.IsDeleted && x.IsExported, x => x.Supplier,x=>x.Supplier.SubDepartment,x=>x.Supplier.SubDepartment.Department);

            return vouchers.OrderBy(x => x.Supplier?.Name)
                           .ThenBy(x => x.Supplier?.SubDepartment?.Department?.Name)
                           .ThenBy(x => x.Status)
                           .Select(x => new SaleVoucherDto
                           {
                Id = x.Id,
                LrNumber = x.LrNumber,
                SupplierBillNumber = x.SupplierBillNumber,
                Date = x.Date,
                SupplierName = x.Supplier.Name,
                IsExported = x.IsExported,
                Department = x.Supplier.SubDepartment.Department.Name,
                ParcelStatus = ((ParcelStatusEnum)x.Status).ToString()
            });
        }

        public async Task<bool> SaveLR(LrRequest request, Guid userId, string userName)
        {
            var repo = _unitOfWork.Repository<SaleVoucher, int>();

            var vouchers = await repo.GetSingleAsync(x => !x.IsDeleted && x.Id==request.Id);

          
            if (vouchers == null)
                throw new Exception("Sale voucher not found.");

            vouchers.LrNumber = request.LrNumber;
            vouchers.LrDate = request.LrDate;

            await repo.UpdateAsync(vouchers);

            return true;
        }
    }
}
