using Textile.Core.Entities.Models.Response.Dashboard;

namespace Textile.Core.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<SupplierDashboardResponse> GetSupplierDashboard(Guid supplierId);

        Task<AdminDashboardResponse> GetAdminDashboard();
    }
}
