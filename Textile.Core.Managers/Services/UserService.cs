
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Users;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;

        public UserService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<User, Guid>();

            var user = await repository.GetByIdAsync(id,x=>x.UserDetail);

            if (user == null)
                return null;

            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive,
                IsDeveloper = user.IsDeveloper,
                Role = (RoleEnum)user.RoleId,
                UserDetail = user.UserDetail == null ? null : new UserDetailResponse
                {
                    Id = user.UserDetail.Id,
                    UserId = user.UserDetail.UserId,
                    DepartmentId = user.UserDetail.DepartmentId

                }
            };
        }


        public async Task<TableResult<UserResponse>> GetTableData(TableDataRequest dataRequest)
        {
            var query = _context.Users.Where(x=>x.RoleId!=(int)RoleEnum.Supplier)
                 .AsNoTracking();


            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(dataRequest.Search))
            {
                var s = dataRequest.Search.Trim().ToLower();

                query = query.Where(x =>
                  x.UserName.ToLower().Contains(s) ||
                  x.Email != null && x.Email.ToString().ToLower().Contains(s) || // Email  should be null
                  x.Phone != null && x.Phone.ToString().ToLower().Contains(s)) // Phone should be null
                  ;
            }

            // 🔢 TOTAL COUNT (before paging)
            int total = await query.CountAsync();

            // 📄 PAGED DATA
            var data = await query
                .OrderByDescending(x => x.Id) // single ordering
                .Skip(dataRequest.PageIndex * dataRequest.PageSize)
                .Take(dataRequest.PageSize)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Password = x.Password,
                    Email = x.Email,
                    Phone = x.Phone,
                    Role = (RoleEnum)x.RoleId,
                    IsActive = x.IsActive,
                    IsDeveloper = x.IsDeveloper
                })
                .ToListAsync();

            return new TableResult<UserResponse>
            {
                TotalRows = total,
                Result = data
            };
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<User, Guid>();

            var user = await repository.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = !user.IsActive;

            await repository.UpdateAsync(user);
            return true;
        }

    }
}
