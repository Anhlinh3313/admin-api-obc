using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Customer
{
    public class CustomerViewModelUploadAvatar
    {
        public CustomerViewModelUploadAvatar()
        {
            
        }
        public IFormFile File { get; set; }
    }
}
