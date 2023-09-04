using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Customer
{
    public class CustomerViewModelCreate : IEntityBase
    {
        public CustomerViewModelCreate()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public int? ProvinceId { get; set; }
        public bool IsActive { get; set; }
        public int CustomerRoleId { get; set; }
    }
}
