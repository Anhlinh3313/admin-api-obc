using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Assess : EntityBasic
    {
        public Assess()
        {
        }
        public int CustomerId { get; set; }
        public int CourseId { get; set; }
        public string Comment { get; set; }
        public int Value { get; set; }
    }
}
