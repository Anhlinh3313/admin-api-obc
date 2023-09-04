using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Opportunity : EntityBasic
    {
        public Opportunity()
        {
        }
        public int CustomerId { get; set; }
        public int ReceiverId { get; set; }
        public string Type { get; set; }
        public string Information { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int Level { get; set; }
        public int StatusOpportunityId { get; set; }
        public string StatusComment { get; set; }
    }
}
