using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Dto;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Exceptions;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SupplierProductService(IUnitOfWork unitOfWork, TextileDbContext context) : ISupplierProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly TextileDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<IEnumerable<SupplierProductDto>> GetAllAsync()
        {
            var repository = _unitOfWork.Repository<SupplierProduct, Guid>();
            var products = await repository.GetAllAsync();

            return products.Select(x => new SupplierProductDto
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                SupplierName =x.Supplier.Name,
                StockGroupId = x.StockGroupId,
                Name = x.Name,
                Alias = x.Alias,
                PrintName = x.PrintName,
                HsnCode = x.HsnCode,
                Barcode = x.Barcode,
                GstApplicable = x.GstApplicable,
                GSTNature = x.GSTNature,
                GSTTaxability = x.GSTTaxability,
                PurchaseRate = x.PurchaseRate,
                Discount = x.Discount,  // gst columns
                ManualWholeSaleRate = x.ManualWholeSaleRate,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                CreatedBy = x.CreatedBy,
                CreatedByUserName = x.CreatedByUserName,
                CreatedOn = x.CreatedOn,
                ModifiedBy = x.ModifiedBy,
                ModifiedByUserName = x.ModifiedByUserName,
                ModifiedOn = x.ModifiedOn
            });
        }

        public async Task<SupplierProductDto?> GetByIdAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<SupplierProduct, Guid>();
            var product = await repository.GetByIdAsync(id,x=>x.Supplier,x=>x.StockGroup);

            if (product == null)
                return null;

            var hsnCode = await _context.HsnCodes
                .FirstOrDefaultAsync(x => x.Name == product.HsnCode);

            return new SupplierProductDto
            {
                Id = product.Id,
                StockGroupName = product.StockGroup.Name,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier.Name,
                StockGroupId = product.StockGroupId,
                Name = product.Name,
                Alias = product.Alias,
                PrintName = product.PrintName,
                HsnCode = product.HsnCode,
                Barcode = await FetchNextBarcodeNumber(),
                GstApplicable = product.GstApplicable,
                GSTNature = product.GSTNature,
                GSTTaxability = product.GSTTaxability,
                PurchaseRate = product.PurchaseRate,
                Discount = product.Discount, // gst column
                ManualWholeSaleRate = product.ManualWholeSaleRate,
                IsActive = product.IsActive,
                IsDeleted = product.IsDeleted,
                CreatedBy = product.CreatedBy,
                CreatedByUserName = product.CreatedByUserName,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedByUserName = product.ModifiedByUserName,
                ModifiedOn = product.ModifiedOn,
                HsnCodeObj = hsnCode != null ? new HsnCodeResponse
                {
                    Id = hsnCode.Id,
                    Name = hsnCode.Name,
                    Description = hsnCode.Description
                } : null,
                SupplierObj = product.Supplier != null ? new SupplierTableResponse
                {
                    Id= product.Supplier.Id,
                    Name = product.Supplier.Name,
                    Code = product.Supplier.Code
                } : null

            };
        }


        public async Task<bool> CreateAsync(SupplierProductRequest request, Guid currentUserId, string currentUserName)
        {
            var repository = _unitOfWork.Repository<SupplierProduct, Guid>();

            var entity = new SupplierProduct
            {
                Id = Guid.NewGuid(),
                SupplierId = request.SupplierId,
                StockGroupId = request.StockGroupId,
                Name = request.Name,
                Alias = request.Alias,
                PrintName = request.PrintName,
                HsnCode = request.HsnCode,
                Barcode =  await FetchNextBarcodeNumber(),
                GstApplicable = request.GstApplicable,
                GSTNature = request.GSTNature,
                GSTTaxability = request.GSTTaxability,
                PurchaseRate = request.PurchaseRate,
                Discount = request.Discount,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = currentUserId,
                CreatedByUserName = currentUserName,
                ManualWholeSaleRate = request.ManualWholeSaleRate,
                CreatedOn = DateTime.UtcNow
            };

            await repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(SupplierProductRequest request, Guid currentUserId,
            string currentUserName, RoleEnum role)
        {
            var repository = _unitOfWork.Repository<SupplierProduct, Guid>();
            var product = await repository.GetByIdAsync(request.Id.Value);
            if (product == null)
                return false;


            bool isAdmin = role == RoleEnum.SuperAdmin;

            if (request.PurchaseRate > product.PurchaseRate && !isAdmin)
            {
                throw new BusinessException("Only admin can increase purchase rate.");
            }

            if (request.PurchaseRate != product.PurchaseRate)
            {
                product.ManualWholeSaleRate = null;
            }

            // Only Admin can change the manual whole sale rate
            if (isAdmin && request.PurchaseRate == product.PurchaseRate)
            {
                product.ManualWholeSaleRate = request.ManualWholeSaleRate;
            }

            product.StockGroupId = request.StockGroupId;
            product.Name = request.Name;
            product.Alias = request.Alias;
            product.PrintName = request.PrintName;
            product.HsnCode = request.HsnCode;
            product.Barcode = request.Barcode;
            product.GstApplicable = request.GstApplicable;
            product.GSTNature = request.GSTNature;
            product.GSTTaxability = request.GSTTaxability;
            product.PurchaseRate = request.PurchaseRate;
            product.Discount = request.Discount;
            product.ModifiedBy = currentUserId;
            product.ModifiedByUserName = currentUserName;
            product.ModifiedOn = DateTime.UtcNow;

            await repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<SupplierProduct, Guid>();
            var product = await repository.GetByIdAsync(id); // replace with proper conversion if needed
            if (product == null)
                return false;

            product.IsDeleted = true;
            await repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<SupplierProduct, Guid>();
            var product = await repository.GetByIdAsync(id); // replace with proper conversion if needed
            if (product == null)
                return false;

            product.IsActive = !product.IsActive;
            await repository.UpdateAsync(product);
            return true;
        }

        public async Task<TableResult<SupplierProductDto>> GetTableData(
          TableDataRequest req,
         Guid? supplierId = null)
        {
            var query = _context.SupplierProductViews
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            //  OPTIONAL SUPPLIER FILTER
            if (supplierId.HasValue)
            {
                query = query.Where(x => x.SupplierId == supplierId.Value);
            }

            //  SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.Alias.ToLower().Contains(s) ||
                    x.PrintName.ToLower().Contains(s) ||
                    x.HsnCode.ToLower().Contains(s) || 
                    x.SupplierName.ToLower().Contains(s));
                    
            }

          

            query = ApplyFilters(query, req.Filters);
            query = ApplySorting(query, req.SortField, req.SortOrder);
             int total = await query.CountAsync();
            var data = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new SupplierProductDto
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    SupplierName = x.SupplierName,

                    StockGroupId = x.StockGroupId,
                    StockGroupName = x.StockGroupName,
                    Name = x.Name,
                    Alias = x.Alias,
                    PrintName = x.PrintName,
                    HsnCode = x.HsnCode,
                    Barcode = x.Barcode,
                    GstApplicable = x.GstApplicable,
                    GSTNature = x.GSTNature,
                    GSTTaxability = x.GSTTaxability,
                    PurchaseRate = x.PurchaseRate,
                    Discount = x.Discount,  // gst column
                    ManualWholeSaleRate =x.ManualWholeSaleRate,
                    WholeSaleRate = x.WholeSaleRate,
                    RetailPrice = x.RetailPrice,
                    MrpRate = x.MrpRate,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedBy = x.CreatedBy,
                    CreatedByUserName = x.CreatedByUserName,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedByUserName = x.ModifiedByUserName,
                    ModifiedOn = x.ModifiedOn
                })
                .ToListAsync();

            return new TableResult<SupplierProductDto>
            {
                TotalRows = total,
                Result = data
            };
        }


        private IQueryable<SupplierProductView> ApplyFilters(
          IQueryable<SupplierProductView> query,
         Dictionary<string, string>? filters)
        {
            if (filters == null || !filters.Any())
                return query;

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value))
                    continue;

                var value = filter.Value.Trim().ToLower();

                switch (filter.Key)
                {
                    case "name":
                        query = query.Where(x => x.Name.ToLower().Contains(value));
                        break;

                    case "printName":
                        query = query.Where(x => x.PrintName.ToLower().Contains(value));
                        break;

                    case "barcode":
                        query = query.Where(x => x.Barcode.ToLower().Contains(value));
                        break;

                    case "supplierName":
                        query = query.Where(x => x.SupplierName.ToLower().Contains(value));
                        break;

                    case "hsnCode":
                        query = query.Where(x => x.HsnCode.ToLower().Contains(value));
                        break;

                    case "isActive":
                        if (bool.TryParse(value, out var isActive))
                        {
                            query = query.Where(x => x.IsActive == isActive);
                        }
                        break;

               
                }
            }

            return query;
        }
        public async Task<string> FetchNextBarcodeNumber()
        {
            var lastCode = await _context.SupplierProducts
               .Where(s => !string.IsNullOrEmpty(s.Barcode))
               .OrderByDescending(s => s.Barcode.Length)
               .ThenByDescending(s => s.Barcode)
               .Select(s => s.Barcode)
               .FirstOrDefaultAsync();

            int next = (lastCode == null) ? 1 : int.Parse(lastCode);
            return (next + 1).ToString("D6");

        }


        public Task<IEnumerable<SupplierProductDto?>> GetAllByStockCategoryAsync(Guid? supplierId)
        {
            throw new NotImplementedException();
        }

        private IQueryable<SupplierProductView> ApplySorting(
    IQueryable<SupplierProductView> query,
    string? sortField,
    int sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortField))
            {
                // Default sorting
                return query.OrderByDescending(x => x.CreatedOn);
            }

            return (sortField.ToLower(), sortOrder) switch
            {
                ("name", 1) => query.OrderBy(x => x.Name),
                ("name", -1) => query.OrderByDescending(x => x.Name),

                ("suppliername", 1) => query.OrderBy(x => x.SupplierName),
                ("suppliername", -1) => query.OrderByDescending(x => x.SupplierName),

                ("categoryname", 1) => query.OrderBy(x => x.StockGroupName),
                ("categoryname", -1) => query.OrderByDescending(x => x.StockGroupName),

                ("barcode", 1) => query.OrderBy(x => x.Barcode),
                ("barcode", -1) => query.OrderByDescending(x => x.Barcode),

                ("purchaserate", 1) => query.OrderBy(x => x.PurchaseRate),
                ("purchaserate", -1) => query.OrderByDescending(x => x.PurchaseRate),

                ("printname", 1) => query.OrderBy(x => x.PrintName),
                ("printname", -1) => query.OrderByDescending(x => x.PrintName),



                _ => query.OrderByDescending(x => x.CreatedOn)
            };
        }

        public async Task<SupplierProductDto?> GetProductViewByIdAsync(Guid id)
        {
            
            
            var product = await _context.SupplierProductViews.FirstOrDefaultAsync(x => x.Id==id);

            if (product == null)
                return null;

            var hsnCode = await _context.HsnCodes
                .FirstOrDefaultAsync(x => x.Name == product.HsnCode);

            return new SupplierProductDto
            {
                Id = product.Id,
                StockGroupName = product.StockGroupName,
                SupplierId = product.SupplierId,
                SupplierName = product.SupplierName,
                StockGroupId = product.StockGroupId,
                Name = product.Name,
                Alias = product.Alias,
                PrintName = product.PrintName,
                HsnCode = product.HsnCode,
                Barcode = product.Barcode,
                GstApplicable = product.GstApplicable,
                GSTNature = product.GSTNature,
                GSTTaxability = product.GSTTaxability,
                PurchaseRate = product.PurchaseRate,
                Discount = product.Discount, // gst column
                ManualWholeSaleRate = product.ManualWholeSaleRate,
                IsActive = product.IsActive,
                IsDeleted = product.IsDeleted,
                CreatedBy = product.CreatedBy,
                CreatedByUserName = product.CreatedByUserName,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedByUserName = product.ModifiedByUserName,
                ModifiedOn = product.ModifiedOn,
                WholeSaleRate= product.WholeSaleRate,
                RetailPrice = product.RetailPrice,
                MrpRate = product.MrpRate
            };
        }

        public async Task<IEnumerable<SupplierProductPriceHistoryDto>> GetProductPriceHistory(Guid productId)
        {
            var repository = _unitOfWork.Repository<SupplierProductPriceHistory, int>();

            var products = await repository.GetAllAsync(x=>x.SupplierProductId==productId && x.IsDeleted==false);

            return products.Select(x => new SupplierProductPriceHistoryDto
            {
                Id = x.Id,
                Date = x.Date,
                PurchaseRate = x.PurchaseRate,
                WholesaleRate = x.WholesaleRate,
                RetailRate = x.RetailRate
            });
        }

        public async Task<bool> UpdateProductPriceHistoryAsync(List<int> saleVoucherIds)
        {
            var products = await _context.SaleVoucherDetails
                .Where(x => saleVoucherIds.Contains(x.SaleVoucherId))
                .ToListAsync();

            var productIds = products
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var lastHistories = await _context.SupplierProductPriceHistories
                .Where(x => productIds.Contains(x.SupplierProductId))
                .GroupBy(x => x.SupplierProductId)
                .Select(g => g.OrderByDescending(x => x.Id).First())
                .ToDictionaryAsync(x => x.SupplierProductId);

            var historiesToAdd = new List<SupplierProductPriceHistory>();

            foreach (var product in products)
            {
                lastHistories.TryGetValue(product.ProductId, out var lastHistory);

                bool shouldInsert =
                    lastHistory == null ||
                    lastHistory.PurchaseRate != product.PurchaseRate ||
                    lastHistory.WholesaleRate != product.WholeSaleRate ||
                    lastHistory.RetailRate != product.RetailPrice;

                if (shouldInsert)
                {
                    historiesToAdd.Add(new SupplierProductPriceHistory
                    {
                        Date = DateTime.Now,
                        SupplierProductId = product.ProductId,
                        PurchaseRate = product.PurchaseRate,
                        WholesaleRate = product.WholeSaleRate,
                        RetailRate = product.RetailPrice,
                        IsDeleted = false
                    });
                }
            }

            if (historiesToAdd.Any())
            {
                await _context.SupplierProductPriceHistories.AddRangeAsync(historiesToAdd);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteProductPriceHistoryAsync(int historyId)
        {
            var history = await _context.SupplierProductPriceHistories
                .FirstOrDefaultAsync(x => x.Id == historyId);

            if (history == null)
                return false;

            history.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
