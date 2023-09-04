using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Core.Entity.Procedures;

namespace Core.Business.ViewModels.Customer
{
    public class CustomerViewModelViewIndicators
    {
        public CustomerViewModelViewIndicators()
        {
            
        }
        public List<Proc_GetIndicators> Indicators { get; set; }
        public List<EventModel> Event { get; set; }
        public List<EventModel> MeetingChapter { get; set; }
        public List<Education> Education { get; set; }
    }

    public class EventModel
    {
        public EventModel()
        {
            
        }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime CreatedWhen { get; set; }
    }

    public class Education
    {
        public Education()
        {
            
        }
        public int EducationId { get; set; }
        public string EducationName { get; set; }
        public DateTime CreatedWhen { get; set; }
    }
}
