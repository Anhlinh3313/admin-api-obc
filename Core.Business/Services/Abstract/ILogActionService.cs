using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface ILogActionService
    {
        Task<JsonResult> GetListLogAction(int customerId);
        Task<JsonResult> CreateLogAction(int userId, string action, string description, string note, int customerId);

    }
}
