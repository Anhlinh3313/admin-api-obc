using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.Services.Abstract
{
    public interface IConnectionManager
    {
        void KeepConnection(string userId, string connectionId);
        void RemoveConnection(string userId);
        List<string> GetConnection(string userId);
        void RemoveConnectionId(string connectionId);
    }
}