using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Expense : EntitySimple
    {
        public Expense()
        {
        }
        public int Duration { get; set; }
        public string TimeType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
