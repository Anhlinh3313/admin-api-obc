using System;
namespace Core.Entity.Abstract
{
    public interface IEntityGeneral : IEntityBase
    {
        string ConcurrencyStamp { get; set; }
    }
}
