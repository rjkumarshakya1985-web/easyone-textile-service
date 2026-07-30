using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response.Billing.DeliveryChallans;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services.Sales;

namespace Textile.Core.Managers.Services.Sales
{
    public class DeliveryChallanReturnService : IDeliveryChallanReturnService
    {
        private readonly TextileDbContext _context;

        public DeliveryChallanReturnService(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryChallanReturnDetailResponse?> GetDeliveryChallan(string number, int finYearId)
        {
            var challan = await _context.DeliveryChallans
                .Include(x => x.DeliveryChallanItems)
                    .ThenInclude(i => i.Stock)
                        .ThenInclude(s => s.Product)
                            .ThenInclude(p => p.StockGroup)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x =>
                    x.DeliveryChallanNumber == number &&
                    x.FinanceYearId == finYearId &&
                    !x.IsDeleted);

            if (challan == null)
                return null;

            var itemIds = challan.DeliveryChallanItems.Select(i => i.Id).ToList();

            var returnData = await _context.DeliveryChallanReturnItems
                .Where(x => itemIds.Contains(x.DeliveryChallanItemId))
                .GroupBy(x => x.DeliveryChallanItemId)
                .Select(g => new
                {
                    DeliveryChallanItemId = g.Key,
                    ReturnedQty = g.Sum(x => x.ReturnQty)
                })
                .ToListAsync();

            var returnDict = returnData.ToDictionary(x => x.DeliveryChallanItemId, x => x.ReturnedQty);

            var response = new DeliveryChallanReturnDetailResponse
            {
                DeliveryChallanId = challan.Id,
                DeliveryChallanDate = challan.Date,
                CustomerId = challan.CustomerId.Value,
                CustomerName = challan.Customer?.Name ?? "",
                CustomerType = challan.Customer?.CustomerType ?? 0,
                Status = (DeliveryChallanStatusEnum)challan.Status,
                DiscountPercent = challan.DiscountPercent
            };

            int totalQty = 0;
            decimal totalAmount = 0;
            foreach (var item in challan.DeliveryChallanItems)
            {
                var returned = returnDict.ContainsKey(item.Id) ? returnDict[item.Id] : 0;
                var balance = Math.Max(0, item.Qty - returned);
                totalQty += balance;
                totalAmount += item.TotalAmount;

                response.Items.Add(new DeliveryChallanReturnDetailItem
                {
                    StockId = item.StockId,
                    DeliveryChallanItemId = item.Id,
                    ProductCategory = item.Stock.Product.StockGroup.Name,
                    Barcode = item.Stock.Product.Barcode,
                    ProductName = item.Stock.Product.Name,
                    SaleRate = item.SaleRate,
                    Qty = item.Qty,
                    Returned = returned,
                    TaxableAmount = item.TaxableAmount,
                    DiscountAmount = item.DiscountAmount,
                    NetAmount = item.NetAmount,
                    GstPercent = item.GstPercent,
                    Balance = balance,
                    ReturnQty = 0,
                    Amount = item.TotalAmount,
                    StockQty = (int)item.Stock.AvailableQty
                });
            }
            response.TotalAmount = totalAmount;
            response.TotalQuantity = totalQty;
            return response;
        }

        public async Task<DeliveryChallanReturnDetailResponse?> GetDeliveryChallanForReturn(string number, int finYearId)
        {
            var challan = await _context.DeliveryChallans
                .Include(x => x.DeliveryChallanItems)
                    .ThenInclude(i => i.Stock)
                        .ThenInclude(s => s.Product)
                            .ThenInclude(p => p.StockGroup)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x =>
                    x.DeliveryChallanNumber == number &&
                    x.FinanceYearId == finYearId &&
                    !x.IsDeleted &&
                    x.Status < (int)DeliveryChallanStatusEnum.FullyReturned);

            if (challan == null)
                return null;

            var itemIds = challan.DeliveryChallanItems.Select(i => i.Id).ToList();

            var returnData = await _context.DeliveryChallanReturnItems
                .Where(x => itemIds.Contains(x.DeliveryChallanItemId))
                .GroupBy(x => x.DeliveryChallanItemId)
                .Select(g => new
                {
                    DeliveryChallanItemId = g.Key,
                    ReturnedQty = g.Sum(x => x.ReturnQty)
                })
                .ToListAsync();

            var returnDict = returnData.ToDictionary(x => x.DeliveryChallanItemId, x => x.ReturnedQty);

            var response = new DeliveryChallanReturnDetailResponse
            {
                DeliveryChallanId = challan.Id,
                DeliveryChallanDate = challan.Date,
                CustomerId = challan.CustomerId.Value,
                CustomerName = challan.Customer?.Name ?? "",
                TotalAmount = challan.TotalAmount,
                TotalTaxableAmount = challan.TotalAmount,
                Status = (DeliveryChallanStatusEnum)challan.Status,
                DiscountPercent = challan.DeliveryChallanItems.Any() ? challan.DeliveryChallanItems.First().DiscountPercent : 0
            };

            int totalQty = 0;
            foreach (var item in challan.DeliveryChallanItems)
            {

                var returned = returnDict.ContainsKey(item.Id) ? returnDict[item.Id] : 0;
                var balance = Math.Max(0, item.Qty - returned);
                totalQty += balance;

                response.Items.Add(new DeliveryChallanReturnDetailItem
                {
                    StockId = item.StockId,
                    DeliveryChallanItemId = item.Id,
                    ProductCategory = item.Stock.Product.StockGroup.Name,
                    Barcode = item.Stock.Product.Barcode,
                    ProductName = item.Stock.Product.Name,
                    SaleRate = item.SaleRate,
                    Qty = item.Qty,
                    Returned = returned,
                    Balance = balance,
                    DiscountPercent = item.DiscountPercent,
                    DiscountAmount = Math.Round(item.DiscountAmount, 2, MidpointRounding.AwayFromZero),
                    NetAmount = Math.Round(item.NetAmount, 2, MidpointRounding.AwayFromZero),
                    TaxableAmount = Math.Round(item.TaxableAmount, 2, MidpointRounding.AwayFromZero),
                    Amount = Math.Round(item.TotalAmount, 2, MidpointRounding.AwayFromZero),
                    
                    GstPercent = item.GstPercent,
                    ReturnQty = 0
                });
            }

            response.TotalQuantity = totalQty;
            return response;
        }
    }
}
