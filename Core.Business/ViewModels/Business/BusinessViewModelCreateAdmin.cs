using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Business
{
    public class BusinessViewModelCreateAdmin : IEntityBase
    {
        public BusinessViewModelCreateAdmin()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int? CustomerRoleId { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public DateTime IdentityCardDate { get; set; }
        public int IdentityCardPlaceId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int? StatusId { get; set; }
        public string Password { get; set; }

        public string BusinessName { get; set; }
        public string TaxCode { get; set; }
        public string Position { get; set; }
        public int ProfessionId { get; set; }
        public int FieldOperationsId { get; set; }
        public int BusinessProvinceId { get; set; }
        public int BusinessDistrictId { get; set; }
        public int BusinessWardId { get; set; }
        public int? ParticipatingProvinceId { get; set; }
        public int? ParticipatingRegionId { get; set; }
        public int? ParticipatingChapterId { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessDescription { get; set; }

        public int MembershipId { get; set; }
        public DateTime CreatedWhen { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Time { get; set; }
        public int ExpenseId { get; set; }
        public DateTime? ExtendDate { get; set; }
    }
}
