using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels
{
    public class SeriNumberViewModel
    {
        public SeriNumberViewModel() { }
        public string SeriNumber { get; set; }
        public double Weight { get; set; }
        public int HubId { get; set; }
        public int UserId { get; set; }
        public string ConnectCode { get; set; }
        public string BodyPrint { get; set; }
        public List<int> ShipmentIds { get; set; }
        public string Token { get; set; }
    }
}
