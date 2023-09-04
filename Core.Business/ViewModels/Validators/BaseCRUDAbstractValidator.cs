using System;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using FluentValidation;

namespace Core.Business.ViewModels.Validators
{
    public class BaseCRUDAbstractValidator<TViewModel, TModel> : AbstractValidator<TViewModel> where TModel : class, IEntityGeneral, new()
    {
        private BaseValidator<TModel> _baseValidator;
        protected readonly IUnitOfWork _unitOfWork;
        private bool? IsEntityExist { get; set; }

        public BaseCRUDAbstractValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseValidator = new BaseValidator<TModel>(_unitOfWork);
        }

        protected bool EntityExist(int id)
        {
            if(IsEntityExist == null)
                IsEntityExist = _baseValidator.Exist(id);
            return IsEntityExist.Value;
        }
    }
}
