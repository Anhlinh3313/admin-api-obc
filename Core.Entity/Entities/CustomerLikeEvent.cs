using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class CustomerLikeEvent : EntityBasic
    {
        public CustomerLikeEvent()
        {
        }
        public int EventId { get; set; }
        public int CustomerId { get; set; }
        public bool IsLiked { get; set; }
    }
}
