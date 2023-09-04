using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class CustomerShareEvent : IEntityBase
    {
        public CustomerShareEvent()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public int EventId { get; set; }
        public int CustomerId { get; set; }
        public bool IsShared { get; set; }
    }
}
