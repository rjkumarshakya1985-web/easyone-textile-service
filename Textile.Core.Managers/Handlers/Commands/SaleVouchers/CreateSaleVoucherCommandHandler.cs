using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Infrastructure.Helpers;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.SaleVouchers;
using Textile.Core.Managers.Common.Exceptions;



namespace Textile.Core.Managers.Handlers.Commands.SaleVouchers
{
    // SalveVoucherSatus Service injection is added to insert the status of the sale voucher when it is created.
    // gives error
    public class CreateSaleVoucherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        TextileDbContext context, ISaleVoucherStatusService saleVoucherService) : IRequestHandler<CreateSaleVoucherCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly TextileDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ISaleVoucherStatusService _saleVoucherService = saleVoucherService ?? throw new ArgumentNullException(nameof(saleVoucherService));

        public async Task<int> Handle(CreateSaleVoucherCommand command, CancellationToken cancellationToken)
        {
            var request = command.SaleVoucherRequest;

            var saleVoucherRepo = _unitOfWork.Repository<SaleVoucher, int>();
            var saleVoucherDetailRepo = _unitOfWork.Repository<SaleVoucherDetail, Guid>();
            var supplierRepo = _unitOfWork.Repository<Supplier, Guid>();
            var transportRepo = _unitOfWork.Repository<Transport, int>();
           
            await _unitOfWork.BeginTransactionAsync(); // ✅ Start transaction

            try
            {
                // ---------------------------
                // Validate Supplier
                // ---------------------------
                var supplier = await supplierRepo.GetSingleAsync(x => x.Id == request.SupplierId)
                               ?? throw new Exception("Supplier not found");

                // ---------------------------
                // Validate Transport
                // ---------------------------
                var transport = await transportRepo.GetSingleAsync(x => x.Id == request.TransportId)
                                 ?? throw new Exception("Transport not found");

                // 1. User selected date (only date part)
                var selectedDate = request.Date.Date;

                // 2. Current system time
                var now = DateTime.Now;

                // 3. Combine date + current time
                var finalDateTime = new DateTime(
                    selectedDate.Year,
                    selectedDate.Month,
                    selectedDate.Day,
                    now.Hour,
                    now.Minute,
                    now.Second
                );


                // ---------------------------
                // Map SaleVoucher
                // ---------------------------
                var saleVoucher = _mapper.Map<SaleVoucher>(request);
                saleVoucher.CreatedBy = command.CurrentUserId;
                saleVoucher.Date = finalDateTime;
                saleVoucher.CreatedByUserName = command.CurrentUserName;
                saleVoucher.Discount = supplier.BillDiscount ?? 0;
                saleVoucher.CreatedOn = DateTime.UtcNow;
                saleVoucher.IsDeleted = false;
                saleVoucher.Remarks ??= null;
                saleVoucher.SaleVoucherDetails.Clear();

                await saleVoucherRepo.AddAsync(saleVoucher);

                // ---------------------------
                // Map SaleVoucherDetails
                // ---------------------------
                var details = request.SaleVoucherDetails?.ToList();
                if (details != null && details.Count > 0)
                {
                    // Fetch all products in a single query
                    var productIds = details.Select(d => d.ProductId).ToList();
                    var products = await _context.SupplierProductViews.Where(x => productIds.Contains(x.Id)).ToListAsync();
                    var saleVoucherDetails = details.Select(detail =>
                    {
                        var product = products.FirstOrDefault(p => p.Id == detail.ProductId)
                                      ?? throw new Exception($"Product {detail.ProductId} not found");

                        return new SaleVoucherDetail
                        {
                            SaleVoucherId = saleVoucher.Id,
                            ProductId = product.Id,
                            PurchaseRate = product.PurchaseRate,
                            Quantity = detail.Quantity,
                            Discount = product.Discount,  /// Gst column
                            WholeSalesMargin = supplier.WholeSalesMargin,
                            RetailMargin = supplier.RetailMargin,
                            MrpMargin = supplier.MrpMargin,
                            ManualWholeSaleRate = product.ManualWholeSaleRate,
                            WholeSaleRate = product.WholeSaleRate,
                            IsSupplierDiscount = detail.IsSupplierDiscount,
                            SupplierDiscount = detail.IsSupplierDiscount ? supplier.BillDiscount ?? 0 : 0,
                        };
                    }).ToList();


                    await saleVoucherDetailRepo.AddAsync(saleVoucherDetails);


                    var saleVoucherStatus = new SaleVoucherStatusView
                    {
                        SaleVoucherId = saleVoucher.Id,
                        Status = (int)saleVoucher.Status,
                        CreatedBy = command.CurrentUserId,
                        CreatedByUserName = command.CurrentUserName,
                        CreatedOn = DateTime.UtcNow,
                        Date = DateTime.UtcNow
                    };

                       
                     await _saleVoucherService.AddAsync(saleVoucherStatus);
                }

                await _unitOfWork.CommitTranscationAsync(); 
                return saleVoucher.Id;
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsDuplicateKey(ex))
            {
                await _unitOfWork.RollbackTranscationAsync();

                throw new DuplicateEntityException(
                    $"Supplier bill number '{request.SupplierBillNumber}' already exists.");
            }
            catch
            {
                await _unitOfWork.RollbackTranscationAsync(); // ✅ Rollback if any error
                throw;
            }
        }
    }

}
