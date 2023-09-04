using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Data.Core.Utils;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Accounts
{
    public class SignUpMobileViewModel : IEntityBase
    {
        public SignUpMobileViewModel()
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

        public string BusinessName { get; set; }
        public string Position { get; set; }
        public int ProfessionId { get; set; }
        public string Language { get; set; }
    }
}
