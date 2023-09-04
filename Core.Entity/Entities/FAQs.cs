using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class FAQs : EntityBasic
    {
        public FAQs()
        {
        }
        public string Question { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
    }
}
