using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Core.Infrastructure.Utils;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Core
{
    public class EntityRRepository<T> : IEntityRRepository<T>
        where T : class, IEntityBase, new()
    {
        private ApplicationContext _context;

        public EntityRRepository(ApplicationContext context)
        {
            _context = context;
        }

        public bool Any()
        {
            return _context.Set<T>().Any<T>(f50P_R26 => f50P_R26.IsEnabled == true);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R31 => f50P_R31.IsEnabled == true);
            return _context.Set<T>().Any<T>(predicate);
        }

        public bool AnyNotIsEnabled(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any<T>(predicate);
        }

        public Task<bool> AnyAsync()
        {
            return _context.Set<T>().AnyAsync<T>(f50P_R36 => f50P_R36.IsEnabled == true);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R41 => f50P_R41.IsEnabled == true);
            return _context.Set<T>().AnyAsync<T>(predicate);
        }

        public int Count()
        {
            return _context.Set<T>().Count(f50P_R46 => f50P_R46.IsEnabled == true);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R51 => f50P_R51.IsEnabled == true);
            return _context.Set<T>().Count(predicate);
        }

        public Task<int> CountAsync()
        {
            return _context.Set<T>().CountAsync(f50P_R56 => f50P_R56.IsEnabled == true);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R61 => f50P_R61.IsEnabled == true);
            return _context.Set<T>().CountAsync(predicate);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R66 => f50P_R66.IsEnabled == true);
            return _context.Set<T>().Where(predicate);
        }
        public IQueryable<T> FindByNotEnabled(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public IAsyncEnumerable<T> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R71 => f50P_R71.IsEnabled == true);
            return _context.Set<T>().Where(predicate).AsAsyncEnumerable();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, string[] includeProperties)
        {
            T t = new T();
            List<string> listIncludeProps = new List<string>();

            foreach (var item in includeProperties)
            {
                if (ClassUtil.HasProperty(t, item))
                {
                    listIncludeProps.Add(item);
                }
            }
            predicate = predicate.And(f50P_R81 => f50P_R81.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in listIncludeProps)
            {
                try
                {
                    query = query.Include(includeProperty);
                }
                catch { }
            }
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            predicate = predicate.And(f50P_R102 => f50P_R102.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IAsyncEnumerable<T> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            predicate = predicate.And(f50P_R113 => f50P_R113.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsAsyncEnumerable();
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().Where(f50134 => f50134.IsEnabled == true);
        }

        public IQueryable<T> GetAllNotEnabled()
        {
            return _context.Set<T>();
        }

        public IQueryable<T> GetAll(string[] includeProperties)
        {
            T t = new T();
            List<string> listIncludeProps = new List<string>();

            foreach (var item in includeProperties)
            {
                if (ClassUtil.HasProperty(t, item))
                {
                    listIncludeProps.Add(item);
                }
            }
            IQueryable<T> query = _context.Set<T>().Where(f50P_R139 => f50P_R139.IsEnabled == true);
            foreach (var includeProperty in listIncludeProps)
            {
                try
                {
                    query = query.Include(includeProperty);
                }
                catch { }
            }
            return query;
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(f50P_R153 => f50P_R153.IsEnabled == true);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IAsyncEnumerable<T> GetAllAsync()
        {
            return _context.Set<T>().Where(f50P_R163 => f50P_R163.IsEnabled == true).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<T> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(f50P_R168 => f50P_R168.IsEnabled == true);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsAsyncEnumerable();
        }


        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, string[] includeProperties)
        {
            T t = new T();
            List<string> listIncludeProps = new List<string>();

            foreach (var item in includeProperties)
            {
                if (ClassUtil.HasProperty(t, item))
                {
                    listIncludeProps.Add(item);
                }
            }
            predicate = predicate.And(f50P_R189 => f50P_R189.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);

            foreach (var includeProperty in listIncludeProps)
            {
                try
                {
                    query = query.Include(includeProperty);
                }
                catch { }
            }
            return query;
        }

        public T GetSingle(int id)
        {
            return _context.Set<T>().SingleOrDefault(f50P_R204 => f50P_R204.Id == id && f50P_R204.IsEnabled == true);
        }

        public T GetSingle(int id, string[] includeProperties)
        {
            T t = new T();
            List<string> listIncludeProps = new List<string>();

            foreach (var item in includeProperties)
            {
                if (ClassUtil.HasProperty(t, item))
                {
                    listIncludeProps.Add(item);
                }
            }
            IQueryable<T> query = _context.Set<T>().Where(f50P_R219 => f50P_R219.Id == id && f50P_R219.IsEnabled == true);
            foreach (var includeProperty in listIncludeProps)
            {
                try
                {
                    query = query.Include(includeProperty);
                }
                catch { }
            }
            return query.FirstOrDefault();
        }

        public T GetSingle(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(f50P_R233 => f50P_R233.Id == id && f50P_R233.IsEnabled == true);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault();
        }

        public Task<T> GetSingleAsync(int id)
        {
            return _context.Set<T>().SingleOrDefaultAsync(f50P_R243 => f50P_R243.Id == id && f50P_R243.IsEnabled == true);
        }

        public Task<T> GetSingleAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(f50_R254 => f50_R254.Id == id && f50_R254.IsEnabled == true);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefaultAsync();
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R248 => f50P_R248.IsEnabled == true);
            return _context.Set<T>().SingleOrDefault(predicate);
        }


        public T GetSingleNotEnabled(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().SingleOrDefault(predicate);
        }

        public Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            predicate = predicate.And(f50P_R264 => f50P_R264.IsEnabled == true);
            return _context.Set<T>().SingleOrDefaultAsync(predicate);
        }
        public Task<T> GetSingleNotEnabledAsync(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, string[] includeProperties)
        {
            T t = new T();
            List<string> listIncludeProps = new List<string>();
            foreach (var item in includeProperties)
            {
                if (ClassUtil.HasProperty(t, item))
                {
                    listIncludeProps.Add(item);
                }
            }
            predicate = predicate.And(f50P_R280 => f50P_R280.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in listIncludeProps)
            {
                try
                {
                    query = query.Include(includeProperty);
                }
                catch { }
            }
            return query.FirstOrDefault();
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            predicate = predicate.And(f50P_R295 => f50P_R295.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault();
        }

        public Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            predicate = predicate.And(f50P_R306 => f50P_R306.IsEnabled == true);
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefaultAsync();
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
