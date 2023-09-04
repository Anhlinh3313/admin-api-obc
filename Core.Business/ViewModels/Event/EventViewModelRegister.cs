using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Event
{
    public class EventViewModelRegister
    {
        public EventViewModelRegister()
        {
            
        }
        public int CustomerEventId { get; set; }
        public int TransactionEventId { get; set; }
        public string ImagePath { get; set; }
    }
}
