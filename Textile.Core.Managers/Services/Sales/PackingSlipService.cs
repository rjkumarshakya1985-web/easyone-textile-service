
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Entities.Models.Response.PackingSlip;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;

using Textile.Core.Interfaces.Services.Sales;

namespace Textile.Core.Managers.Services
{
    //public class PackingSlipService : IPackingSlipService
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly TextileDbContext _context;
    //    private readonly IMediator _mediator;


    //    public PackingSlipService(IUnitOfWork unitOfWork, TextileDbContext context, IMediator mediator)
    //    {
    //        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    //        _context = context ?? throw new ArgumentNullException(nameof(context));
    //        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    //    }

    //    public async Task<int> CreateAsync(PackingSlipRequest request, Guid currentUserId, string currentUserName)
    //    {
    //        if (request.Items == null || !request.Items.Any())
    //            throw new Exception("Packing slip must contain at least one item");

    //        // Generate Slip Number
    //        var command = new GenerateVoucherNumberCommand(VoucherTypeEnum.PackingSlip, request.FinanceYearId);
    //        var packingSlipNumber = await _mediator.Send(command);

    //        await using var transaction = await _context.Database.BeginTransactionAsync();

    //        try
    //        {

    //            var visitor = await _context.Visitors.Include(x => x.Customer)
    //                .FirstOrDefaultAsync(x => x.Id == request.VisitorId);

    //            // Get stock ids
    //            var stockIds = request.Items.Select(x => x.StockId).ToList();

    //            var stocks = await _context.Stocks
    //                .Where(x => stockIds.Contains(x.Id))
    //                .ToDictionaryAsync(x => x.Id);

    //            // if visitor has customer, get discount
    //            var packingSlip = new PackingSlip
    //            {
    //                SlipNumber = packingSlipNumber,
    //                Date = DateTime.UtcNow,
    //                FinanceYearId = request.FinanceYearId,
    //                SalesPersonId = request.SalesPersonId,
    //                VisitorId = request.VisitorId,
    //                UserId = currentUserId,
    //                Discount = visitor?.Customer?.Discount,
    //                IsDeleted = false,
    //                CreatedBy = currentUserId,
    //                CreatedByUserName = currentUserName,
    //                CreatedOn = DateTime.UtcNow
    //            };

    //            foreach (var itemRequest in request.Items)
    //            {
    //                if (!stocks.TryGetValue(itemRequest.StockId, out var stock))
    //                    throw new Exception($"Stock not found for Id {itemRequest.StockId}");

    //                if (stock.AvailableQty < itemRequest.Qty)
    //                    throw new Exception($"Insufficient stock for {stock.Id}");

    //                // Reserve stock
    //                stock.ReservedQty += itemRequest.Qty;

    //                packingSlip.Items.Add(new PackingSlipItem
    //                {
    //                    StockId = itemRequest.StockId,
    //                    SaleRate = itemRequest.SaleRate,
    //                    Qty = itemRequest.Qty,
    //                    GstValue = itemRequest.GstValue,
    //                    TaxableAmount = itemRequest.TaxableAmount,
    //                    Amount  = itemRequest.Amount
    //                });
    //            }

    //            // Calculate totals
    //            packingSlip.TotalQuantity = packingSlip.Items.Sum(x => x.Qty);
    //            packingSlip.TotalAmount = packingSlip.Items.Sum(x => x.Amount);
    //            packingSlip.TotalTaxableAmount = packingSlip.Items.Sum(x => x.TaxableAmount);

    //            // Add packing slip
    //            await _context.PackingSlips.AddAsync(packingSlip);

    //            // Save everything
    //            await _context.SaveChangesAsync();

    //            await transaction.CommitAsync();

    //            return packingSlip.Id;
    //        }
    //        catch
    //        {
    //            await transaction.RollbackAsync();
    //            throw;
    //        }
    //    }

    //    public async Task<bool> UpdateAsync(PackingSlipRequest request, Guid currentUserId, string currentUserName)
    //    {
    //        if (request.Items == null || !request.Items.Any())
    //            throw new Exception("Packing slip must contain at least one item");

    //        await using var transaction = await _context.Database.BeginTransactionAsync();

    //        try
    //        {
    //            var packingSlip = await _context.PackingSlips
    //                .Include(x => x.Items)
    //                .FirstOrDefaultAsync(x => x.Id == request.Id);

