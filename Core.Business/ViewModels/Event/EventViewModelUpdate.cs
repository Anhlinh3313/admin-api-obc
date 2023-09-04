using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Core.Entity.Entities;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelUpdate
    {
        public EventViewModelUpdate()
        {
            
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public TimeEventModel[] TimeEvents { get; set; }
        public string Objects { get; set; }
        public bool Fee { get; set; }
        public string LinkInformation { get; set; }
        public string LinkInformationQrCodePath { get; set; }
        public string LinkCheckIn { get; set; }
        public string LinkCheckInQrCodePath { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string[] ImagePath { get; set; }
    }

}
