using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class LogAction : EntityBasic
    {
        public LogAction()
        {
        }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int? UserId { get; set; }
        public int CustomerId { get; set; }
        public int? ChapterId { get; set; }
        public int? MembershipActionId { get; set; }
    }
}
