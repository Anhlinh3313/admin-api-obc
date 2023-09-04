using System;
namespace Core.Entity.Abstract
{
    public interface IEntitySimple : IEntityGeneral
    {
		string Code { get; set; }
		string Name { get; set; }
    }
}
