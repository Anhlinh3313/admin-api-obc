using System;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Data.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IEntityVPRepository<T> Repository<T>() where T : class, IEntityProcView, new();
        IEntityCRUDRepository<T> RepositoryCRUD<T>() where T : class, IEntityBase, new();
        IEntityRRepository<T> RepositoryR<T>() where T : class, IEntityBase, new();
		int Commit();
		Task<int> CommitAsync();
        void Rollback();
    }
}