    //            if (packingSlip == null)
    //                throw new Exception("Packing slip not found");

    //            var allStockIds = request.Items.Select(x => x.StockId)
    //                .Union(packingSlip.Items.Select(x => x.StockId))
    //                .ToList();

    //            var stocks = await _context.Stocks
    //                .Where(x => allStockIds.Contains(x.Id))
    //                .ToDictionaryAsync(x => x.Id);

    //            // Update header
    //            packingSlip.SalesPersonId = request.SalesPersonId;
    //            packingSlip.VisitorId = request.VisitorId;

    //            packingSlip.ModifiedBy = currentUserId;
    //            packingSlip.ModifiedByUserName = currentUserName;
    //            packingSlip.ModifiedOn = DateTime.UtcNow;

    //            foreach (var itemRequest in request.Items)
    //            {
    //                var existingItem = packingSlip.Items
    //                    .FirstOrDefault(x => x.StockId == itemRequest.StockId);

    //                var stock = stocks[itemRequest.StockId];

    //                if (existingItem != null)
    //                {
    //                    var diff = itemRequest.Qty - existingItem.Qty;

    //                    stock.ReservedQty += diff;

    //                    existingItem.Qty = itemRequest.Qty;
    //                    existingItem.SaleRate = itemRequest.SaleRate;
    //                    existingItem.TaxableAmount = itemRequest.TaxableAmount;
    //                    existingItem.Amount = itemRequest.Amount;
    //                }
    //                else
    //                {
    //                    stock.ReservedQty += itemRequest.Qty;

    //                    packingSlip.Items.Add(new PackingSlipItem
    //                    {
    //                        StockId = itemRequest.StockId,
    //                        Qty = itemRequest.Qty,
    //                        SaleRate = itemRequest.SaleRate,
    //                        GstValue = itemRequest.GstValue,
    //                        TaxableAmount = itemRequest.TaxableAmount,
    //                        Amount = itemRequest.Amount
    //                    });
    //                }
    //            }

    //            // Remove deleted items
    //            var itemsToRemove = packingSlip.Items
    //                .Where(x => !request.Items.Any(r => r.StockId == x.StockId))
    //                .ToList();

    //            foreach (var removeItem in itemsToRemove)
    //            {
    //                var stock = stocks[removeItem.StockId];

    //                stock.ReservedQty -= removeItem.Qty;

    //                packingSlip.Items.Remove(removeItem);
    //            }

    //            // Recalculate totals
    //            packingSlip.TotalQuantity = request.Items.Sum(x => x.Qty);
    //            packingSlip.TotalAmount = request.Items.Sum(x => x.Amount);
    //            packingSlip.TotalTaxableAmount = request.Items.Sum(x => x.TaxableAmount);

    //            await _context.SaveChangesAsync();

    //            await transaction.CommitAsync();

    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            await transaction.RollbackAsync();
    //            throw new Exception($"Error updating packing slip: {ex.Message}", ex);
    //        }
    //    }

    //    public async Task<bool> DeleteAsync(int id, Guid currentUserId, string currentUserName)
    //    {
    //        var packingSlip = await _context.PackingSlips
    //            .Include(x => x.Items)
    //            .FirstOrDefaultAsync(x => x.Id == id);

    //        if (packingSlip == null)
    //            throw new Exception("Packing slip not found");

    //        foreach (var item in packingSlip.Items)
    //        {
    //            var stock = await _context.Stocks
    //                .FirstOrDefaultAsync(x => x.Id == item.StockId);

    //            if (stock != null)
    //            {
    //                stock.ReservedQty -= item.Qty;
    //                if (stock.ReservedQty < 0)
    //                    stock.ReservedQty = 0;
    //            }
    //        }

    //        packingSlip.IsDeleted = true;
    //        packingSlip.ModifiedBy = currentUserId;
    //        packingSlip.ModifiedByUserName = currentUserName;
    //        packingSlip.ModifiedOn = DateTime.UtcNow;

    //        await _context.SaveChangesAsync();

    //        return true;
    //    }


    //    private async Task<PackingSlipResponse?> GetByIdForBillingAsync(int id)
    //    {
    //        var packingSlip = await _context.PackingSlips.Include(x => x.Visitor)
    //             .ThenInclude(x => x.Customer)
    //            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    //        if (packingSlip == null)
    //            throw new Exception("Packing slip not found");

