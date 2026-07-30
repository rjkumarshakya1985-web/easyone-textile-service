using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Textile.Core.Infrastructure.Helpers
{
    public static class DbExceptionHelper
    {
        public static bool IsDuplicateKey(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx
                   && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }
    }
}
