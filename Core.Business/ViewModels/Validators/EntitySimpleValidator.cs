using System;
using System.Linq.Expressions;
using Core.Business.ViewModels.Abstract;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using Core.Infrastructure.Extensions;
using FluentValidation.Resources;
using FluentValidation.Validators;

namespace Core.Business.ViewModels.Validators
{
    public class EntitySimpleValidator<TEntity> : PropertyValidator where TEntity : class, IEntitySimple, new()
    {
        protected readonly IUnitOfWork _unitOfWork;
        private const string errorMessage = "ErrorMessage";

        public EntitySimpleValidator(IUnitOfWork unitOfWork) : base()
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

        public bool UniqueCode(string code)
        {
            return !_unitOfWork.RepositoryR<TEntity>().Any(x => x.Code.EqualsIgnoreCase(code));
        }

        public bool UniqueCode<TViewModel>(TViewModel model) where TViewModel : class, IEntitySimple, new()
        {
            return !_unitOfWork.RepositoryR<TEntity>().Any(x => x.Code.EqualsIgnoreCase(model.Code) && x.Id != model.Id);
        }

        public bool UniqueName(string name)
        {
            return !_unitOfWork.RepositoryR<TEntity>().Any(x => x.Name.EqualsIgnoreCase(name));
        }

        public bool UniqueName<TViewModel>(TViewModel model) where TViewModel : class, IEntitySimple, new()
        {
            return !_unitOfWork.RepositoryR<TEntity>().Any(x => x.Name.EqualsIgnoreCase(model.Name) && x.Id != model.Id);
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
