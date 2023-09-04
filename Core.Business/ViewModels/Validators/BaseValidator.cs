using System;
using System.Linq.Expressions;
using Core.Business.ViewModels.Abstract;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using FluentValidation.Resources;
using FluentValidation.Validators;

namespace Core.Business.ViewModels.Validators
{
    public class BaseValidator<TEntity> : PropertyValidator where TEntity : class, IEntityGeneral, new()
    {
        protected readonly IUnitOfWork _unitOfWork;
        private const string errorMessage = "ErrorMessage";

        public BaseValidator(IUnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return true;
        }

		public bool Exist(int id)
		{
			return _unitOfWork.RepositoryR<TEntity>().Any(x => x.Id == id);
		}

		public bool ConcurrencyStamp(string value)
		{
            return _unitOfWork.RepositoryR<TEntity>().Any(x => x.ConcurrencyStamp.Equals(value));
		}

        public bool SetErrorMessage(PropertyValidatorContext context, string message)
		{
			context.MessageFormatter.AppendArgument(errorMessage, message);
			return false;
		}
    }
}
