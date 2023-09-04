using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Thanks : EntityBasic
    {
        public Thanks()
        {
        }
        public int CustomerId { get; set; }
        public int ReceiverId { get; set; }
        public string Type { get; set; }
        public long Value { get; set; }
        public string Note { get; set; }
    }
}
