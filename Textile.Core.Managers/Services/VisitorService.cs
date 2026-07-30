using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;


namespace Textile.Core.Managers.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly IMapper _mapper;

        public VisitorService(IUnitOfWork unitOfWork, TextileDbContext context,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // -------------------------
        // PAGINATION / TABLE
        // -------------------------
        public async Task<TableResult<VisitorResponse>> GetTableData(TableDataRequest req)
        {
            var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            var start = indiaTime.Date;
            var end = start.AddDays(1);

            var query = _context.Visitors
                .Include(x => x.City.State)
                .AsNoTracking()
                .Where(x => x.VisitDate >= start && x.VisitDate < end);

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.Mobile!.ToLower().Contains(s));
            }

            int total = await query.CountAsync();

            // Apply Sorting
            query = ApplySorting(query, req.SortField, req.SortOrder);
            var data = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            var visitors = _mapper.Map<List<VisitorResponse>>(data);

            return new TableResult<VisitorResponse>
            {
                TotalRows = total,
                Result = visitors
            };
        }

        public async Task<VisitorResponse> GetVisitoryById(int id)
        {
            var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(
               DateTime.UtcNow,
               TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            var start = indiaTime.Date;
            var end = start.AddDays(1);

            var visitor = await _context.Visitors.Include(x=>x.Customer)
                .Include(x => x.City)
                    .ThenInclude(c => c.State)
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new VisitorResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Mobile = x.Mobile,
                    CustomerType = x.CustomerType,
                    VisitDate = x.VisitDate,
                    Remarks = x.Remarks,
                    CityId = x.CityId,
                    StateId = x.City.StateId, 
                    CreatedBy = x.CreatedBy,
                    CreatedByUserName = x.CreatedByUserName,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedByUserName = x.ModifiedByUserName,
                    ModifiedOn = x.ModifiedOn,
                    CustomerResponse = new Entities.Models.Response.Customers.CustomerResponse()
                    {
                        Id = x.CustomerId ?? Guid.Empty,
                        Name = x.Customer != null ? x.Customer.Name : string.Empty,
                        Mobile = x.Customer != null ? x.Customer.Mobile : string.Empty,
                        CustomerType = x.Customer != null ? x.Customer.CustomerType : 0,
                        Discount = x.Customer != null ? x.Customer.Discount : null,
                    }
                })
                .FirstOrDefaultAsync();

            return visitor;
        }

        public async Task<VisitorResponse?> GetVisitoryByMobile(string mobile)
        {
            var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            var start = indiaTime.Date;
            var end = start.AddDays(1);

            var visitor = await _context.Visitors.Include(x => x.Customer)
                .Include(x => x.City)
                    .ThenInclude(c => c.State)
                .AsNoTracking()
                .Where(x => x.Mobile == mobile &&
                            x.VisitDate >= start &&
                            x.VisitDate < end)
                .Select(x => new VisitorResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Mobile = x.Mobile,
                    CustomerType = x.CustomerType,
                    VisitDate = x.VisitDate,
                    Remarks = x.Remarks,
                    CityId = x.CityId,
                    CreatedBy = x.CreatedBy,
                    CreatedByUserName = x.CreatedByUserName,
                    CreatedOn = x.CreatedOn,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedByUserName = x.ModifiedByUserName,
                    ModifiedOn = x.ModifiedOn,
                    CustomerResponse = new Entities.Models.Response.Customers.CustomerResponse()
                    {
                        Id = x.CustomerId ?? Guid.Empty,
                        Name = x.Customer != null ? x.Customer.Name : string.Empty,
                        Mobile = x.Customer != null ? x.Customer.Mobile : string.Empty,
                        CustomerType = x.Customer != null ? x.Customer.CustomerType : 0,
                        Discount = x.Customer != null ? x.Customer.Discount : null,
                    }
                })
                .FirstOrDefaultAsync();

            return visitor;
        }

        private IQueryable<Visitor> ApplySorting(IQueryable<Visitor> query,string? sortField,int sortOrder)
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
                
                ("customertype", 1) => query.OrderBy(x => x.CustomerType),
                ("customertype", -1) => query.OrderByDescending(x => x.CustomerType),

                ("mobile", 1) => query.OrderBy(x => x.Mobile),
                ("mobile", -1) => query.OrderByDescending(x => x.Mobile),             


                _ => query.OrderByDescending(x => x.Name)
            };
        }
    }
}
