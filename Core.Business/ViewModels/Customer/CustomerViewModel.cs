using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Customer
{
    public class CustomerViewModel : IEntityBase
    {
        public CustomerViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public DateTime? IdentityCardDate { get; set; }
        public int? IdentityCardPlace { get; set; }
        public string Gender { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }

        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string StatusName { get; set; }
        public string AvatarPath { get; set; }
        public string QrCodePath { get; set; }
    }
}
