using System;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class EntityBasic : IEntityBasic
    {
		public virtual int Id { get; set; }
        public virtual DateTime? CreatedWhen { get; set; }
		public virtual int? CreatedBy { get; set; } 
		public virtual DateTime? ModifiedWhen { get; set; }
		public virtual int? ModifiedBy { get; set; }
		public virtual string ConcurrencyStamp { get; set; }
        public virtual bool IsEnabled { get; set; }
    }
}
