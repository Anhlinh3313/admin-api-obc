using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class MembershipAction : EntityBasic
    {
        public MembershipAction()
        {
        }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int ExpenseId { get; set; }
        public DateTime? ExtendDate { get; set; }
        public int CustomerId { get; set; }
    }
}
