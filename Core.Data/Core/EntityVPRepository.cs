using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Core
{
    public class EntityVPRepository<T> : IEntityVPRepository<T>
        where T : class, IEntityProcView, new()
    {
        private ApplicationContext _context;
		public EntityVPRepository(ApplicationContext context)
        {
			_context = context;
		}

		public T ExecProcedureSingle(IEntityProc entityProc)
		{
			return _context.Set<T>().FromSqlRaw(entityProc.GetQuery(), entityProc.GetParams()).AsEnumerable().FirstOrDefault();
		}

		public T ExecProcedureSingle(string query, params object[] parameters)
		{
			return _context.Set<T>().FromSqlRaw(query, parameters).AsEnumerable().FirstOrDefault();
		}

		public IEnumerable<T> ExecProcedure(IEntityProc entityProc)
		{
            return _context.Set<T>().FromSqlRaw<T>(entityProc.GetQuery(), entityProc.GetParams());
		}

		public IEnumerable<T> ExecProcedure(string query, params object[] parameters)
		{
			return _context.Set<T>().FromSqlRaw<T>(query, parameters);
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