    //        var response = new PackingSlipResponse
    //        {
    //            Id = packingSlip.Id,
    //            SlipNumber = packingSlip.SlipNumber,
    //            Date = packingSlip.Date,

    //            FinanceYearId = packingSlip.FinanceYearId,
    //            UserId = packingSlip.UserId ?? Guid.Empty,
    //            TotalQuantity = packingSlip.TotalQuantity,
    //            TotalAmount = packingSlip.TotalAmount,
    //            Status = (PackingSlipStatusEnum)packingSlip.Status,
    //            Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
    //            {
    //                Id = packingSlip.Visitor.Id,
    //                Name = packingSlip.Visitor.Name,
    //                Mobile = packingSlip.Visitor.Mobile,
    //                CustomerType = packingSlip.Visitor.CustomerType,
    //                CustomerResponse = packingSlip.Visitor.Customer == null ? null :
    //                new CustomerResponse
    //                {
    //                    Id = packingSlip.Visitor.Customer.Id,
    //                    Name = packingSlip.Visitor.Customer.Name,
    //                    Mobile = string.Join(",",
    //                             new[]{
    //                                   packingSlip.Visitor?.Mobile,
    //                                   packingSlip.Visitor?.Customer?.Phone}.Where(x => !string.IsNullOrWhiteSpace(x)))
    //                }
    //            }
    //        };

    //        return response;
    //    }

    //    public async Task<PackingSlipResponse?> GetByIdAsync(int id)
    //    {
    //        var packingSlip = await _context.PackingSlips.Include(x => x.Visitor).ThenInclude(x => x.Customer)
    //            .Include(x => x.Items)

    //                .ThenInclude(x => x.Stock)
    //                    .ThenInclude(x => x.Product)
    //                        .ThenInclude(x => x.StockGroup)
    //            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    //        if (packingSlip == null)
    //            throw new Exception("Packing slip not found");

    //        var response = new PackingSlipResponse
    //        {
    //            Id = packingSlip.Id,
    //            SlipNumber = packingSlip.SlipNumber,
    //            Date = packingSlip.Date,
    //            SalesPersonId = packingSlip.SalesPersonId,
    //            FinanceYearId = packingSlip.FinanceYearId,
    //            UserId = packingSlip.UserId ?? Guid.Empty,
    //            TotalQuantity = packingSlip.TotalQuantity,
    //            TotalAmount = packingSlip.TotalAmount,
    //            TotalTaxableAmount = packingSlip.TotalTaxableAmount,
    //            Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
    //            {
    //                Id = packingSlip.Visitor.Id,
    //                Name = packingSlip.Visitor.Name,
    //                Mobile = packingSlip.Visitor.Mobile,
    //                CustomerType = packingSlip.Visitor.CustomerType,
    //                CustomerResponse = packingSlip.Visitor.Customer == null ? null :
    //                new CustomerResponse
    //                {
    //                    Id = packingSlip.Visitor.Customer.Id,
    //                    Name = packingSlip.Visitor.Customer.Name,
    //                    Mobile = string.Join(",",
    //                             new[]{
    //                                   packingSlip.Visitor?.Mobile,
    //                                   packingSlip.Visitor?.Customer?.Phone}.Where(x => !string.IsNullOrWhiteSpace(x)))
    //                }
    //            },
    //            Items = packingSlip.Items.Select(x => new PackingSlipItemResponse
    //            {
    //                Id = x.Id,
    //                StockId = x.StockId,
    //                StockGroup = x.Stock.Product.StockGroup.Name,
    //                ProductName = x.Stock.Product.Name,
    //                BarCode = x.Stock.Product.Barcode,
    //                Qty = x.Qty,
    //                AvailableQty = (int)x.Stock.AvailableQty,
    //                SaleRate = x.SaleRate,
    //                GstValue = x.GstValue,
    //                TaxableAmount = x.TaxableAmount,
    //                Amount = x.Amount
    //            }).ToList()
    //        };

    //        return response;
    //    }

    //    public async Task<PackingSlipResponse?> GetByPackingSlipNumberAsync(string number)
    //    {
    //        var currentFinanceYearId = await _context.FinanceYears.AsNoTracking()
    //            .Where(x => x.IsActive && !x.IsClosed)
    //            .Select(x => x.Id)
    //            .FirstOrDefaultAsync();

