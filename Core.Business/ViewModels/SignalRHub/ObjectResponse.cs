using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels.SignalRHub
{
    public class ObjectResponse
    {
        public string Message { get; set; }
        //public List<string> ConnectionIds { get; set; }
        public List<string> UserIds { get; set; }
    }
}