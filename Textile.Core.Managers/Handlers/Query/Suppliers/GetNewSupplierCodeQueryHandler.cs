using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Suppliers
{
    public class GetNewSupplierCodeQueryHandler : IRequestHandler<GetNewSupplierCodeQuery, string>
    {
        private readonly TextileDbContext _context;

        public GetNewSupplierCodeQueryHandler(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> Handle(GetNewSupplierCodeQuery request, CancellationToken cancellationToken)
        {
            // Step 1: Get max code from database
            var rowCode = await _context.Suppliers.CountAsync(cancellationToken);



            int newCode = rowCode + 1;

            // Step 5: Format to 4 digits: 0001, 0009, 0010, 0123, 1001...
            return newCode.ToString("D4");
        }
    }

}
