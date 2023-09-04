using System;
namespace Core.Entity.Abstract
{
    public interface IEntityBase
    {
        int Id { get; set; }
        bool IsEnabled { get; set; }
    }
}
