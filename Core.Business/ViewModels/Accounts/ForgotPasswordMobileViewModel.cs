using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Data.Core.Utils;

namespace Core.Business.ViewModels.Accounts
{
    public class ForgotPasswordMobileViewModel
    {
        public ForgotPasswordMobileViewModel()
        {
        }

        [EmailAddress]
        public string Email { get; set; }
        public string Code { get; set; }
        public int? CustomerId { get; set; }
        public string Language { get; set; }
    }
}
