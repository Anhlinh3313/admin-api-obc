using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Core.Infrastructure.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Data.Core
{
    public class EntityCRUDRepository<T> : EntityRRepository<T>, IEntityCRUDRepository<T>
        where T : class, IEntityBase, new()
    {
        private ApplicationContext _context;
        //private string _tableName;
        private int _userId;

        public EntityCRUDRepository(ApplicationContext context) : base(context)
        {
            _context = context;
            //var mapping = context.Model.GetEntityTypes(typeof(T));
            //_tableName = mapping.GetTableName();
            //_userId = HttpContext.CurrentUserId;
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public void DeleteEmpty(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public void Delete(int id)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(x => x.Id == id);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public void DeleteEmpty(int id)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(x => x.Id == id);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }

        }

        public void DeleteEmptyWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }

        }

        public void Insert(T entity)
        {
            if (entity is IEntityBasic)
            {
                DateTime currentDate = DateTime.Now;
                int currentUserId = HttpContext.CurrentUserId;
                var tempEntity = entity as IEntityBasic;
                tempEntity.CreatedBy = currentUserId;
                tempEntity.CreatedWhen = currentDate;
                tempEntity.ModifiedBy = currentUserId;
                tempEntity.ModifiedWhen = currentDate;
                tempEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
                entity = tempEntity as T;
            }
            entity.IsEnabled = true;
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public void InsertNotSetEnabled(T entity)
        {
            if (entity is IEntityBasic)
            {
                DateTime currentDate = DateTime.Now;
                int currentUserId = HttpContext.CurrentUserId;
                var tempEntity = entity as IEntityBasic;
                tempEntity.CreatedBy = currentUserId;
                tempEntity.CreatedWhen = currentDate;
                tempEntity.ModifiedBy = currentUserId;
                tempEntity.ModifiedWhen = currentDate;
                tempEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
                entity = tempEntity as T;
            }
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public void InsertNotSetEnabledNotCurrentUserId(T entity)
        {
            if (entity is IEntityBasic)
            {
                DateTime currentDate = DateTime.Now;
                var tempEntity = entity as IEntityBasic;
                tempEntity.CreatedBy = 1;
                tempEntity.CreatedWhen = currentDate;
                tempEntity.ModifiedBy = 1;
                tempEntity.ModifiedWhen = currentDate;
                tempEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
                entity = tempEntity as T;
            }
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public void InsertNotCurrentUserId(T entity)
        {
            if (entity is IEntityBasic)
            {
                DateTime currentDate = DateTime.Now;
                //int currentUserId = HttpContext.CurrentUserId;
                var tempEntity = entity as IEntityBasic;
                tempEntity.CreatedBy = 1;
                tempEntity.CreatedWhen = currentDate;
                tempEntity.ModifiedBy = 1;
                tempEntity.ModifiedWhen = currentDate;
                tempEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
                entity = tempEntity as T;
            }
            entity.IsEnabled = true;
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public void InsertAndUpdate(T entity)
        {
            if (entity.Id == 0)
                Insert(entity);
            else
                Update(entity);
        }

        public void Update(T entity)
        {
            if (entity is IEntityBasic)
            {
                DateTime currentDate = DateTime.Now;
                int currentUserId = HttpContext.CurrentUserId;
                var tempEntity = entity as IEntityBasic;
                tempEntity.ModifiedBy = currentUserId;
                tempEntity.ModifiedWhen = currentDate;
                tempEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
                entity = tempEntity as T;
            }
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            try
            {
                dbEntityEntry.State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                dbEntityEntry.State = EntityState.Detached;
            }
        }

        public void UpdateNotCurrentUserId(T entity)
        {
            if (entity is IEntityBasic)
            {
                DateTime currentDate = DateTime.Now;
                //int currentUserId = HttpContext.CurrentUserId;
                var tempEntity = entity as IEntityBasic;
                tempEntity.ModifiedBy = 1;
                tempEntity.ModifiedWhen = currentDate;
                tempEntity.ConcurrencyStamp = Guid.NewGuid().ToString();
                entity = tempEntity as T;
            }
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            try
            {
                dbEntityEntry.State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                dbEntityEntry.State = EntityState.Detached;
            }
        }
        public void UpdateOnly(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
    }
}