    //        var packingSlip = await _context.PackingSlips.AsNoTracking()
    //            .Where(x => x.SlipNumber == number
    //                     && x.FinanceYearId == currentFinanceYearId
    //                     && !x.IsDeleted && x.Status == (int)PackingSlipStatusEnum.Created)
    //            .Select(x => new { x.Id })
    //            .FirstOrDefaultAsync();

    //        if (packingSlip == null)
    //            return null;

    //        return await GetByIdAsync(packingSlip.Id);
    //    }

    //    public async Task<PackingSlipResponse?> GetPackingSlipNumberForBillingAsync(string number, int financeYearId)
    //    {

    //        var packingSlip = await _context.PackingSlips.AsNoTracking()
    //            .Where(x => x.SlipNumber == number
    //                     && x.FinanceYearId == financeYearId
    //                     && !x.IsDeleted && x.Status == (int)PackingSlipStatusEnum.Created)
    //            .Select(x => new { x.Id })
    //            .FirstOrDefaultAsync();

    //        if (packingSlip == null)
    //            return null;

    //        return await GetByIdForBillingAsync(packingSlip.Id);
    //    }


    //    public async Task<BillPackingSlipsResponse?> GetPackingSlipsNumberForBillingByVisitorIdAsync(int visitorId, int financeYearId)
    //    {
    //        var packingSlips = await _context.PackingSlips
    //            .Include(x => x.Visitor).ThenInclude(x => x.Customer)
    //            .Include(x => x.Items)
    //                .ThenInclude(x => x.Stock)
    //                    .ThenInclude(x => x.Product)
    //                        .ThenInclude(x => x.StockGroup)
    //             .Where(x => x.VisitorId == visitorId && x.FinanceYearId == financeYearId
    //                        && !x.IsDeleted
    //                        && x.Status == (int)PackingSlipStatusEnum.Created)
    //            .ToListAsync();

    //        if (packingSlips == null || !packingSlips.Any())
    //            new BillPackingSlipsResponse();

    //        var response = new BillPackingSlipsResponse
    //        {
    //            PackingSlips = packingSlips.Select(packingSlip => new PackingSlipResponse
    //            {
    //                Id = packingSlip.Id,
    //                SlipNumber = packingSlip.SlipNumber,
    //                Date = packingSlip.Date,
    //                FinanceYearId = packingSlip.FinanceYearId,
    //                UserId = packingSlip.UserId ?? Guid.Empty,
    //                TotalQuantity = packingSlip.TotalQuantity,
    //                TotalAmount = packingSlip.TotalAmount,

    //                Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
    //                {
    //                    Id = packingSlip.Visitor.Id,
    //                    Name = packingSlip.Visitor.Name,
    //                    Mobile = packingSlip.Visitor.Mobile,
    //                    CustomerType = packingSlip.Visitor.CustomerType,
    //                    CustomerResponse = packingSlip.Visitor.Customer == null ? null :
    //                    new CustomerResponse
    //                    {
    //                        Id = packingSlip.Visitor.Customer.Id,
    //                        Name = packingSlip.Visitor.Customer.Name,
    //                        Mobile = string.Join(",",
    //                            new[]
    //                            {
    //                        packingSlip.Visitor?.Mobile,
    //                        packingSlip.Visitor?.Customer?.Phone
    //                            }.Where(x => !string.IsNullOrWhiteSpace(x)))
    //                    }
    //                },



    //            }).ToList()
    //        };

    //        // ✅ Total calculation
    //        response.TotalPcs = response.PackingSlips.Sum(x => x.TotalQuantity);
    //        response.GrandTotal = response.PackingSlips.Sum(x => x.TotalAmount);

    //        return response;
    //    }

    //    public async Task<List<PackingSlipResponse>> GetPendingPackingSlipForBilling(
    //      Guid currentUserId,RoleEnum role,int? financeYearId)
    //    {
    //        var query = _context.PackingSlips
    //            .Include(x => x.Visitor)
    //                .ThenInclude(x => x.Customer)
    //            .Where(x => !x.IsDeleted
    //                        && x.Status == (int)PackingSlipStatusEnum.Created);

    //        // Filter by Finance Year
    //        if (financeYearId.HasValue)
    //        {
    //            query = query.Where(x => x.FinanceYearId == financeYearId.Value);
    //        }

