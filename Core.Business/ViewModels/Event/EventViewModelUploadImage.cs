using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelUploadImage
    {
        public EventViewModelUploadImage()
        {
            
        }
        public int EventId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
