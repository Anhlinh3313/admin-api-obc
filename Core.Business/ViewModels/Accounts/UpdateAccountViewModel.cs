using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Business.ViewModels.Abstract;
using Core.Data.Core.Utils;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Accounts
{
    public class UpdateAccountViewModel : IEntityBasic, IValidatableObject
    {
        public UpdateAccountViewModel()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? CreatedWhen { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedWhen { get; set; }
        public int? ModifiedBy { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string IdentityCard { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? Gender { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleId { get; set; }
        public int? UserTypeId { get; set; }
        public bool? IsPassWordBasic { get; set; }
        public bool? IsBlocked { get; set; }
        public string AvatarPath { get; set; }
        public List<int> RoleIds { get; set; }
        public int? SupplierId { get; set; }
        public int? BranchId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
            var validator = new UpdateAccountViewModelValidator(EntityUtil.GetUnitOfWork(validationContext));
			var result = validator.Validate(this);
			return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
		}
    }
}
