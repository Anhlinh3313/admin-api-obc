using Core.Business.ViewModels.Accounts;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Security;
using FluentValidation.Validators;
using System.ComponentModel.DataAnnotations;

namespace Core.Business.ViewModels.Validators.Properties
{
    public class AccountValidator : BaseValidator<Entity.Entities.User>
    {
        public AccountValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            bool result = true;

            if (context.PropertyValue is SignInViewModel)
            {
                result = SignInViewModelProcess(context);
            }

            return result;
        }

        private bool SignInViewModelProcess(PropertyValidatorContext context)
        {
            var encryption = new Encryption();
            var model = context.PropertyValue as SignInViewModel;

            var user = _unitOfWork.RepositoryCRUD<Entity.Entities.User>().GetSingle(
                x => x.UserName.Equals(model.UserName) &&
                x.PasswordHash.Equals(encryption.EncryptPassword(model.PassWord, x.SecurityStamp))
            );

            if (user == null)
            {
                return SetErrorMessage(context, ValidatorMessage.Account.InvalidUserNamePassWord);
            }
            else if (!user.IsEnabled)
            {
                return SetErrorMessage(context, ValidatorMessage.Account.AccountHasBeenBlock);
            }

            return base.IsValid(context);
        }

        public bool IdentityCard(string value)
        {
            return value.Length <= 12 ? true : false;
        }

        public bool UniqueUserName(string userName)
        {
            return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.UserName.EqualsIgnoreCase(userName));
        }

        public bool UniqueCode(string code)
        {
            return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Code.EqualsIgnoreCase(code));
        }

        public bool UniqueEmail(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Email.Equals(Email));
            }

            return true;
        }

        public bool NotEmptyEmail(UpdateAccountViewModel model)
        {
            if (model.UserTypeId == UserTypeHelper.ADMIN)
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public bool NotEmptyEmail(CreateAccountViewModel model)
        {
            if (model.UserTypeId == UserTypeHelper.ADMIN)
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public bool UniqueEmail(UpdateAccountViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {

                if (model.Id == 0)
                {
                    return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Email.Equals(model.Email));
                }
                else
                {
                    return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Email.Equals(model.Email) && x.Id != model.Id);
                }
            }
            return true;
        }

        public bool ValidEmail(string Email)
        {
            if (!string.IsNullOrEmpty(Email))
            {
                return new EmailAddressAttribute().IsValid(Email);
            }
            return true;
        }

        public bool UniquePhone(string Phonenumber)
        {
            return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.PhoneNumber.Equals(Phonenumber));
        }

        public bool UniquePhone(UpdateAccountViewModel model)
        {
            if (model.Id == 0)
            {
                return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.PhoneNumber.Equals(model.PhoneNumber));
            }
            else
            {
                return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.PhoneNumber.Equals(model.PhoneNumber) && x.Id != model.Id);
            }
        }

        public bool UniqueCode(UpdateAccountViewModel model)
        {
            if (model.Id == 0)
            {
                return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Code.EqualsIgnoreCase(model.Code));
            }
            else
            {
                return !_unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Code.EqualsIgnoreCase(model.Code) && x.Id != model.Id);
            }
        }

        public bool UserEnabled(int id)
        {
            return _unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.Id.Equals(id) && x.IsEnabled);
        }

        public bool UserEnabled(string userName)
        {
            return _unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => x.UserName.Equals(userName) && x.IsEnabled);
        }

        public bool SecurityStampValid(int userId, string securityStampHash)
        {
            return _unitOfWork.RepositoryR<Entity.Entities.User>().Any(x => new Encryption().HashSHA256(x.SecurityStamp).Equals(securityStampHash));
        }
    }
}
