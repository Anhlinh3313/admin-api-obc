using System;

namespace Core.Data.Abstract
{
    public interface IEntityBaseRepository<T> : IDisposable where T : class, new() {
    }
}
