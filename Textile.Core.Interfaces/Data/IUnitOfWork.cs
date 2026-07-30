using Textile.Core.Entities.DbEnitites;

namespace Textile.Core.Interfaces.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>() where TEntity : DatabaseEntity<TEntityId>;
        Task BeginTransactionAsync();
        Task CommitTranscationAsync();
        Task RollbackTranscationAsync();
        Task<int> SaveChangesAsync();
    }
}
