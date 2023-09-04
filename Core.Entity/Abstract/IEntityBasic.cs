using System;
using Core.Entity.Abstract;

namespace Core.Entity.Abstract
{
    public interface IEntityBasic : IEntityGeneral
    {
        DateTime? CreatedWhen { get; set; }
		int? CreatedBy { get; set; }
		DateTime? ModifiedWhen { get; set; }
		int? ModifiedBy { get; set; }
    }
}
