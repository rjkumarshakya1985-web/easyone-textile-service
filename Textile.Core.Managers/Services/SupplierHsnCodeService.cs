using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SupplierHsnCodeService : ISupplierHsnCodeService
    {
        private readonly TextileDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public SupplierHsnCodeService(TextileDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<bool> AssignSupplierHsnCode(SupplierHsnCodeRequest request)
        {
            var supplierStockGroupRepository =
                 _unitOfWork.Repository<SupplierHsnCode, Guid>();

            var supplierRepository =
                _unitOfWork.Repository<Supplier, Guid>();

            var hsnCodeRepository =
                _unitOfWork.Repository<ProductHsnCode, Guid>();

            var stockGroupRepository =
               _unitOfWork.Repository<StockGroup, int>();

            // ---------------------------
            // Validate Supplier
            // ---------------------------
            var supplier = await supplierRepository
                .GetSingleAsync(x => x.Id == request.SupplierId);

            if (supplier == null)
                throw new Exception("Supplier not found");

            // ---------------------------
            // Validate Hsn Code
            // ---------------------------
            var hsnCode = await hsnCodeRepository
                .GetSingleAsync(x => x.Id == request.HsnCodeId);

            if (hsnCode == null)
                throw new Exception("Hsn code not found");


            // ---------------------------
            // Validate Stock Group
            // ---------------------------
            var stockGroup = await stockGroupRepository
                .GetSingleAsync(x => x.Id == request.StockGroupId);

            if (stockGroup == null)
                throw new Exception("Stock group not found");

            // ---------------------------
            // Check if mapping already exists
            // ---------------------------
            var existingMap = await supplierStockGroupRepository
                .GetSingleAsync(x =>
                    x.SupplierId == request.SupplierId &&
                    x.HsnCodeId == request.HsnCodeId && x.StockGroupId == request.StockGroupId);

            if (existingMap != null)
                throw new Exception("This hsn code  is already mapped to the stock group and supplier");

            // ---------------------------
            // Add new mapping
            // ---------------------------
            var newMapping = new SupplierHsnCode
            {
                SupplierId = request.SupplierId,
                HsnCodeId = request.HsnCodeId,
                StockGroupId = request.StockGroupId,
                IsActive = true
            };

            await supplierStockGroupRepository.AddAsync(newMapping);

            return true;
        }

        public async Task<TableResult<SupplierHsnCodeResponse>> GetSupplierHsnCodeMappings(TableDataRequest req)
        {
            var baseQuery = _context.SupplierHsnCodes
                .AsNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.HsnCode)
                .Include(x => x.StockGroup)
                .Where(x => x.IsActive);

            // SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string s = req.Search.Trim().ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Supplier.Name.ToLower().Contains(s) ||
                    x.Supplier.Code.ToLower().Contains(s));
            }

            // TOTAL ROWS (Supplier + StockGroup)
            int total = await baseQuery
                .Select(x => new { x.SupplierId, x.StockGroupId })
                .Distinct()
                .CountAsync();

            // PAGINATION (Supplier + StockGroup level)
            var pageKeys = await baseQuery
                .Select(x => new
                {
                    x.SupplierId,
                    SupplierName = x.Supplier.Name,
                    SupplierCode = x.Supplier.Code,
                    x.StockGroupId,
                    StockGroupName = x.StockGroup.Name
                })
                .Distinct()
                .OrderBy(x => x.SupplierName)
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            var supplierIds = pageKeys.Select(x => x.SupplierId).ToList();
            var stockGroupIds = pageKeys.Select(x => x.StockGroupId).ToList();

            var data = await baseQuery
                .Where(x =>
                    supplierIds.Contains(x.SupplierId) &&
                    stockGroupIds.Contains(x.StockGroupId))
                .ToListAsync();

            // GROUP RESULT (FIXED)
            var result = data
                .GroupBy(x => new
                {
                    x.SupplierId,
                    SupplierName = x.Supplier.Name,
                    SupplierCode = x.Supplier.Code,
                    x.StockGroupId,
                    StockGroupName = x.StockGroup.Name
                })
                .Select(g => new SupplierHsnCodeResponse
                {
                    SupplierId = g.Key.SupplierId,
                    Name = g.Key.SupplierName,
                    Code = g.Key.SupplierCode,
                    StockGroupId = g.Key.StockGroupId,
                    StockGroupName = g.Key.StockGroupName,
                    HsnCodeResponses = g.Select(x => new HsnCodeResponse
                    {
                        Id = x.HsnCodeId,
                        Name = x.HsnCode.Name,
                        Description = x.HsnCode.Description
                    }).ToList()
                })
                .ToList();

            return new TableResult<SupplierHsnCodeResponse>
            {
                TotalRows = total,
                Result = result
            };
        }

        public async Task<IEnumerable<HsnCodeResponse>> GetSupplierOrphanHsnCodes(Guid supplierId,int stockGroupId,string search)
        {
            var query = _context.HsnCodes
             .AsNoTracking()
             .Where(sg =>
                 !sg.IsDeleted &&
                 !_context.SupplierHsnCodes
                     .Any(ssg => ssg.HsnCodeId == sg.Id && ssg.SupplierId == supplierId && ssg.StockGroupId==stockGroupId)
             );

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(sg =>
                    sg.Name.Contains(search)
                );
            }

            var result = await query
                .OrderBy(sg => sg.Name)
                .Take(5)
                .Select(sg => new HsnCodeResponse
                {
                    Id = sg.Id,                  // int → int?
                    Name = sg.Name,
                    Description = sg.Description
                })
                .ToListAsync();

            return result;
        }

        public async Task<bool> SupplierHsnCodeDelete(SupplierHsnCodeRequest supplierStockGroup)
        {
            var request = supplierStockGroup;

            var repo = _unitOfWork.Repository<SupplierHsnCode, Guid>();


            // Check if mapping exists
            var mapping = await repo.GetSingleAsync(x =>
                x.SupplierId == request.SupplierId &&
                x.HsnCodeId == request.HsnCodeId && x.StockGroupId == request.StockGroupId);

            if (mapping == null)
                throw new Exception("Supplier hsn code mapping not found.");

            // Delete the mapping row
            await repo.DeleteAsync(mapping);

            return true;
        }

        public async Task<IEnumerable<HsnCodeResponse>> GetSupplierStockGroupHsnCodes(Guid supplierId, int stockGroupId)
        {
            var repository = _unitOfWork.Repository<SupplierHsnCode, Guid>();

            var supplierHsnCods = await repository.GetAllAsync(
                x => x.SupplierId == supplierId && x.StockGroupId==stockGroupId && x.IsActive,
                x => x.HsnCode
            );

            var result = supplierHsnCods.Select(x => new HsnCodeResponse
            {
                Id = x.HsnCode.Id,
                Name = x.HsnCode.Name,
                Description = x.HsnCode.Description
            });

            return result;
        }
    }
}
