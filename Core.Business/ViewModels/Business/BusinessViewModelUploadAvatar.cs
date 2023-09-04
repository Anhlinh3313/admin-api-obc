using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Business
{
    public class BusinessViewModelUploadAvatar
    {
        public BusinessViewModelUploadAvatar()
        {
            
        }
        public IFormFile File { get; set; }
    }
}
