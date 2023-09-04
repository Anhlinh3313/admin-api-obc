using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Event : EntitySimple
    {
        public Event()
        {
        }
        public string Objects { get; set; }
        public int NumberOfAttendees { get; set; }
        public bool Fee { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnd { get; set; }
        public string LinkInformation { get; set; }
        public string LinkInformationQrCodePath { get; set; }
        public string LinkCheckIn { get; set; }
        public string LinkCheckInQrCodePath { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
    }
}
