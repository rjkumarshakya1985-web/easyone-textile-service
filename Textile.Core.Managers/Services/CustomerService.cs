using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Entities.Models.Requests.Customers;
using Textile.Core.Entities.Models.Response.Billing;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Infrastructure.Helpers;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Common.Exceptions;

namespace Textile.Core.Managers.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, TextileDbContext context,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // -------------------------
        // CREATE
        // -------------------------
        public async Task<Guid> CreateAsync(CustomerRequest request, Guid currentUserId, string currentUserName)
        {
            await ValidateBusinessRulesAsync(request);
            try
            {
                var customerName = request.Name.Trim();
                var gstIn = request.GstIn?.Trim();

                var nameExists = await _context.Customers
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == customerName.ToLower());

                if (nameExists)
                    throw new DuplicateEntityException("A customer with this name already exists.");

                if (!string.IsNullOrWhiteSpace(gstIn))
                {
                    var gstInExists = await _context.Customers
                        .AsNoTracking()
                        .AnyAsync(x => !x.IsDeleted && x.GstIn == gstIn);

                    if (gstInExists)
                        throw new DuplicateEntityException("A customer with this GSTIN already exists.");
                }

                var repository = _unitOfWork.Repository<Customer, Guid>();

                var entity = _mapper.Map<Customer>(request);
                entity.CreatedBy = currentUserId;
                entity.CreatedByUserName = currentUserName;
                entity.CreatedOn = DateTime.UtcNow;
                entity.IsActive = true;

                await repository.AddAsync(entity);
                return entity.Id;
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsDuplicateKey(ex))
            {
                throw new DuplicateEntityException(
                    "Customer already exists.");
            }
        }

        // -------------------------
        // UPDATE
        // -------------------------
        public async Task<bool> UpdateAsync(CustomerRequest request, Guid currentUserId, string currentUserName)
        {
            await ValidateBusinessRulesAsync(request);
            var repository = _unitOfWork.Repository<Customer, Guid>();

            var entity = await repository.GetByIdAsync(request.Id.Value);
            if (entity == null)
                throw new Exception("Customer not found");

            entity.Name = request.Name;
            entity.Alias = request.Alias;
            entity.LedgerName = request.LedgerName;
            entity.PrintName = request.PrintName;
            entity.GroupName = request.GroupName;
            entity.GstIn = request.GstIn;
            entity.Pan = request.Pan;
            entity.RegType = request.RegType;
            entity.Discount = request.Discount;
            entity.Mu = request.Mu;
            entity.PaymentTerm = request.PaymentTerm;
            entity.CustomerCategory = request.CustomerCategory;
            entity.CustomerStatus = request.CustomerStatus;
            entity.RateType = request.RateType;
            entity.AlternateNo = request.AlternateNo;
            entity.CreditAlertLimit = request.CreditAlertLimit;
            entity.Incentive = request.Incentive;
            entity.Term = request.Term;
            entity.Reference = request.Reference;
            entity.CustomerCode = request.CustomerCode;
            entity.TransportId = request.TransportId;
            entity.CustomerAgentId = request.CustomerAgentId;
            entity.BillingAddress = request.BillingAddress;
            entity.ShippingAddress = request.ShippingAddress;
            entity.CityId = request.CityId;
            entity.PinCode = request.PinCode;
            entity.Phone = request.Phone;
            entity.Mobile = request.Mobile;
            entity.Email = request.Email;
            entity.ContactPerson = request.ContactPerson;
            entity.OpeningBalance = request.OpeningBalance;
            entity.CreditDays = request.CreditDays;
            entity.CreditLimit = request.CreditLimit;
            entity.PriceLevel = request.PriceLevel;
            entity.TallyLedgerType = request.TallyLedgerType;
            entity.TallyCategory = request.TallyCategory;
            entity.CustomerType = request.CustomerType;
            entity.Remarks = request.Remarks;

            entity.ModifiedBy = currentUserId;
            entity.ModifiedOn = DateTime.UtcNow;

            await repository.UpdateAsync(entity);
            return true;
        }

        // -------------------------
        // DELETE (Soft Delete)
        // -------------------------
        public async Task<bool> DeleteAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<Customer, Guid>();

            var entity = await repository.GetByIdAsync(id,x=>x.City.State);
            if (entity == null)
                throw new Exception("Customer not found");

            entity.IsDeleted = true;
            await repository.UpdateAsync(entity);
            return true;
        }

        private async Task ValidateBusinessRulesAsync(CustomerRequest request)
        {
            if (request.CustomerStatus is null or < 1 or > 3)
                throw new ArgumentException("Customer Status must be Dormant, NPA, or Potential.");

            if (request.RateType is null or < 1 or > 2)
                throw new ArgumentException("Rate Type must be Nett or Dhara.");

            if (request.RateType == 2 && !request.Mu.HasValue)
                throw new ArgumentException("MU is required when Rate Type is Dhara.");

            if (request.RateType != 2)
                request.Mu = null;

            if (request.CreditAlertLimit.HasValue && request.CreditLimit.HasValue &&
                request.CreditAlertLimit.Value >= request.CreditLimit.Value)
                throw new ArgumentException("Credit Alert Limit must be less than Credit Limit.");

            if (string.IsNullOrWhiteSpace(request.CustomerCode))
                throw new ArgumentException("Customer Code is required.");

            request.CustomerCode = request.CustomerCode.Trim();

            if (!request.TransportId.HasValue)
                throw new ArgumentException("Sales Transport is required.");

            var isSalesTransport = await _context.Transports.AsNoTracking().AnyAsync(x =>
                x.Id == request.TransportId.Value && x.IsActive && !x.IsDeleted &&
                (x.TransportType == (int)TransportTypeEnum.Sales ||
                 x.TransportType == (int)TransportTypeEnum.Both));
            if (!isSalesTransport)
                throw new ArgumentException("Selected transport must be Sales or Both type.");

            if (!request.CustomerAgentId.HasValue)
                throw new ArgumentException("Customer Agent is required.");

            var isValidCustomerAgent = await _context.CustomerAgents.AsNoTracking().AnyAsync(x =>
                x.Id == request.CustomerAgentId.Value && x.IsActive && !x.IsDeleted);
            if (!isValidCustomerAgent)
                throw new ArgumentException("Selected Customer Agent is invalid or inactive.");
        }

        // -------------------------
        // GET BY ID
        // -------------------------
        public async Task<CustomerResponse?> GetByIdAsync(Guid id)
        {
            var customer = await _context.Customers.Include(x=>x.City.State).Include(x => x.CustomerAgent)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            var entity = _mapper.Map<CustomerResponse>(customer);    
            entity.StateId = customer?.City?.StateId;
            entity.CityId = customer?.City?.Id;
            entity.CustomerAgentObj = customer?.CustomerAgent == null ? null : new AgentTableResponse
            {
                Id = customer.CustomerAgent.Id,
                Name = customer.CustomerAgent.Name,
                ContactPersonName = customer.CustomerAgent.ContactPersonName,
                ContactPersonMobile = customer.CustomerAgent.ContactPersonMobile,
                GSTIN = customer.CustomerAgent.GSTIN,
                PAN = customer.CustomerAgent.PAN,
                IsActive = customer.CustomerAgent.IsActive
            };
            return entity;
        }

        // -------------------------
        // PAGINATION / TABLE
        // -------------------------
        public async Task<TableResult<CustomerResponse>> GetTableData(TableDataRequest req)
        {
            var query = _context.Customers.Include(x=>x.City.State)
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.Mobile!.ToLower().Contains(s) ||
                    x.Email!.ToLower().Contains(s));
            }

            int total = await query.CountAsync();

            // Apply Sorting
            query = ApplySorting(query, req.SortField, req.SortOrder);
            var data = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            var customers = _mapper.Map<List<CustomerResponse>>(data);
           
            return new TableResult<CustomerResponse>
            {
                TotalRows = total,
                Result = customers
            };
        }
        private IQueryable<Customer> ApplySorting( 
           IQueryable<Customer> query,
           string? sortField,
           int sortOrder)
         {
            if (string.IsNullOrWhiteSpace(sortField))
            {
                // Default sorting
                return query.OrderByDescending(x => x.Name);
            }

            return (sortField.ToLower(), sortOrder) switch
            {
                ("name", 1) => query.OrderBy(x => x.Name),
                ("name", -1) => query.OrderByDescending(x => x.Name),

                ("gstin", 1) => query.OrderBy(x => x.GstIn),
                ("gstin", -1) => query.OrderByDescending(x => x.GstIn),

                ("customertype", 1) => query.OrderBy(x => x.CustomerType),
                ("customertype", -1) => query.OrderByDescending(x => x.CustomerType),

                ("mobile", 1) => query.OrderBy(x => x.Mobile),
                ("mobile", -1) => query.OrderByDescending(x => x.Mobile),

                ("email", 1) => query.OrderBy(x => x.Email),
                ("email", -1) => query.OrderByDescending(x => x.Email),

                ("pan", 1) => query.OrderBy(x => x.Pan),
                ("pan", -1) => query.OrderByDescending(x => x.Pan),
               
                ("pincode", 1) => query.OrderBy(x => x.PinCode),
                ("pincode", -1) => query.OrderByDescending(x => x.PinCode),

                ("billingaddress", 1) => query.OrderBy(x => x.BillingAddress),
                ("billingaddress", -1) => query.OrderByDescending(x => x.BillingAddress),

                ("shippingaddress", 1) => query.OrderBy(x => x.ShippingAddress),
                ("shippingaddress", -1) => query.OrderByDescending(x => x.ShippingAddress),
                 ("contactperson", 1) => query.OrderBy(x => x.ContactPerson),
                ("contactperson", -1) => query.OrderByDescending(x => x.ContactPerson),
                 ("creditlimit", 1) => query.OrderBy(x => x.CreditLimit),
                ("creditlimit", -1) => query.OrderByDescending(x => x.CreditLimit),
                 ("creditdays", 1) => query.OrderBy(x => x.CreditDays),
                ("creditdays", -1) => query.OrderByDescending(x => x.CreditDays),
                 ("customerType", 1) => query.OrderBy(x => x.CustomerType),
                ("customerType", -1) => query.OrderByDescending(x => x.CustomerType),
           



                _ => query.OrderByDescending(x => x.Name)
            };
        }

        public async Task<List<BillingCustomerResponse>> GetBillingCustomers()
        {
            return await _context.Customers.Where(x=>x.IsActive && !x.IsDeleted).AsNoTracking()
           
           .Select(x => new BillingCustomerResponse
           {
               Id = x.Id,
               Name = x.Name,
               GstIn = x.GstIn,
               Discount = x.Discount,
               Mobile = x.Mobile,
               CustomerType = x.CustomerType
              
           }).ToListAsync();
        }
       
    }
}
