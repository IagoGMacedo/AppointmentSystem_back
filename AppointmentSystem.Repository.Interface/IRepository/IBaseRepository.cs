using AppointmentSystem.Entity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace AppointmentSystem.Repository.Interface.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity> GetById(object id);
        Task<List<TEntity>> GetAll();
        Task<TEntity> Create(TEntity entity);
        Task Create(IEnumerable<TEntity> entitys);
        Task<TEntity> Update(TEntity entity);
        Task Delete(TEntity entity);
        Task Delete(IEnumerable<TEntity> entitys);
        Task DeleteById(object id);
    }
}
