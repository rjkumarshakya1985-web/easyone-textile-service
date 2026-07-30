using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales;

namespace Textile.Core.Managers.Handlers.Commands.Sales
{
    public class GenerateVoucherNumberCommandHandler
    : IRequestHandler<GenerateVoucherNumberCommand, string>
    {
        private readonly TextileDbContext _context;

        public GenerateVoucherNumberCommandHandler(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(
          GenerateVoucherNumberCommand request,
          CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var today = DateTime.Today;

            // Finance year

            // Series
            var series = await _context.VoucherNumberSeries
                .FirstOrDefaultAsync(x =>
                    x.VoucherType == (int)request.VoucherType &&
                    x.FinanceYearId == request.FinanceYearId,
                    cancellationToken);

            // Create series if missing
            if (series == null)
            {
                var voucherType = await _context.VoucherTypes
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == (int)request.VoucherType, cancellationToken);

                series = new VoucherNumberSeries
                {
                    FinanceYearId = request.FinanceYearId,
                    CurrentNumber = 0,
                    Prefix = voucherType.Prefix,
                    VoucherType = (int)request.VoucherType,
                    NumberLength = voucherType.NumberLength
                };

                _context.VoucherNumberSeries.Add(series);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Call stored procedure
            var number = _context.Database
                .SqlQuery<int>($@"
            EXEC GetNextVoucherNumber 
                @VoucherType = {(int)request.VoucherType}, 
                @FinanceYearId = {request.FinanceYearId}")
                .AsEnumerable()
                .First();

            var formatted = number.ToString().PadLeft(series.NumberLength, '0');

            await transaction.CommitAsync(cancellationToken);

            return $"{series.Prefix}{formatted}";
        }

    }
}
