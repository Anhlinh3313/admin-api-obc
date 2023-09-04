using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels.SignalRHub
{
    public class NotificationBooking
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
