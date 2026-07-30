using AutoMapper;
using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.SalePersons;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services.Sales;

namespace Textile.Core.Managers.Services.Sales
{
    public class SalesPersonService : ISalesPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;



        public SalesPersonService(IUnitOfWork unitOfWork,
            TextileDbContext context, IMediator mediator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, string currentUserName)
        {
            var repo = _unitOfWork.Repository<SalePerson, Guid>();

            var entity = await repo.GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return false;

            entity.IsDeleted = true;
            entity.ModifiedBy = currentUserId;
            entity.ModifiedByUserName = currentUserName;
            entity.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SalePersonResponse>> GetActiveSalesPerson()
        {
            var data = await _context.SalesPersons.Include(x=>x.City.State)
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<SalePersonResponse>>(data);
        }

        public async Task<SalePersonResponse?> GetByIdAsync(Guid id)
        {
            var repo = _unitOfWork.Repository<SalePerson, Guid>();
            var salePerson = await repo.GetByIdAsync(id,x=>x.City.State);
           return  _mapper.Map<SalePerson, SalePersonResponse>(salePerson);
        }

        public async Task<TableResult<SalePersonResponse>> GetTableData(TableDataRequest req)
        {
            var query = _context.SalesPersons
                .Include(x => x.City.State)
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            // SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.PhoneNumber!.ToLower().Contains(s) ||
                    x.Email!.ToLower().Contains(s));
            }

            int total = await query.CountAsync();

            // Sorting
            query = ApplySorting(query, req.SortField, req.SortOrder);

            var data = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            var salesPersons = _mapper.Map<List<SalePersonResponse>>(data);

            return new TableResult<SalePersonResponse>
            {
                TotalRows = total,
                Result = salesPersons
            };
        }
        public async Task<bool> SaveAsync(SalePersonRequest request, Guid currentUserId, string currentUserName)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var repo = _unitOfWork.Repository<SalePerson, Guid>();
            SalePerson entity;

            if (request.Id != null)
            {
                // UPDATE
                entity = await   repo.GetByIdAsync(request.Id.Value);

                if (entity == null)
                    throw new Exception("SalesPerson not found");

                // map updated fields
                _mapper.Map(request, entity);

                entity.ModifiedBy = currentUserId;
                entity.ModifiedByUserName = currentUserName;
                entity.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                // CREATE
                entity = _mapper.Map<SalePerson>(request);

                entity.CreatedBy = currentUserId;
                entity.CreatedByUserName = currentUserName;
                entity.CreatedOn = DateTime.UtcNow;
                entity.IsDeleted = false;

                await _context.SalesPersons.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        private IQueryable<SalePerson> ApplySorting(
      IQueryable<SalePerson> query,
      string? sortField,
      int sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortField))
                return query.OrderByDescending(x => x.Name);

            return (sortField.ToLower(), sortOrder) switch
            {
                ("name", 1) => query.OrderBy(x => x.Name),
                ("name", -1) => query.OrderByDescending(x => x.Name),

                ("mobile", 1) => query.OrderBy(x => x.PhoneNumber),
                ("mobile", -1) => query.OrderByDescending(x => x.PhoneNumber),

                ("email", 1) => query.OrderBy(x => x.Email),
                ("email", -1) => query.OrderByDescending(x => x.Email),

                _ => query.OrderByDescending(x => x.Name)
            };
        }
    }
}
