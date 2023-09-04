using System;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using FluentValidation;

namespace Core.Business.ViewModels.Validators
{
    public class BaseAbstractValidator<TViewModel, TModel> : AbstractValidator<TViewModel> where TModel : class, IEntityBase, new()
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseAbstractValidator(IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;
		}
    }
}
