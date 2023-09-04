using System;
using System.ComponentModel.DataAnnotations;
using Core.Data.Abstract;
using Core.Entity.Abstract;

namespace Core.Data.Core.Utils
{
    public static class EntityUtil
    {
        public static IEntityRRepository<T> GetEntityR<T>(ValidationContext validationContext) where T : class, IEntityBasic, new()
        {
            return (IEntityCRUDRepository<T>)validationContext.GetService(typeof(IEntityCRUDRepository<T>));
        }

        public static IUnitOfWork GetUnitOfWork(ValidationContext validationContext)
		{
			return (IUnitOfWork)validationContext.GetService(typeof(IUnitOfWork));
		}
    }
}
