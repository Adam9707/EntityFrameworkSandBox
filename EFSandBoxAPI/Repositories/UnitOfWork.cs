using EFSandBoxAPI.Contracts;
using EFSandBoxAPI.DBContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Configuration;


namespace EFSandBoxAPI.Repositories
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private DbContext dbContext;

        private bool isCompleated = false;

        public IDbContextTransaction dbContextTransaction = null;

        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public UnitOfWork(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("AVConnectionString");
            var options = new DbContextOptionsBuilder<AdventureWorks2019Context>().UseSqlServer(connectionString).Options;
            this.dbContext = new AdventureWorks2019Context(options);
        }

        public IUnitOfWork UseTransaction()
        {
            this.dbContextTransaction = dbContext.Database.BeginTransaction();
            return this;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)))
            {
                return repositories[typeof(T)] as IGenericRepository<T>;
            }
            IGenericRepository<T> repository = new GenericRepository<T>(dbContext);
            repositories.Add(typeof(T), repository);

            return repository;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public void Compleate()
        {
            isCompleated = true;
        }

        public void Dispose()
        {
            SaveChanges();

            if (!isCompleated && dbContextTransaction is not null)
            {
                dbContextTransaction.Rollback();
            }
            else if (isCompleated && dbContextTransaction is not null)
            {
                dbContextTransaction.Commit();
            }

            dbContextTransaction=null;
            GC.SuppressFinalize(this);
        }
    }
}
