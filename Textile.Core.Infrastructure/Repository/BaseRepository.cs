using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;

namespace Textile.Core.Infrastructure.Repository
{
    public class BaseRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>
            where TEntity : DatabaseEntity<TEntityId>
    {

        private readonly TextileDbContext _ctx;

        public BaseRepository(TextileDbContext context)
        {
            _ctx = context;
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await _ctx.Set<TEntity>().AddAsync(entity);
            await _ctx.SaveChangesAsync();
            return result.Entity;
        }

        public  async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await _ctx.Set<TEntity>().AddAsync(entity);
            }
 
            await _ctx.SaveChangesAsync();
            return entities;
        }

        public async Task<bool> DeleteAllAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _ctx.Entry(entity).State = EntityState.Deleted;
            }
           

            var result = await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            _ctx.Entry(entity).State = EntityState.Deleted;
            var result = await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var command = _ctx.Set<TEntity>().Where(predicate);
            var entities = await command.ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {

            IQueryable<TEntity> query = _ctx.Set<TEntity>();

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply predicate only if provided
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var entities = await _ctx.Set<TEntity>().ToListAsync();
            return entities;
        }

        public async Task<TEntity> GetByIdAsync(TEntityId id)
        {
           var entity = await _ctx.Set<TEntity>().SingleOrDefaultAsync(x=>x.Id.Equals(id));
            return entity;
        }

        public async Task<TEntity> GetByIdAsync(TEntityId id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _ctx.Set<TEntity>();

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply the id filter
            var entity = await query.SingleOrDefaultAsync(x => x.Id.Equals(id));

            return entity;
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var command = _ctx.Set<TEntity>().Where(predicate);
            var entity = await command.SingleOrDefaultAsync();
            return entity;
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var command = _ctx.Set<TEntity>().Where(predicate);

            foreach (var include in includes)
            {
                command = command.Include(include);
            }

            var entity = await command.SingleOrDefaultAsync();
            return entity;

        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = _ctx.Set<TEntity>().Update(entity);
            await _ctx.SaveChangesAsync();
            return result.Entity;
        }
    }
}
