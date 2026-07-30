using MediatR;
using Textile.Core.Entities.Enums;

namespace Textile.Core.Managers.Commands.Sales
{
    public class GenerateVoucherNumberCommand : IRequest<string>
    {
        public VoucherTypeEnum VoucherType { get; }

        public int FinanceYearId { get; set; }

        public GenerateVoucherNumberCommand(VoucherTypeEnum voucherType, int financeYearId)
        {
            VoucherType = voucherType;
            FinanceYearId = financeYearId; 
        }
    }
}
