using Core.Business.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.Services
{
    public class ConnectionManager : IConnectionManager

    {
        private static Dictionary<string, List<string>> connections = new Dictionary<string, List<string>>();
        private static string connectionLock = string.Empty;

        public void KeepConnection(string userId, string connectionId)
        {
            lock (connectionLock)
            {
                if (!connections.ContainsKey(userId))
                {
                    connections[userId] = new List<string>();
                }
                connections[userId].Add(connectionId);
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (connectionLock)
            {
                foreach (var userId in connections.Keys)
                {
                    if (connections[userId].Contains(userId))
                    {
                        connections[userId].Remove(userId);
                        break;
                    }
                }
            }
        }

        public void RemoveConnectionId(string connectionId)
        {
            lock (connectionLock)
            {
                foreach (var connectionIds in connections.Values)
                {
                    connectionIds.Remove(connectionId);
                }
            }
        }

        public List<string> GetConnection(string userId)
        {
            var con = new List<string>();
            userId = "\"" + userId + "\"";
            lock (connectionLock)
            {
                if (connections.ContainsKey(userId))
                {
                    con = connections[userId];
                }
            }
            return con;
        }
    }
}