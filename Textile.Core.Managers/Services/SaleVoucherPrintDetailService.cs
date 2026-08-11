using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.SaleVoucherPrintDetails;
using Textile.Core.Entities.Models.Response.SaleVoucherPrintDetails;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SaleVoucherPrintDetailService : ISaleVoucherPrintDetailService
    {
        private readonly TextileDbContext _context;

        public SaleVoucherPrintDetailService(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SaleVoucherPrintDetailResponse> GetAsync()
        {
            var detail = await _context.SaleVoucherPrintDetails
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            return detail == null ? new SaleVoucherPrintDetailResponse() : Map(detail);
        }

        public async Task<bool> SaveAsync(SaleVoucherPrintDetailRequest request, Guid currentUserId)
        {
            await EnsureDeveloperAdminAsync(currentUserId);

            var detail = request.Id > 0
                ? await _context.SaleVoucherPrintDetails.FirstOrDefaultAsync(x => x.Id == request.Id)
                : await _context.SaleVoucherPrintDetails.OrderBy(x => x.Id).FirstOrDefaultAsync();

            if (detail == null)
            {
                detail = new SaleVoucherPrintDetail();
                _context.SaleVoucherPrintDetails.Add(detail);
            }

            detail.CompanyName = NormalizeRequired(request.CompanyName);
            detail.Address1 = NormalizeRequired(request.Address1);
            detail.Address2 = NormalizeOptional(request.Address2);
            detail.Description = NormalizeOptional(request.Description);
            detail.GstIn = NormalizeOptional(request.GstIn);

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task EnsureDeveloperAdminAsync(Guid currentUserId)
        {
            var currentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == currentUserId);

            if (currentUser == null ||
                currentUser.RoleId != (int)RoleEnum.SuperAdmin ||
                !currentUser.IsDeveloper)
            {
                throw new UnauthorizedAccessException("Only developer admin can update sale voucher print details.");
            }
        }

        private static SaleVoucherPrintDetailResponse Map(SaleVoucherPrintDetail detail)
        {
            return new SaleVoucherPrintDetailResponse
            {
                Id = detail.Id,
                CompanyName = detail.CompanyName,
                Address1 = detail.Address1,
                Address2 = detail.Address2,
                Description = detail.Description,
                GstIn = detail.GstIn
            };
        }

        private static string NormalizeRequired(string? value)
        {
            return value?.Trim().Trim(',') ?? string.Empty;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim().Trim(',');
        }
    }
}
