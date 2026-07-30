using Microsoft.EntityFrameworkCore.Storage;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;

namespace Textile.Core.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TextileDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(TextileDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        // Begin transaction
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        // Commit transaction
        public async Task CommitTranscationAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            catch
            {
                await RollbackTranscationAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        // Rollback transaction
        public async Task RollbackTranscationAsync()
        {

            await (_transaction?.RollbackAsync() ?? Task.CompletedTask);
            _transaction?.Dispose();
            _transaction = null;
           
        }

        // Generic repository resolution
        public IRepository<TEntity, TEntityId> Repository<TEntity, TEntityId>()
            where TEntity : DatabaseEntity<TEntityId>
        {
            if(_repositories.ContainsKey(typeof(TEntity)))
                return (IRepository<TEntity,TEntityId>)_repositories[typeof(TEntity)];
            
            var repository = new BaseRepository<TEntity,TEntityId>(_context);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        // Save changes outside transaction
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

}
