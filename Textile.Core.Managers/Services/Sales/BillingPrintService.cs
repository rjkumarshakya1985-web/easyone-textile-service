using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Models;
using Textile.Core.Entities.Models.Response.BillingPrint;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services.Sales;

namespace Textile.Core.Managers.Services.Sales
{
    
    public class BillingPrintService : IBillingPrintService
    {

        private readonly TextileDbContext _context;

        public BillingPrintService(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryChallanPrintResponse?> GetDeliveryChallanPrint(int id)
        {
            // Single query with subquery for company detail
            var result = await _context.DeliveryChallans
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new DeliveryChallanPrintResponse
                {
                    Id = x.Id,
                    Date = x.Date,
                    SlipNo = x.DeliveryChallanNumber,
                    CashierName = x.User.UserName,
                  //  TotalTaxableAmount = x.TotalTaxableAmount,
                    TotalAmount = x.TotalAmount,
                    CompanyDetail = _context.SaleVoucherPrintDetails
                        .Select(s => new CompanyDetailResponse
                        {
                            Id = s.Id,
                            CompanyName = s.CompanyName,
                            Address1 = s.Address1,
                            Address2 = s.Address2,
                            Description = s.Description,
                            GstIn = s.GstIn,

                        })
                        .FirstOrDefault(),
                   
                    Customer = new CustomerPrintResponse
                    {
                        
                        Name = x.Customer.Name,
                        BillingAddress = x.Customer.BillingAddress,
                        CityName = x.Customer.City.Name,
                        GstIn = x.Customer.GstIn,
                            Pan = x.Customer.Pan,

                        PrintName = x.Customer.PrintName,
                        StateCode = x.Customer.City.State.Code,
                        StateName = x.Customer.City.State.Name

                    },
                    Items = x.DeliveryChallanItems.Select(i => new PackingSlipPrintItemResponse
                    {
                        Id = i.Id,
                        StockGroupName = i.Stock.Product.StockGroup.Name,
                        ProductName = i.Stock.Product.PrintName,
                        Quantity = i.Qty,
                        Rate = i.SaleRate,
                        TaxableAmount = i.TaxableAmount,
                      //  Amount = i.Amount,
                        Barcode = i.Stock.Product.Barcode,
                        GstPercentage = (int)i.GstPercent,
                        ReturnQty = i.ReturnQty,
                        HsnCode = i.Stock.Product.HsnCode
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            result.TotalQuantity = result.Items.Sum(i => i.Quantity);

            return result;
        }
        public async Task<PackingSlipPrintResponse?> GetPackingSlipPrint(int id)
        {
            // Single query with subquery for company detail
            var result = await _context.PackingSlips
        .AsNoTracking()
        .Where(x => x.Id == id && !x.IsDeleted)
        .Select(x => new PackingSlipPrintResponse
        {
            Id = x.Id,
            Date = x.Date,
            PackingSlipNo = x.SlipNumber,

            SalesManName = x.SalesPerson != null
                ? x.SalesPerson.Name
                : null,

            PackingSlipManName = x.User.UserName,

            Department = x.User.UserDetail != null &&
                         x.User.UserDetail.Department != null
                ? x.User.UserDetail.Department.Name
                : null,

            TotalQuantity = x.TotalQuantity,
            TotalAmount = x.TotalAmount,
            Remarks= x.Remarks,
            Visitor = x.Visitor == null ? null : new VisitorResponse
            {
                Id = x.Visitor.Id,
                CustomerId = x.CustomerId,
                Name = x.Visitor.Name,
                Mobile = x.Visitor.Mobile,
                CustomerType = x.Visitor.CustomerType,

                CustomerResponse = x.Visitor.Customer == null
                    ? null
                    : new CustomerResponse
                    {
                        Id = x.Visitor.Customer.Id,
                        Name = x.Visitor.Customer.Name,
                        Mobile = x.Visitor.Customer.Phone                            
                    }
            },

            CustomerResponse = x.Customer == null ? null : new CustomerResponse
            {
                Id = x.Customer.Id,
                Name = x.Customer.Name,
                Mobile = x.Customer.Mobile,
                CustomerType = x.Customer.CustomerType
            },

            CompanyDetail = _context.SaleVoucherPrintDetails
                .Select(s => new CompanyDetailResponse
                {
                    Id = s.Id,
                    CompanyName = s.CompanyName,
                    Address1 = s.Address1,
                    Address2 = s.Address2,
                    Description = s.Description,
                    GstIn = s.GstIn
                })
                .FirstOrDefault(),

            Items = x.Items.Select(i => new PackingSlipPrintItemResponse
            {
                Id = i.Id,
                StockGroupName = i.Stock.Product.StockGroup.Name,
                ProductName = i.Stock.Product.PrintName,
                Quantity = i.Qty,
                Rate = i.SaleRate,
                TaxableAmount = i.TaxableAmount,
                DiscountPercent = i.DiscountPercent,
                Barcode = i.Stock.Product.Barcode,
                HsnCode = i.Stock.Product.HsnCode
            }).ToList()
        })
        .FirstOrDefaultAsync();

            if (result == null)
                throw new Exception("Packing slip not found");

            return result;           

        }
       
    }
}
