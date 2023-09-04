using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Notify : EntityBasic
    {
        public Notify()
        {
        }
        public int CustomerId { get; set; }
        public bool IsSeen { get; set; }
        public int NotifyTypeId { get; set; }
        public int ActionTypeId { get; set; }
        public int? CustomerCancelId { get; set; }
        public string ReasonCancel { get; set; }
    }
}
