using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class AssessCustomer : EntityBasic
    {
        public AssessCustomer()
        {
        }
        public int CustomerId { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }
    }
}
