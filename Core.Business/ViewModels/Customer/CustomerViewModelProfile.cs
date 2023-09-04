using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Customer
{
    public class CustomerViewModelProfile
    {
        public CustomerViewModelProfile()
        {
            
        }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Position { get; set; }
        public string BusinessName { get; set; }
        public string ProfessionName { get; set; }
        public string FieldOperationName { get; set; }
        public string ChapterName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string QrCodePath { get; set; }
        public string Address { get; set; }
        public string AvatarPath { get; set; }
        public bool Assessed { get; set; }
        public int StatusId { get; set; }
        public string ChapterType { get; set; }
    }
}
