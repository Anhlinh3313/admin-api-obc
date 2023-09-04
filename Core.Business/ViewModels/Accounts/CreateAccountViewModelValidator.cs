using System;
using Core.Business.ViewModels.Validators;
using Core.Business.ViewModels.Validators.Properties;
using Core.Data.Abstract;
using Core.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Core.Business.ViewModels.Accounts
{
    public class CreateAccountViewModelValidator : BaseAbstractValidator<CreateAccountViewModel, Entity.Entities.User>
    {
        public CreateAccountViewModelValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            AccountValidator accountValidator = new AccountValidator(unitOfWork);
            //var roleValidator = new EntitySimpleValidator<Role>(unitOfWork);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(ValidatorMessage.Account.UserNameNotEmpty)
                .Must(accountValidator.UniqueUserName).WithMessage(ValidatorMessage.Account.UniqueUserName);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(ValidatorMessage.Account.CodeNotEmpty)
                .Must(accountValidator.UniqueCode).WithMessage(ValidatorMessage.Account.UniqueCode);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessage.Account.FullNameNotEmpty);

            RuleFor(x => x.PassWord).NotEmpty().WithMessage(ValidatorMessage.Account.PassWordNotEmpty);

            RuleFor(x => x.Email)
                .Must(accountValidator.ValidEmail).WithMessage(ValidatorMessage.Account.EmailInvalid)
                .Must(accountValidator.UniqueEmail).WithMessage(ValidatorMessage.Account.UniqueEmail);

            //RuleFor(x => x)
            //    .Must(accountValidator.NotEmptyEmail).WithMessage(ValidatorMessage.Account.EmailNotEmpty);

            RuleFor(x => x.IdentityCard)
                .Must(accountValidator.IdentityCard).WithMessage(ValidatorMessage.Account.IdentityCardInvalid)
				.Unless(x => string.IsNullOrEmpty(x.IdentityCard));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(ValidatorMessage.Account.PhonenumberNotEmpty)
                .Must(accountValidator.UniquePhone).WithMessage(ValidatorMessage.Account.UniquePhone);
        }
    }
}
