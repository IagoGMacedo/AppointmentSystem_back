using System.Data;

namespace AppointmentSystem.Repository.Interface.IRepository
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