    //        if(role == RoleEnum.PackingSlipOperator)
    //        {
    //            query = query.Where(x => x.UserId != null && x.UserId==currentUserId);
    //        }

    //        var packingSlips = await query
    //            .OrderByDescending(x => x.Date)
    //            .ToListAsync();

    //        /// company  = 
    //        /// 


    //        var response = packingSlips.Select(packingSlip => new PackingSlipResponse
    //        {
    //            Id = packingSlip.Id,
    //            SlipNumber = packingSlip.SlipNumber,
    //            Date = packingSlip.Date,
    //            FinanceYearId = packingSlip.FinanceYearId,
    //            UserId = packingSlip.UserId ?? Guid.Empty,
    //            TotalQuantity = packingSlip.TotalQuantity,
    //            TotalAmount = packingSlip.TotalAmount,
    //            Status = (PackingSlipStatusEnum)packingSlip.Status,
    //            Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
    //            {
    //                Id = packingSlip.Visitor.Id,
    //                Name = packingSlip.Visitor.Name,
    //                Mobile = packingSlip.Visitor.Mobile,
    //                CustomerType = packingSlip.Visitor.CustomerType,

    //                CustomerResponse = packingSlip.Visitor.Customer == null ? null :
    //                new CustomerResponse
    //                {
    //                    Id = packingSlip.Visitor.Customer.Id,
    //                    Name = packingSlip.Visitor.Customer.Name,
    //                    Mobile = string.Join(",",
    //                        new[]
    //                        {
    //                    packingSlip.Visitor?.Mobile,
    //                    packingSlip.Visitor?.Customer?.Phone
    //                        }.Where(x => !string.IsNullOrWhiteSpace(x)))
    //                }
    //            }

    //        }).ToList();

    //        return response;
    //    }
    //}


