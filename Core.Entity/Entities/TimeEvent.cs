using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class TimeEvent : IEntityBase
    {
        public TimeEvent()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int EventId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
