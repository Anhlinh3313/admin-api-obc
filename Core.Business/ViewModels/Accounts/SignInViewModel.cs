using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Data.Core.Utils;

namespace Core.Business.ViewModels.Accounts
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
            
        }

		public string UserName { get; set; }
		public string PassWord { get; set; }
        public int? HourLogin { get; set; }
        public int? MinuteLogin { get; set; }
    }
    public class SignInViewModelResponse
    {
        public SignInViewModelResponse()
        {

        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string RoleName { get; set; }
        public string Token { get; set; }
        public int Expires { get; set; }
        public DateTime ExpiresDate { get; set; }
    }
}
