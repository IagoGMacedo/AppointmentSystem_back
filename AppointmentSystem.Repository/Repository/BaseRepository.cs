using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Repository.Interface.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Repository.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly Context _context;
        protected virtual DbSet<TEntity> EntitySet { get; }
        public BaseRepository(Context context)
        {
            _context = context;
            EntitySet = _context.Set<TEntity>();
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            var entityEntry = await EntitySet.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task Delete(TEntity entity)
        {
            EntitySet.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteById(object id)
        {
            var entity = await EntitySet.FindAsync(id);

            if (entity != null)
                await Delete(entity);
        }

        public Task<List<TEntity>> GetAll() => EntitySet.ToListAsync();


        public async Task<TEntity> GetById(object id)
        {
            return await EntitySet.FindAsync(id);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var entityEntry = EntitySet.Update(entity);

            await _context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task Create(IEnumerable<TEntity> entitys)
        {
            await EntitySet.AddRangeAsync(entitys);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(IEnumerable<TEntity> entitys)
        {
            EntitySet.RemoveRange(entitys);

            await _context.SaveChangesAsync();
        }
    }
}