    public class PackingSlipService : IPackingSlipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public PackingSlipService(TextileDbContext context, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> DeleteAsync(int id, Guid currentUserId, string currentUserName)
        {
            var packingSlip = await _context.PackingSlips
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (packingSlip == null)
                throw new Exception("Packing slip not found");

            foreach (var item in packingSlip.Items)
            {
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(x => x.Id == item.StockId);

                if (stock != null)
                {
                    stock.ReservedQty -= item.Qty;
                    if (stock.ReservedQty < 0)
                        stock.ReservedQty = 0;
                }
            }

            packingSlip.IsDeleted = true;
            packingSlip.ModifiedBy = currentUserId;
            packingSlip.ModifiedByUserName = currentUserName;
            packingSlip.ModifiedOn = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<PackingSlipResponse?> GetByIdAsync(int id)
        {
            var packingSlip = await _context.PackingSlips.Include(x=>x.Customer).Include(x => x.Visitor).ThenInclude(x => x.Customer)
                .Include(x => x.Items)

                    .ThenInclude(x => x.Stock)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.StockGroup)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (packingSlip == null)
                throw new Exception("Packing slip not found");

            var response = new PackingSlipResponse
            {
                Id = packingSlip.Id,
                SlipNumber = packingSlip.SlipNumber,
                Date = packingSlip.Date,
                SalesPersonId = packingSlip.SalesPersonId,
                FinanceYearId = packingSlip.FinanceYearId,
                UserId = packingSlip.UserId ?? Guid.Empty,
                TotalQuantity = packingSlip.TotalQuantity,
                TotalAmount = packingSlip.TotalAmount,
                TotalTaxableAmount = packingSlip.TotalAmount - (packingSlip.TotalDiscount + packingSlip.TotalGst),
                Remarks=packingSlip.Remarks,
                Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
                {
                    Id = packingSlip.Visitor.Id,
                    CustomerId = packingSlip.CustomerId,
                    Name = packingSlip.Visitor.Name,
                    Mobile = packingSlip.Visitor.Mobile,
                    CustomerType = packingSlip.Visitor.CustomerType,
                    CustomerResponse = packingSlip.Visitor.Customer == null ? null :
                    new CustomerResponse
                    {
                        Id = packingSlip.Visitor.Customer.Id,
                        Name = packingSlip.Visitor.Customer.Name,
                        Mobile = string.Join(",",
                                 new[]{
                                       packingSlip.Visitor?.Mobile,
                                       packingSlip.Visitor?.Customer?.Phone}.Where(x => !string.IsNullOrWhiteSpace(x)))
                    }
                },
                CustomerResponse = packingSlip.Customer == null ? null : new CustomerResponse
                {
                    Id = packingSlip.Customer.Id,
                    Name = packingSlip.Customer.Name,
                    Mobile = string.Join(",",
                                 new[]{
                                           packingSlip.Customer.Mobile,
                                           packingSlip.Customer?.Phone}.Where(x => !string.IsNullOrWhiteSpace(x))),
                    CustomerType = packingSlip.Customer.CustomerType
                },
                Items = packingSlip.Items.Select(x => new PackingSlipItemResponse
                {
                    Id = x.Id,
                    StockId = x.StockId,
                    StockGroup = x.Stock.Product.StockGroup.Name,
                    ProductName = x.Stock.Product.Name,
                    BarCode = x.Stock.Product.Barcode,
                    Qty = x.Qty,
                    SaleRate = x.SaleRate,
                    TaxableAmount = x.TaxableAmount,
                    DiscountPercent = x.DiscountPercent,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    GstPercent = x.GstPercent,
                    TotalAmount = x.TotalAmount,
                    AvailableQty = (int)x.Stock.AvailableQty,
                }).ToList()
            };

            return response;
        }

        public async Task<PackingSlipResponse?> GetByPackingSlipNumberAsync(string number)
        {
            var currentFinanceYearId = await _context.FinanceYears.AsNoTracking()
                .Where(x => x.IsActive && !x.IsClosed)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var packingSlip = await _context.PackingSlips.AsNoTracking()
                .Where(x => x.SlipNumber == number
                         && x.FinanceYearId == currentFinanceYearId
                         && !x.IsDeleted && x.Status == (int)PackingSlipStatusEnum.Created)
                .Select(x => new { x.Id })
                .FirstOrDefaultAsync();

            if (packingSlip == null)
                return null;

            return await GetByIdAsync(packingSlip.Id);
        }

        public async Task<PackingSlipResponse?> GetPackingSlipNumberForBillingAsync(string number, int financeYearId)
        {
            var packingSlip = await _context.PackingSlips.AsNoTracking()
                .Where(x => x.SlipNumber == number
                         && x.FinanceYearId == financeYearId
                         && !x.IsDeleted && x.Status == (int)PackingSlipStatusEnum.Created)
                .Select(x => new { x.Id })
                .FirstOrDefaultAsync();

            if (packingSlip == null)
                return null;

            return await GetByIdForBillingAsync(packingSlip.Id);
        }

        private async Task<PackingSlipResponse?> GetByIdForBillingAsync(int id)
        {
            var packingSlip = await _context.PackingSlips.Include(x=>x.User).Include(x=>x.Customer).Include(x => x.Visitor)
                 .ThenInclude(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (packingSlip == null)
                throw new Exception("Packing slip not found");

            var response = new PackingSlipResponse
            {
                Id = packingSlip.Id,
                SlipNumber = packingSlip.SlipNumber,
                Date = packingSlip.Date,

                FinanceYearId = packingSlip.FinanceYearId,
                UserId = packingSlip.UserId ?? Guid.Empty,
                TotalQuantity = packingSlip.TotalQuantity,
                TotalAmount = packingSlip.TotalAmount,
                Status = (PackingSlipStatusEnum)packingSlip.Status,
                Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
                {
                    Id = packingSlip.Visitor.Id,
                    Name = packingSlip.Visitor.Name,
                    Mobile = packingSlip.Visitor.Mobile,
                    CustomerType = packingSlip.Visitor.CustomerType,
                    CustomerResponse = packingSlip.Visitor.Customer == null ? null :
                    new CustomerResponse
                    {
                        Id = packingSlip.Visitor.Customer.Id,
                        Name = packingSlip.Visitor.Customer.Name,
                        Mobile = string.Join(",",
                                 new[]{
                                           packingSlip.Visitor?.Mobile,
                                           packingSlip.Visitor?.Customer?.Phone}.Where(x => !string.IsNullOrWhiteSpace(x)))
                    }
                },
                CustomerResponse =packingSlip.Customer==null?null : new CustomerResponse
                {
                    Id = packingSlip.Customer.Id,
                    Name = packingSlip.Customer.Name,
                    Mobile = string.Join(",",
                                 new[]{
                                           packingSlip.Customer.Mobile,
                                           packingSlip.Customer?.Phone}.Where(x => !string.IsNullOrWhiteSpace(x))),
                    CustomerType = packingSlip.Customer.CustomerType
                }

            };

            return response;
        }

        public Task<BillPackingSlipsResponse?> GetPackingSlipsNumberForBillingByVisitorIdAsync(int visitorId, int financeYearId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PackingSlipResponse>> GetPendingPackingSlipForBilling(Guid currentUserId, RoleEnum role, int? financeYearId)
        {
            if(financeYearId==null)
            {
                financeYearId = await _context.FinanceYears.AsNoTracking()
                .Where(x => x.IsActive && !x.IsClosed)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            }
            var query = _context.PackingSlips.Include(x=>x.Customer)
                .Include(x => x.Visitor)
                    .ThenInclude(x => x.Customer)
                .Where(x => !x.IsDeleted
                            && x.Status == (int)PackingSlipStatusEnum.Created);

                    // Filter by Finance Year
                    if (financeYearId.HasValue)
                    {
                        query = query.Where(x => x.FinanceYearId == financeYearId.Value);
                    }

                    if(role == RoleEnum.PackingSlipOperator)
                    {
                        query = query.Where(x => x.UserId != null && x.UserId==currentUserId);
                    }

                    var packingSlips = await query
                        .OrderByDescending(x => x.Date)
                        .ToListAsync();

                    /// company  = 
                    /// 


                    var response = packingSlips.Select(packingSlip => new PackingSlipResponse
                    {
                        Id = packingSlip.Id,
                        SlipNumber = packingSlip.SlipNumber,
                        Date = packingSlip.Date,
                        FinanceYearId = packingSlip.FinanceYearId,
                        UserId = packingSlip.UserId ?? Guid.Empty,
                        TotalQuantity = packingSlip.TotalQuantity,
                        TotalAmount = packingSlip.TotalAmount,
                        Status = (PackingSlipStatusEnum)packingSlip.Status,
                        CustomerResponse = packingSlip.Customer == null ? null :
                            new CustomerResponse
                            {
                                Id = packingSlip.Customer.Id,
                                Name = packingSlip.Customer.Name,
                                Mobile = string.Join(",",
                                    new[]
                                    {
                                packingSlip.Customer?.Mobile,
                                packingSlip.Customer?.Phone
                                    }.Where(x => !string.IsNullOrWhiteSpace(x)))
                            },
                        Visitor = packingSlip.Visitor == null ? null : new VisitorResponse
                        {
                            Id = packingSlip.Visitor.Id,
                            Name = packingSlip.Visitor.Name,
                            Mobile = packingSlip.Visitor.Mobile,
                            CustomerType = packingSlip.Visitor.CustomerType,

                            CustomerResponse = packingSlip.Visitor.Customer == null ? null :
                            new CustomerResponse
                            {
                                Id = packingSlip.Visitor.Customer.Id,
                                Name = packingSlip.Visitor.Customer.Name,
                                Mobile = string.Join(",",
                                    new[]
                                    {
                                packingSlip.Visitor?.Mobile,
                                packingSlip.Visitor?.Customer?.Phone
                                    }.Where(x => !string.IsNullOrWhiteSpace(x)))
                            }
                        }

                    }).ToList();

                    return response;
        }

        public async Task<TableResult<PackingSlipListResponse>> GetTableData(TableDataRequest tableDataRequest)
        {
            var query = _context.PackingSlips.Include(x=>x.Visitor).AsNoTracking().Where(x => !x.IsDeleted && x.Status == (int)PackingSlipStatusEnum.Created);

           
            if (!string.IsNullOrWhiteSpace(tableDataRequest.Search))
            {
                var s = tableDataRequest.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.SlipNumber.ToLower().Contains(s));
            }

            int total = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.SlipNumber)
                .Skip(tableDataRequest.PageIndex * tableDataRequest.PageSize)
                .Take(tableDataRequest.PageSize)
                .ToListAsync();

            return new TableResult<PackingSlipListResponse>
            {
                TotalRows = total,
                Result = data.Select(x => new PackingSlipListResponse
                {
                    Id = x.Id,
                    SlipNumber = x.SlipNumber,
                    VisitorName = x.Visitor != null ? x.Visitor.Name : string.Empty,
                    Date = x.Date,
                    TotalQuantity = x.TotalQuantity,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status
                }).ToList()
            };
        }
    }
}