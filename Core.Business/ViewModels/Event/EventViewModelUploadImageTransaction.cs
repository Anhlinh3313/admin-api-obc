using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelUploadImageTransaction
    {
        public EventViewModelUploadImageTransaction()
        {
            
        }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int EventId { get; set; }
        public IFormFile File { get; set; }
    }
}
