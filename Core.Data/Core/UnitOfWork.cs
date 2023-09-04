using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Infrastructure.Http;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Core.Entity.Entities;
using System.Threading.Tasks;

namespace Core.Data.Core
{
    public class UnitOfWork : BaseDisposable, IUnitOfWork
	{
		private readonly ApplicationContext _context;
		private Dictionary<string, object> _repository;
		private Dictionary<string, object> _repositoriesCRUD;
		private Dictionary<string, object> _repositoriesR;

		public UnitOfWork(ApplicationContext context)
		{
			_context = context;
			_repository = new Dictionary<string, object>();
			_repositoriesCRUD = new Dictionary<string, object>();
			_repositoriesR = new Dictionary<string, object>();
		}

        public IEntityVPRepository<T> Repository<T>() where T : class, IEntityProcView, new()
		{
			if (_repository == null)
			{
				_repository = new Dictionary<string, object>();
			}

			var type = typeof(T).Name;

			if (!_repository.ContainsKey(type))
			{
				var repositoryType = typeof(EntityVPRepository<>);
				var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
				_repository.Add(type, repositoryInstance);
			}
			return (EntityVPRepository<T>)_repository[type];
		}

		public IEntityCRUDRepository<T> RepositoryCRUD<T>() where T : class, IEntityBase, new()
		{
			if (_repositoriesCRUD == null)
			{
				_repositoriesCRUD = new Dictionary<string, object>();
			}

			var type = typeof(T).Name;

			if (!_repositoriesCRUD.ContainsKey(type))
			{
				var repositoryType = typeof(EntityCRUDRepository<>);
				var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
				_repositoriesCRUD.Add(type, repositoryInstance);
			}
            return (EntityCRUDRepository<T>)_repositoriesCRUD[type];
		}

        public IEntityRRepository<T> RepositoryR<T>() where T : class, IEntityBase, new()
		{
			if (_repositoriesR == null)
			{
				_repositoriesR = new Dictionary<string, object>();
			}

			var type = typeof(T).Name;

			if (!_repositoriesR.ContainsKey(type))
			{
				var repositoryType = typeof(EntityRRepository<>);
				var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
				_repositoriesR.Add(type, repositoryInstance);
			}
			return (EntityRRepository<T>)_repositoriesR[type];
		}

		public int Commit()
		{
			return _context.SaveChanges();
		}

		public Task<int> CommitAsync()
		{
            return _context.SaveChangesAsync();
		}

		public void Rollback()
		{
			_context.ChangeTracker
				.Entries()
				.ToList()
				.ForEach(x => x.Reload());
		}

		protected override void DisposeCore()
		{
			if (_context != null)
				_context.Dispose();
		}
    }
}
