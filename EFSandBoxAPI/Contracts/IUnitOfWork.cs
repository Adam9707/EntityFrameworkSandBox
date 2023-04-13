using EFSandBoxAPI.Repositories;

namespace EFSandBoxAPI.Contracts
{
    public interface IUnitOfWork
    {
        void Compleate();
        void Dispose();
        IGenericRepository<T> Repository<T>() where T : class;
        void SaveChanges();
        IUnitOfWork UseTransaction();
    }
}