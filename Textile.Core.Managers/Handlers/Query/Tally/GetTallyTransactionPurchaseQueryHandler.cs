using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Entities.Models.Response.Departments;
using Textile.Core.Entities.Models.Response.StockGroups;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Entities.Models.Response.Tally;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Tally;

namespace Textile.Core.Managers.Handlers.Query.Tally
{

    public class GetTallyTransactionPurchaseQueryHandler
      : IRequestHandler<GetTallyTransactionPurchaseQuery, TallyTransactionPurchaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TextileDbContext _TextileDbContext;

        public GetTallyTransactionPurchaseQueryHandler(
          IUnitOfWork unitOfWork,
          IMapper mapper, TextileDbContext textileDbContext)
        {
            _unitOfWork = unitOfWork ??
              throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ??
              throw new ArgumentNullException(nameof(mapper));
            _TextileDbContext = textileDbContext ??
              throw new ArgumentNullException(nameof(textileDbContext));

        }

        public async Task<TallyTransactionPurchaseResponse> Handle(
          GetTallyTransactionPurchaseQuery request,
          CancellationToken cancellationToken)
        {
            var saleVoucherRepo = _unitOfWork.Repository<SaleVoucher,
              int>();
            var saleVoucherDetailRepo = _unitOfWork.Repository<SaleVoucherDetail,
              Guid>();
            var saleVoucherPrintRepo = _unitOfWork.Repository<SaleVoucherPrintDetail, int>();

            // 1️ Supplier with relations           
            var saleVoucher = await _TextileDbContext.SaleVouchers
              .AsNoTracking()
              .Include(x => x.Transport)
              .Include(x => x.Supplier).ThenInclude(s => s.City.State)
              .Include(s => s.Supplier.Agent)
              .Include(s => s.Supplier.SubDepartment.Department)
              .Include(s => s.Supplier.SupplierProducts)
              .ThenInclude(sp => sp.StockGroup)
              .ThenInclude(sg => sg.GstRules)
              .Include(x => x.Supplier)
              .ThenInclude(s => s.SupplierProducts)
              .ThenInclude(sp => sp.SupplierProductPriceHistories)
              .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (saleVoucher is null)
                throw new KeyNotFoundException($"SaleVoucher not found. Id: {request.Id}");

            var status = (ParcelStatusEnum)saleVoucher.Status;

            // ✅ Allow only Packed at Location (5) Opened (6) and TallySynched (11)
            if (status != ParcelStatusEnum.PackedAtLocation && status != ParcelStatusEnum.Opened && status != ParcelStatusEnum.TallySynced)
            {
                throw new InvalidOperationException($"Voucher must be 'Packed at Location', 'Opened' or 'TallySynched'. Current: {status}");
            }

            // 2️ Details with Product (materialize once)
            var saleVoucherDetails = (await saleVoucherDetailRepo.GetAllAsync(
                x => x.SaleVoucherId == saleVoucher.Id,
                x => x.Product,
                x => x.Product.StockGroup,
                x => x.Product.StockGroup.GstRules,
                x=>x.Product.SupplierProductPriceHistories))
              .ToList();

            if (saleVoucherDetails.Count == 0)
                throw new KeyNotFoundException("SaleVoucher item not found for print.");

            // 3 Print master data (fetch once, not full list)
            var billingInformation = (await saleVoucherPrintRepo.GetAllAsync())
                .FirstOrDefault()
                ?? throw new KeyNotFoundException("SaleVoucher print detail not found.");

            // 4 SaleVoucherPrint          
            var saleVoucherPrint = new SaleVoucherPrint
            {
                Id = saleVoucher.Id,
                CompanyName = saleVoucher.Supplier.Name,
                Address = string.Join(", ",
                    billingInformation.Address1,
                    billingInformation.Address2),
                InVoiceNo = saleVoucher.SupplierBillNumber,
                VoucherForeignkey = $"{saleVoucher.Id}-{saleVoucher.Date:yyyyMMdd}",
                TransportName = saleVoucher.Transport?.Name,
                SupplierBillNumber = saleVoucher.SupplierBillNumber,
                Date = saleVoucher.Date,                
                GstIn = billingInformation.GstIn,
                Discount = saleVoucher.Discount,
                LrNumber=saleVoucher.LrNumber,
                LrDate=saleVoucher.LrDate,
                DueDate=saleVoucher.Date.AddDays(saleVoucher.Supplier.CreditDays ?? 0),
                ParcelStatus=saleVoucher.Status,
                AdditionalCharges=saleVoucher.AdditionalCharges

            };


            // 5 SupplierResponse
            var supplierResponse = new SupplierResponse
            {
                Id = saleVoucher.Supplier.Id,
                Name = saleVoucher.Supplier.Name,
                TallyLedgerName = saleVoucher.Supplier.TallyLedgerName,
                GstIn = saleVoucher.Supplier.GstIn,
                SubDepartment = saleVoucher.Supplier.SubDepartment.Name,
                Department = saleVoucher.Supplier.SubDepartment.Department.Name,
                Code = saleVoucher.Supplier.Code,
                Address = saleVoucher.Supplier.Address,
                City = saleVoucher.Supplier.City.Name,
                State = saleVoucher.Supplier.City.State.Name,
                Pincode = saleVoucher.Supplier.Pincode,
                StateCode = saleVoucher.Supplier.City.State.Code,
                Mobile = saleVoucher.Supplier.Mobile,
                PAN = saleVoucher.Supplier.PAN,
                Alias = saleVoucher.Supplier.Alias,
                RegType = saleVoucher.Supplier.RegType,
                Email = saleVoucher.Supplier.Email,
                ContactPerson = saleVoucher.Supplier.ContactPerson,
                BankName = saleVoucher.Supplier.BankName,
                AccountNumber = saleVoucher.Supplier.AccountNumber,
                IFSC = saleVoucher.Supplier.IFSC,
                Branch = saleVoucher.Supplier.Branch,
                UPID = saleVoucher.Supplier.UPID,
                CreditDays = saleVoucher.Supplier.CreditDays,
                CreditLimit = saleVoucher.Supplier.CreditLimit,
                GstRegistrationDate = saleVoucher.Supplier.GstRegistrationDate,
                MSMENumber = saleVoucher.Supplier.MSMENumber,
                ECCNumber = saleVoucher.Supplier.ECCNumber,
                Remarks = saleVoucher.Supplier.Remarks,
                DiscountType = saleVoucher.Supplier.DiscountType,
                Discount = saleVoucher.Discount,
                PaymentDiscount=saleVoucher.Supplier.PaymentDiscount,
                AnnualIncentive=saleVoucher.Supplier.AnnualIncentive,
                AgentObj = new AgentTableResponse
                {
                    Id = saleVoucher.Supplier.Agent.Id,
                    Name = saleVoucher.Supplier.Agent.Name,
                    Address = saleVoucher.Supplier.Agent.Address,
                    ContactPersonName = saleVoucher.Supplier.Agent.ContactPersonName,
                    ContactPersonMobile = saleVoucher.Supplier.Agent.ContactPersonMobile,
                    GSTIN = saleVoucher.Supplier.Agent.GSTIN,
                    PAN = saleVoucher.Supplier.Agent.PAN,
                    Pincode = saleVoucher.Supplier.Agent.Pincode,
                    TallyLedgerName = saleVoucher.Supplier.Agent.TallyLedgerName,
                    Area = saleVoucher.Supplier.Agent.Area
                },
            };

            // 6 Stock Groups

            var stockGroupResponse = saleVoucher.Supplier.SupplierProducts
            .Where(sp => sp.StockGroup != null)
            .GroupBy(sp => sp.StockGroup.Id)
            .Select(g =>
             {
                 var first = g.First();
                 // ✅ Get ALL GST Rules (if enabled)                 
                 var gstRules = first.StockGroup.IsGstRule
                     ? first.StockGroup.GstRules
                    .OrderBy(r => r.ApplyOrder) // ✅ सुनिश्चित correct order
                    .Select(r => new GstRuleDto
                    {
                        GstValue =r.GstValue,
                        StartRange = r.ApplyOrder == 0 ? r.StartRange : 0,
                        EndRange = r.ApplyOrder == 0 ? r.EndRange : 0
                    }).ToList()
                    : new List<GstRuleDto>();
                 var hsnCode = first.StockGroup.SupplierHsnCodes?
                     .Where(x => x.SupplierId == saleVoucher.Supplier.Id && x.IsActive)
                     .Select(x => x.HsnCode.Name)
                     .FirstOrDefault();
                 return new StockGroupResponse
                 {
                     Id = first.StockGroup.Id,
                     Name = first.Supplier.Name,
                     TallyLedgerName = first.Supplier.TallyLedgerName,
                     GstValue = first.StockGroup.GstValue,
                     Description = first.StockGroup.Description,
                     IsGstRule = first.StockGroup.IsGstRule,
                     GstRules = gstRules,
                     HsnCode = hsnCode,
                     SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1) + saleVoucher.Supplier.Code,
                     DepartmentObj = new DepartmentResponse
                     {
                         Id = saleVoucher.Supplier.SubDepartment.Department.Id,
                         Name = saleVoucher.Supplier.SubDepartment.Department.Name,
                         Description = saleVoucher.Supplier.SubDepartment.Department.Description,
                         IsActive = saleVoucher.Supplier.SubDepartment.Department.IsActive                        
                     },

                 };
             })
              .ToList();

