using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelMobile
    {
        public EventViewModelMobile()
        {
            
        }
        public int EventId { get; set; }
        public int EventType { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public int RowNum { get; set; }
        public TimeEventMobile[] TimeEvents { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string QrInformation { get; set; }
        public string LinkCheckInQrCodePath { get; set; }
        public string Objects { get; set; }
        public string[] ImagePath { get; set; }
        public bool IsFee { get; set; }
        public bool Liked { get; set; }
        public int? SumLike { get; set; }
        public bool Shared { get; set; }
        public int? SumShare { get; set; }
        public int Total { get; set; }
    }

    public class TimeEventMobile
    {
        public TimeEventMobile()
        {
            
        }
        public string Date { get; set; }
        public string Time { get; set; }
    }

    public class EventViewModelRecent
    {
        public EventViewModelRecent()
        {

        }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public TimeEventMobile[] TimeEvents { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
    }
}
