using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Data.Core.Utils;

namespace Core.Business.ViewModels.Accounts
{
    public class SignInMobileViewModel
    {
        public SignInMobileViewModel()
        {
            
        }

		public string EmailOrPhone { get; set; }
		public string Password { get; set; }
        public int? HourLogin { get; set; }
        public int? MinuteLogin { get; set; }
        public string Language { get; set; }
    }
}
