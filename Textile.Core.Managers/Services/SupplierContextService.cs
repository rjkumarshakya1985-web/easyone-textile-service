using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SupplierContextService : ISupplierContextService
    {
        private readonly TextileDbContext _context;
        private readonly IUserContextService _userContext;

        private Guid? _cachedSupplierId;

        public SupplierContextService(
            TextileDbContext context,
            IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<Guid> GetSupplierIdAsync()
        {
            if (_cachedSupplierId.HasValue)
                return _cachedSupplierId.Value;

            if (_userContext.GetUserRole() != RoleEnum.Supplier)
                throw new UnauthorizedAccessException("User is not Supplier");

            var supplierId = await _context.Suppliers
                .Where(s => s.UserId == _userContext.GetUserId())
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (supplierId == Guid.Empty)
                throw new UnauthorizedAccessException("Supplier profile not found");

            _cachedSupplierId = supplierId;
            return supplierId;
        }
    }

}
