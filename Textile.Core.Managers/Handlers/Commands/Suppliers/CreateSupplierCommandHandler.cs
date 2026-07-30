using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Requests.Users;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Suppliers;
using Textile.Core.Managers.Commands.Users;
using Textile.Core.Managers.Query.Suppliers;

namespace Textile.Core.Managers.Handlers.Commands.Suppliers
{


    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator; // To call UserHandler
        private readonly ISupplierStockGroupService _supplierStockGroupService;
        private readonly ISupplierHsnCodeService _supplierHsnCodeService;

        public CreateSupplierCommandHandler(IUnitOfWork unitOfWork, IMediator mediator,
            ISupplierStockGroupService supplierStockGroupService,
            ISupplierHsnCodeService supplierHsnCodeService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _supplierStockGroupService = supplierStockGroupService;
            _supplierHsnCodeService = supplierHsnCodeService;
        }

        public async Task<Guid> Handle(CreateSupplierCommand command, CancellationToken cancellationToken)
        {
            var request = command.SupplierRequest;
            var supplierRepo = _unitOfWork.Repository<Supplier, Guid>();
            var userRepo = _unitOfWork.Repository<User, Guid>();
            Guid? userId = null;
            string supplierCode = string.Empty;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                bool isNewSupplier = request.Id == null;

                if (isNewSupplier)
                {
                    // 0️⃣ VALIDATION
                    supplierCode = await _mediator.Send(new GetNewSupplierCodeQuery());

                    if (await userRepo.GetSingleAsync(u => u.UserName == request.UserName) != null)
                        throw new Exception("UserName already exists.");

                    if (await supplierRepo.GetSingleAsync(u => u.GstIn == request.GstIn) != null)
                        throw new Exception("A supplier with the same GSTIN already exists.");

                    // 1️⃣ CREATE USER
                    var userRequest = new UserRequest
                    {
                        UserName = request.UserName,
                        Password = supplierCode,
                        Email = request.Email,
                        Phone = string.Empty,
                        IsActive = true,
                        RoleId = 2,
                        CreatedBy = command.CreatedBy,
                        CreatedByUserName = command.CreatedByUserName
                    };

                    userId = await _mediator.Send(new CreateUserCommand(userRequest), cancellationToken);

                    // 2️⃣ CREATE SUPPLIER
                    var newSupplier = MapSupplierFields(request, userId.Value,  supplierCode, command.CreatedBy, command.CreatedByUserName);
                    await supplierRepo.AddAsync(newSupplier);


                    // 3 TRANSPORT MAPPING
                    if (request.TransportIds !=null && request.TransportIds.Any())
                    {
                        foreach (var transportId in request.TransportIds)
                        {
                            await _mediator.Send(new AddSupplierTransportCommand(
                                new AddSupplierTransportRequest
                                {
                                    SupplierId = newSupplier.Id,
                                    TransportId = transportId
                                }), cancellationToken);
                        }
                    }

                   
                    //  STOCK GROUP MAPPING
                    if (request.StockGroupId.HasValue)
                    {
                       var isStockGroup =   await _supplierStockGroupService.AssignSupplierStockGroup(
                                new AddSupplierStockGroupRequest
                                {
                                    SupplierId = newSupplier.Id,
                                    StockGroupId = request.StockGroupId.Value
                                });

                        if (request.HsnCodeId.HasValue && isStockGroup)
                        {
                            await _supplierHsnCodeService.AssignSupplierHsnCode(
                                   new SupplierHsnCodeRequest
                                   {
                                       SupplierId = newSupplier.Id,
                                       HsnCodeId = request.HsnCodeId.Value,
                                       StockGroupId = request.StockGroupId.Value
                                   });
                        }
                    }

                    await _unitOfWork.CommitTranscationAsync();
                    return newSupplier.Id;

                }
                else
                {
                    // 3️⃣ UPDATE SUPPLIER
                    var existingSupplier = await supplierRepo.GetSingleAsync(s => s.Id == request.Id);
                    if (existingSupplier == null)
                        throw new Exception("Supplier not found.");

                    UpdateSupplierFields(existingSupplier, request, command.CreatedBy, command.CreatedByUserName);
                    await supplierRepo.UpdateAsync(existingSupplier);

                    if (request.TransportIds != null)
                    {
                        var mappingRepo = _unitOfWork.Repository<SupplierTransport, Guid>();
                        var mappings = (await mappingRepo.GetAllAsync(x => x.SupplierId == existingSupplier.Id)).ToList();
                        var requestedIds = request.TransportIds.Distinct().ToHashSet();

                        var removedMappings = mappings.Where(x => !requestedIds.Contains(x.TransportId)).ToList();
                        if (removedMappings.Count > 0)
                            await mappingRepo.DeleteAllAsync(removedMappings);

                        foreach (var transportId in requestedIds.Where(id => mappings.All(x => x.TransportId != id)))
                        {
                            await _mediator.Send(new AddSupplierTransportCommand(new AddSupplierTransportRequest
                            {
                                SupplierId = existingSupplier.Id,
                                TransportId = transportId
                            }), cancellationToken);
                        }
                    }
                    await _unitOfWork.CommitTranscationAsync();
                    return existingSupplier.Id;
                }
            }
            catch
            {
                await _unitOfWork.RollbackTranscationAsync();
                throw;
            }
        }

        private Supplier MapSupplierFields(SupplierRequest request, Guid userId, string code, Guid createdBy, string createdByUserName)
        {
            return new Supplier
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Code = code,
                SubDepartmentId = request.SubDepartmentId,
                AgentId = request.AgentId,
                Name = request.Name,
                Alias = request.Alias,
                GstIn = request.GstIn,
                PAN = request.PAN,
                RegType = request.RegType,
                Address = request.Address,
                CityId = request.CityId,
                Mobile = request.Mobile,
                Email = request.Email,
                ContactPerson = request.ContactPerson,
                BankName = request.BankName,
                Branch = request.Branch,
                AccountNumber = request.AccountNumber,
                IFSC = request.IFSC,
                UPID = request.UPID,
                CreditDays = request.CreditDays,
                CreditLimit = request.CreditLimit,
                GstRegistrationDate = request.GstRegistrationDate,
                MSMENumber = request.MSMENumber,
                ECCNumber = request.ECCNumber,
                Remarks = request.Remarks,
                DiscountType = request.DiscountType,
                TransactionType = request.TransactionType,
                WholeSalesMargin = request.WholeSalesMargin,
                RetailMargin = request.RetailMargin,
                MrpMargin = request.MrpMargin,
                BillDiscount = request.BillDiscount,
                PaymentDiscount = request.PaymentDiscount,
                AnnualIncentive = request.AnnualIncentive,
                Pincode = request.PinCode,
                CreatedBy = createdBy,
                CreatedByUserName = createdByUserName,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
            };


        }

        private void UpdateSupplierFields(Supplier supplier, SupplierRequest request, Guid modifiedBy, string modifiedByUserName)
        {
            supplier.SubDepartmentId = request.SubDepartmentId;
            supplier.AgentId = request.AgentId;
            supplier.Name = request.Name;
            supplier.Alias = request.Alias;
            supplier.GstIn = request.GstIn;
            supplier.PAN = request.PAN;
            supplier.RegType = request.RegType;
            supplier.Address = request.Address;
            supplier.CityId = request.CityId;
            supplier.Mobile = request.Mobile;
            supplier.Email = request.Email;
            supplier.ContactPerson = request.ContactPerson;
            supplier.BankName = request.BankName;
            supplier.Branch = request.Branch;
            supplier.AccountNumber = request.AccountNumber;
            supplier.IFSC = request.IFSC;
            supplier.UPID = request.UPID;
            supplier.CreditDays = request.CreditDays;
            supplier.CreditLimit = request.CreditLimit;
            supplier.GstRegistrationDate = request.GstRegistrationDate;
            supplier.MSMENumber = request.MSMENumber;
            supplier.ECCNumber = request.ECCNumber;
            supplier.Remarks = request.Remarks;
            supplier.DiscountType = request.DiscountType;
            supplier.TransactionType = request.TransactionType;
            supplier.WholeSalesMargin = request.WholeSalesMargin;
            supplier.RetailMargin = request.RetailMargin;
            supplier.MrpMargin = request.MrpMargin;
            supplier.BillDiscount = request.BillDiscount;
            supplier.PaymentDiscount = request.PaymentDiscount;
            supplier.AnnualIncentive = request.AnnualIncentive;
            supplier.Pincode = request.PinCode;
            supplier.ModifiedBy = modifiedBy;
            supplier.ModifiedByUserName = modifiedByUserName;
            supplier.ModifiedOn = DateTime.UtcNow;
        }




    }

}
