using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Core.Entity.Entities;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModel : IEntityBase
    {
        public EventViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public TimeEventMobile[] TimeEvents { get; set; }
        public string Objects { get; set; }
        public int NumberOfAttendees { get; set; }
        public bool Fee { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnd { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string ImagePath { get; set; }
    }
}
