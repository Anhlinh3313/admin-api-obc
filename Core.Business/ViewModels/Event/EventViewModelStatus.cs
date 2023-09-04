using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelStatus
    {
        public EventViewModelStatus()
        {
            
        }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}
