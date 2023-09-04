using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels
{
    public class CheckHourViewModel
    {
        public CheckHourViewModel() { }
        public int? HourLogin { get; set; }
        public int? UserId { get; set; }
        public int? MinuteLogin { get; set; }

    }
}
