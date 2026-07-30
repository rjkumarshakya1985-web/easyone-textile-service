using Microsoft.EntityFrameworkCore;
using System.Threading;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response.Masters;
using Textile.Core.Entities.Models.Response.StockGroups;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SupplierStockGroupService : ISupplierStockGroupService
    {

        private readonly TextileDbContext _context;
        private readonly IUnitOfWork _unitOfWork;


        public SupplierStockGroupService(TextileDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> AssignSupplierStockGroup(
      AddSupplierStockGroupRequest request)
        {
            var supplierStockGroupRepository =
                _unitOfWork.Repository<SupplierStockGroup, Guid>();

            var supplierRepository =
                _unitOfWork.Repository<Supplier, Guid>();

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
                    x.StockGroupId == request.StockGroupId);

            if (existingMap != null)
                throw new Exception("This stock group is already mapped to the supplier");

            // ---------------------------
            // Add new mapping
            // ---------------------------
            var newMapping = new SupplierStockGroup
            {
                SupplierId = request.SupplierId,
                StockGroupId = request.StockGroupId,
                IsActive = true
            };

            await supplierStockGroupRepository.AddAsync(newMapping);

            return true;
        }

        public async Task<TableResult<SupplierStockGroupResponse>> GetSupplierStockGroupMappings(TableDataRequest tableDataRequest)
        {
            var req = tableDataRequest;

            // Base query
            var query = _context.SupplierStockGroups
                .AsNoTracking()
                .Include(st => st.Supplier)
                .Include(st => st.StockGroup)
                .Select(st => new
                {
                    st.SupplierId,
                    SupplierName = st.Supplier.Name,
                    SupplerCode = st.Supplier.Code,
                    st.StockGroupId,
                    StockGroupName = st.StockGroup.Name,
                    st.IsActive
                });

            // SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.SupplierName.ToLower().Contains(s) ||
                    x.SupplerCode.ToLower().Contains(s)
                );
            }

            // Total after search
            int total = await query.CountAsync();

            // ORDER BY SUPPLIER
            query = query.OrderBy(x => x.SupplierName);

            // Pagination
            var data = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            // GROUP BY SUPPLIER
            var grouped = data
                .GroupBy(x => new { x.SupplierId, x.SupplierName, x.SupplerCode })
                .Select(g => new SupplierStockGroupResponse
                {
                    SupplierId = g.Key.SupplierId,
                    Name = g.Key.SupplierName,
                    Code = g.Key.SupplerCode,
                    StockGroupResponses = g.Select(t => new StockGroupResponse
                    {
                        Id = t.StockGroupId,
                        Name = t.StockGroupName
                    })
                })
                .ToList();

            return new TableResult<SupplierStockGroupResponse>
            {
                TotalRows = total,
                Result = grouped
            };
        }

        public async Task<IEnumerable<StockGroupResponse>> GetSupplierOrphanStockGroups(
         Guid supplierId)
        {

            var query = _context.StockGroups
                .AsNoTracking()
                .Where(sg =>
                    sg.IsActive &&
                    !sg.IsDeleted &&
                    !_context.SupplierStockGroups
                        .Any(ssg => ssg.StockGroupId == sg.Id && ssg.SupplierId == supplierId)
                );

            // Apply search


            var result = await query
                .OrderBy(sg => sg.Name)
                .Take(10)
                .Select(sg => new StockGroupResponse
                {
                    Id = sg.Id,                  // int → int?
                    Name = sg.Name,
                    GstValue = sg.GstValue,
                    Description = sg.Description,
                    IsActive = sg.IsActive,
                    IsDeleted = sg.IsDeleted
                })
                .ToListAsync();

            return result;
        }

        public async Task<bool> SupplierStockGroupDelete(SupplierStockGroupDeleteRequest supplierStockGroup)
        {
            var request = supplierStockGroup;

            bool hasActiveProducts = await _context.SupplierProducts
                      .AsNoTracking().AnyAsync(x =>
                      x.SupplierId == request.SupplierId &&
                      x.StockGroupId == request.StockGroupId &&
                     !x.IsDeleted &&
                      x.IsActive);

            if (hasActiveProducts)
            {
                throw new Exception(
                    "This stock group cannot be removed because active products exist for this supplier."
                );
            }


            var repo = _unitOfWork.Repository<SupplierStockGroup, Guid>();



            // Check if mapping exists
            var mapping = await repo.GetSingleAsync(x =>
                x.SupplierId == request.SupplierId &&
                x.StockGroupId == request.StockGroupId);

            if (mapping == null)
                throw new Exception("Supplier stock group mapping not found.");

            // Delete the mapping row
            await repo.DeleteAsync(mapping);

            return true;
        }

        public async Task<IEnumerable<StockGroupResponse>> SupplierStockGroups(Guid supplierId)
        {
            var repository = _unitOfWork.Repository<SupplierStockGroup, Guid>();

            var supplierStockGroups = await repository.GetAllAsync(
                x => x.SupplierId == supplierId && x.IsActive,
                x => x.StockGroup,
                x => x.StockGroup.GstRules
            );

            var result = supplierStockGroups.Select(sg => new StockGroupResponse
            {
                Id = sg.StockGroup.Id,
                Name = sg.StockGroup.Name,
                GstValue = sg.StockGroup.GstValue,
                Description = sg.StockGroup.Description,
                IsActive = sg.StockGroup.IsActive,
                IsDeleted = sg.StockGroup.IsDeleted,
                IsGstRule = sg.StockGroup.IsGstRule,

                GstRuleDtos = sg.StockGroup.IsGstRule
                    ? sg.StockGroup.GstRules
                        .OrderBy(r => r.ApplyOrder)
                        .Select(r => new GstRuleDto
                        {
                            Id = r.Id,
                            StockGroupId = r.StockGroupId,
                            StockGroupName = sg.StockGroup.Name,
                            GstValue = r.GstValue,
                            ApplyOrder = r.ApplyOrder,
                            StartRange = r.StartRange,
                            EndRange = r.EndRange
                        })
                        .ToList()
                    : new List<GstRuleDto>()   // ✅ EMPTY LIST
            });

            return result;
        }

    }
}
