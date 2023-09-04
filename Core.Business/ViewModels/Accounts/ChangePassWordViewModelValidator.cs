using System;
using Core.Business.ViewModels.Validators;
using Core.Business.ViewModels.Validators.Properties;
using Core.Data.Abstract;
using Core.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Core.Business.ViewModels.Accounts
{
    public class ChangePassWordViewModelValidator : BaseCRUDAbstractValidator<ChangePassWordViewModel, Entity.Entities.User>
    {
        public ChangePassWordViewModelValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            var av = new AccountValidator(_unitOfWork);
        }
    }
}
