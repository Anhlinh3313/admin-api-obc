using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Core.Data.Abstract
{
    public interface IEntityCRUDRepository<T> : IEntityRRepository<T> where T : class, new()
	{
        void Insert(T entity);
        void InsertNotSetEnabled(T entity);
        void InsertNotSetEnabledNotCurrentUserId(T entity);
        void InsertNotCurrentUserId(T entity);
        void Update(T entity);
        void UpdateNotCurrentUserId(T entity);
        void UpdateOnly(T entity);
        void InsertAndUpdate(T entity);
        void Delete(int id);
        void DeleteEmpty(int id);
        void Delete(T entity);
        void DeleteEmpty(T entity);
        void DeleteWhere(Expression<Func<T, bool>> predicate);
        void DeleteEmptyWhere(Expression<Func<T, bool>> predicate);
        int Commit();
        Task<int> CommitAsync();
    }
}
