using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services.Abstract
{
    public interface ISignalRHubService
    {
        string GetConnectionId();
        void RegisterUserToHub(string userId);
        JsonResult RemoveUserToHub(string userId);
        Task SendNotifications(string userIds, string notificationName, object obj);
        Task OnDisconnectedAsync(Exception exception);
        Task SendNotification(string message);
    }
}
