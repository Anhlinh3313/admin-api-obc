using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class CustomerEvent : EntityBasic
    {
        public CustomerEvent()
        {
        }
        public int CustomerId { get; set; }
        public int EventId { get; set; }
        public bool Status { get; set; }
        public bool CheckIn { get; set; }
        public string Note { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
