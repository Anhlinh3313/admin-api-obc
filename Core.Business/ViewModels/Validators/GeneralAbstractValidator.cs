using System;
using Core.Data.Abstract;
using Core.Entity.Abstract;
using FluentValidation;

namespace Core.Business.ViewModels.Validators
{
    public class GeneralAbstractValidator<TViewModel, TModel> : BaseCRUDAbstractValidator<TViewModel, TModel>
        where TViewModel : class, IEntitySimple, new()
        where TModel : class, IEntitySimple, new()
    {
        protected readonly EntitySimpleValidator<TModel> _esv;

        public GeneralAbstractValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _esv = new EntitySimpleValidator<TModel>(_unitOfWork);

            When(x => x != null, () =>
            {
                //Insert
                When(x => x.Id == 0, () => ValidateInsert());

                //Update
                When(x => x.Id > 0, () => ValidateUpdate());
            });
        }

        public void ValidateInsert()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorMessage.General.NameNotEmpty);
                       //.Must(_esv.UniqueName).WithMessage(ValidatorMessage.General.UniqueName);
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(ValidatorMessage.General.CodeNotEmpty)
                .Must(_esv.UniqueCode).WithMessage(ValidatorMessage.General.UniqueCode);
        }

        public void ValidateUpdate()
        {
            RuleFor(x => x.Id).Must(EntityExist).WithMessage(ValidatorMessage.General.NotExist);
            When(x => EntityExist(x.Id), () =>
            {
                RuleFor(x => x.ConcurrencyStamp).Must(_esv.ConcurrencyStamp).WithMessage(ValidatorMessage.General.ConcurrencyStamp);
                RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessage.General.NameNotEmpty);
                //RuleFor(x => x).Must(_esv.UniqueName).WithMessage(ValidatorMessage.General.UniqueName);
                RuleFor(x => x.Code).NotEmpty().WithMessage(ValidatorMessage.General.CodeNotEmpty);
                RuleFor(x => x).Must(_esv.UniqueCode).WithMessage(ValidatorMessage.General.UniqueCode);
            });
        }
    }
}
