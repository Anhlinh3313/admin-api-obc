using System;
using Core.Business.ViewModels.Validators;
using Core.Business.ViewModels.Validators.Properties;
using Core.Data.Abstract;
using Core.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Core.Business.ViewModels.Accounts
{
    public class UpdateAccountViewModelValidator : BaseCRUDAbstractValidator<UpdateAccountViewModel, Entity.Entities.User>
    {
        public UpdateAccountViewModelValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            AccountValidator accountValidator = new AccountValidator(unitOfWork);
			//var roleValidator = new EntitySimpleValidator<Role>(unitOfWork);

            RuleFor(x => x.Id).Must(EntityExist).WithMessage(ValidatorMessage.Account.NotExist);
            When(x => EntityExist(x.Id), () =>
            {
                RuleFor(x => x.ConcurrencyStamp).Must(accountValidator.ConcurrencyStamp).WithMessage(ValidatorMessage.General.ConcurrencyStamp);
				RuleFor(x => x.Code).NotEmpty().WithMessage(ValidatorMessage.Account.CodeNotEmpty);
				RuleFor(x => x).Must(accountValidator.UniqueCode).WithMessage(ValidatorMessage.Account.UniqueCode);
				RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessage.Account.FullNameNotEmpty);

                RuleFor(x => x.Email)
                .Must(accountValidator.ValidEmail).WithMessage(ValidatorMessage.Account.EmailInvalid);

                //RuleFor(x => x)
                //.Must(accountValidator.NotEmptyEmail).WithMessage(ValidatorMessage.Account.EmailNotEmpty);

                RuleFor(x => x.IdentityCard)
					.Must(accountValidator.IdentityCard).WithMessage(ValidatorMessage.Account.IdentityCardInvalid)
					.Unless(x => string.IsNullOrEmpty(x.IdentityCard));

                RuleFor(x=>x).Must(accountValidator.UniqueEmail).WithMessage(ValidatorMessage.Account.UniqueEmail);

                RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(ValidatorMessage.Account.PhonenumberNotEmpty);

                RuleFor(x => x).Must(accountValidator.UniquePhone).WithMessage(ValidatorMessage.Account.UniquePhone);
            });
        }
    }
}
