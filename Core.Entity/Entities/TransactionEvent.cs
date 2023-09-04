using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class TransactionEvent : EntityBasic
    {
        public TransactionEvent()
        {
        }
        public int CustomerId { get; set; }
        public int? ChapterId { get; set; }
        public int EventId { get; set; }
        public string ImagePath { get; set; }
        public string Note { get; set; }
        public int StatusId { get; set; }
        public DateTime? DateActive { get; set; }
        public string Code { get; set; }
    }
}
