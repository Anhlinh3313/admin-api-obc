using Core.Business.Services.Abstract;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class SignalRHubService : Hub, ISignalRHubService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IHubContext<SignalRHubService> _hubContext;

        public SignalRHubService(IConnectionManager connectionManager, IHubContext<SignalRHubService> hubContext)
        {
            _connectionManager = connectionManager;
            _hubContext = hubContext;
        }

        public string GetConnectionId()
        {
            base.OnConnectedAsync();
            var connectionId = Context.ConnectionId;
            return connectionId;
        }

        public void RegisterUserToHub(string userId)
        {
            var connectionId = GetConnectionId();
            _connectionManager.KeepConnection(userId, connectionId);
            base.OnConnectedAsync();
        }

        public JsonResult RemoveUserToHub(string userId)
        {
            var connectionId = _connectionManager.GetConnection(userId);
            foreach (var item in connectionId)
            {
                _connectionManager.RemoveConnection(userId);
            }
            return JsonUtil.Success();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            var connnectionId = Context.ConnectionId;
            _connectionManager.RemoveConnectionId(connnectionId);
        }

        public async Task SendNotifications(string userId, string notificationName, object obj)
        {
            var connectionIds = new List<string>();
            connectionIds = _connectionManager.GetConnection(userId);
            await _hubContext.Clients.Clients(connectionIds).SendAsync("AwaitReceiverNotification", notificationName, obj);
        }

        public async Task SendNotification(string message)
        {
            await _hubContext.Clients.All.SendAsync("AwaitNotificationMessage", new { Message = message });
        }
    }

    public class ObjectTest
    {
        public string Message { get; set; }
    }

}
