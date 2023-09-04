using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Business
{
    public class BusinessViewModelTextQrCode
    {
        public BusinessViewModelTextQrCode()
        {
            
        }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BusinessName { get; set; }
        public string ProfessionName { get; set; }
        public string FieldOperationName { get; set; }
        public string ProvinceName { get; set; }
    }
}