            // 7 Stock Category

            var stockCategoryResponse = saleVoucher.Supplier.SupplierProducts
              .Where(sp => sp != null)
              .GroupBy(sp => sp.StockGroup.Id) // ✅ avoid duplicates
              .Select(g => new StockCategoryResponse
              {
                  Id = g.First().StockGroup.Id,
                  Name = g.First().StockGroup.Name,
                  TallyLedgerName = g.First().StockGroup.TallyLedgerName,
                  GstValue = g.First().StockGroup.GstValue,
                  Description = g.First().StockGroup.Description,
                  SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1) + saleVoucher.Supplier.Code,

              })
              .ToList();

            // 8 Stock items

            var stockItemResponse = saleVoucherDetails.Select(d =>
            {
                var lineAmount = d.PurchaseRate * d.Quantity;
                var discountAmount = d.SupplierDiscount == 0 ? 0 : (lineAmount * d.SupplierDiscount) / 100;
                var taxableamt = lineAmount - discountAmount;
                var stockGroup = d.Product?.StockGroup;
                decimal cgst = 0;
                decimal sgst = 0;
                decimal igst = 0;
                decimal payableamt = 0;
                decimal gstPercent = 0;
                decimal startRange = 0;
                decimal endRange = 0;
                bool isGstRule = false;
                if (stockGroup != null)
                    isGstRule = stockGroup.IsGstRule;

                var gstRules = new List<GstRuleDto>(); 
                if (stockGroup != null && stockGroup.IsGstRule && stockGroup.GstRules != null)
                {
                    gstRules = stockGroup.GstRules
                      .OrderBy(r => r.ApplyOrder) // ✅ use ApplyOrder
                        .Select(r => new GstRuleDto
                        {
                            GstValue =r.GstValue,
                            StartRange = r.ApplyOrder == 0 ? r.StartRange : 0,
                            EndRange = r.ApplyOrder == 0 ? r.EndRange : 0
                        })                       
                        .ToList();
                }
                var priceHistories = d.Product?.SupplierProductPriceHistories?
                 .Where(x => x.IsDeleted == false)
               .OrderBy(x => x.Date)
              .Select(x => new SupplierProductRateHistoryDTO
              {              
                  Date = x.Date,
                  SupplierProductId = x.SupplierProductId,
                  PurchaseRate = x.PurchaseRate,
                  WholesaleRate=x.WholesaleRate,
                  RetailRate=x.RetailRate
              })
              .ToList() ?? new List<SupplierProductRateHistoryDTO>();

                if (saleVoucher.Supplier.City.State.Code == "09")
                {
                    // Intra-state  // discount is gst column
                    cgst = Math.Round(taxableamt * (d.Discount / 2) / 100, 2);
                    sgst = Math.Round(taxableamt * (d.Discount / 2) / 100, 2);
                }
                else
                {
                    // Inter-state  discount is gst column
                    igst = Math.Round(taxableamt * d.Discount / 100, 2);
                }
                payableamt = taxableamt + cgst + sgst + igst;
                return new StockitemResponse
                {
                    Id = d.ProductId,
                    ProductName = d.Product.PrintName,
                    TallyLedgerName = d.Product.TallyLedgerName,
                    HsnCode = d.Product.HsnCode,
                    StockGroupName = d.Product.StockGroup != null ? d.Product.StockGroup.Name : null,
                    Barcode= d.Product.Barcode,
                    Quantity = d.Quantity,
                    PurchasePrice = d.PurchaseRate,
                    Discount = d.SupplierDiscount,
                    Gst = d.Discount, // discount is gst column
                    Total = lineAmount,
                    DiscountAmount = lineAmount - discountAmount,
                    CGST = cgst,
                    SGST = sgst,
                    IGST = igst,
                    PayableAmount = payableamt,
                    WholeSaleRate = d.WholeSaleRate,
                    MrpRate = d.MrpRate,
                    IsGstRule = isGstRule,
                    GstRules = gstRules,
                    GstApplicable=d.Product.GstApplicable,
                    GSTNature = d.Product.GSTNature,
                    GSTTaxability = d.Product.GSTTaxability,
                    SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1) + saleVoucher.Supplier.Code,
                    // All price history records
                    PriceHistories = priceHistories

                };
            }).ToList();

            // 9 Final Response
            return new TallyTransactionPurchaseResponse
            {
                SaleVoucherPrint = saleVoucherPrint,
                SupplierResponse = supplierResponse,
                StockCategoryResponse = stockCategoryResponse,
                StockGroupResponse = stockGroupResponse,
                StockitemResponse = stockItemResponse
            };
        }
    }

}