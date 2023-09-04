using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelCreate
    {
        public EventViewModelCreate()
        {
            
        }
        public string Name { get; set; }
        public TimeEventModel[] TimeEvents { get; set; }
        public string Objects { get; set; }
        public bool Fee { get; set; }
        public string LinkInformation { get; set; }
        public string LinkCheckIn { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
    }

    public class TimeEventModel
    {
        public TimeEventModel()
        {
            
        }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
}
