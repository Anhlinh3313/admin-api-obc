using System;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class EntitySimple : EntityBasic, IEntitySimple
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
