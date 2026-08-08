using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.AdminMenu;
using Textile.Core.Entities.Models.Response.AdminMenu;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class AdminMenuService : IAdminMenuService
    {
        private readonly TextileDbContext _context;

        public AdminMenuService(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<AdminMenuSettingResponse>> GetAsync()
        {
            return await _context.AdminMenuSettings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new AdminMenuSettingResponse
                {
                    MenuKey = x.MenuKey,
                    Label = x.Label,
                    IsEnabled = x.IsEnabled
                })
                .ToListAsync();
        }

        public async Task<bool> SaveAsync(AdminMenuSettingRequest request, Guid currentUserId)
        {
            var currentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == currentUserId);

            if (currentUser == null ||
                currentUser.RoleId != (int)RoleEnum.SuperAdmin ||
                !currentUser.IsDeveloper)
            {
                throw new UnauthorizedAccessException("Only developer admin can update menu settings.");
            }

            var existingSettings = await _context.AdminMenuSettings.ToListAsync();
            var existingByKey = existingSettings.ToDictionary(x => x.MenuKey, StringComparer.OrdinalIgnoreCase);

            foreach (var item in request.Items)
            {
                if (string.IsNullOrWhiteSpace(item.MenuKey))
                    continue;

                var key = item.MenuKey.Trim();
                var label = string.IsNullOrWhiteSpace(item.Label) ? key : item.Label.Trim();

                if (existingByKey.TryGetValue(key, out var existing))
                {
                    existing.Label = label;
                    existing.IsEnabled = item.IsEnabled;
                }
                else
                {
                    _context.AdminMenuSettings.Add(new AdminMenuSetting
                    {
                        MenuKey = key,
                        Label = label,
                        IsEnabled = item.IsEnabled
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
