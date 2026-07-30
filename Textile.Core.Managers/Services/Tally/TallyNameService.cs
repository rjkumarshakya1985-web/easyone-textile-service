using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.Tally;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services.Tally;

namespace Textile.Core.Managers.Services.Tally
{
    public class TallyNameService:ITallyNameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public TallyNameService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }       
        public async Task <bool> UpdateBulkTallyNames(List <TallyNameRequest> items)
        {
            if (items == null || !items.Any())
                throw new Exception("No data provided");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var grouped = items.GroupBy(x => x.Type);

                foreach (var group in grouped)
                {
                    switch (group.Key)
                    {
                        case TallyType.Agent: // Guid (Agent)

                            var agentDict = group
                                .Where(x => Guid.TryParse(x.Id, out _))
                                .ToDictionary(x => Guid.Parse(x.Id), x => x);

                            var agentIds = agentDict.Keys.ToList();

                            var agents = await _context.Agents
                                .Where(x => agentIds.Contains(x.Id))
                                .ToListAsync();

                            foreach (var entity in agents)
                            {
                                if (agentDict.TryGetValue(entity.Id, out var req))
                                    entity.TallyLedgerName = req.TallyName;
                            }

                            break;
                        case TallyType.Supplier: // Guid (Supplier)

                            var supplierDict = group
                                .Where(x => Guid.TryParse(x.Id, out _))
                                .ToDictionary(x => Guid.Parse(x.Id), x => x);

                            var supplierIds = supplierDict.Keys.ToList();

                            var supplier = await _context.Suppliers
                                .Where(x => supplierIds.Contains(x.Id))
                                .ToListAsync();

                            foreach (var entity in supplier)
                            {
                                if (supplierDict.TryGetValue(entity.Id, out var req))
                                    entity.TallyLedgerName = req.TallyName;
                            }

                            break;

                        case TallyType.StockGroup: // int

                            var groupDict = group
                                .Where(x => int.TryParse(x.Id, out _))
                                .ToDictionary(x => int.Parse(x.Id), x => x);

                            var groupIds = groupDict.Keys.ToList();

                            var stockGroups = await _context.StockGroups
                                .Where(x => groupIds.Contains(x.Id))
                                .ToListAsync();

                            foreach (var entity in stockGroups)
                            {
                                if (groupDict.TryGetValue(entity.Id, out var req))
                                    entity.TallyLedgerName = req.TallyName;
                            }

                            break;

                        case TallyType.StockItem: // Guid

                            var itemDict = group
                                .Where(x => Guid.TryParse(x.Id, out _))
                                .ToDictionary(x => Guid.Parse(x.Id), x => x);

                            var itemIds = itemDict.Keys.ToList();

                            var itemsData = await _context.SupplierProducts
                                .Where(x => itemIds.Contains(x.Id))
                                .ToListAsync();

                            foreach (var entity in itemsData)
                            {
                                if (itemDict.TryGetValue(entity.Id, out var req))
                                    entity.TallyLedgerName = req.TallyName;
                            }

                            break;

                        default:
                            throw new Exception($"Invalid Type: {group.Key}");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
