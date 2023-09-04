using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class TermCondition : IEntityBase
    {
        public TermCondition()
        {
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Content { get; set; }
    }
}
