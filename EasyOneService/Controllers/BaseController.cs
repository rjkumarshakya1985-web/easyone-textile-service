using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    /// <summary>
    /// Base Controller
    /// </summary>
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        private readonly IUserContextService _userContextService;

        protected BaseController(IUserContextService userContextService)
        {
            _userContextService = userContextService;
        }

        protected Guid CurrentUserId => _userContextService.GetUserId();
        protected string CurrentUserName => _userContextService.GetUserName();
        protected RoleEnum CurrentUserRole => _userContextService.GetUserRole();

        protected async Task<Guid> GetSupplierIdAsync(
        TextileDbContext context,CancellationToken cancellationToken = default)
        {
            // Role validation
            if (CurrentUserRole != RoleEnum.Supplier)
                throw new UnauthorizedAccessException("Current user is not a Supplier");

            // Resolve SupplierId using UserId from token
            var supplierId = await context.Suppliers
                .Where(s => s.UserId == CurrentUserId)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (supplierId == Guid.Empty)
                throw new UnauthorizedAccessException("Supplier profile not found");

            return supplierId;
        }

        protected IActionResult ApiResponse<T>(T data, string message = null)
        {
            return Ok(new
            {
                Success = true,
                Message = message ?? string.Empty,
                Data = data
            });
        }

        protected IActionResult ApiError(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                Success = false,
                Message = message
            });
        }

        protected async Task<int> GetCurrentFinanceYearIdAsync(
                 TextileDbContext context)
        {
            
            var financeYear = await context.FinanceYears
                .AsNoTracking()
                .FirstOrDefaultAsync(fy => fy.IsActive && !fy.IsClosed);

            if (financeYear == null)
                throw new Exception("Current finance year not found");

            return financeYear.Id;
        }
    }

}
