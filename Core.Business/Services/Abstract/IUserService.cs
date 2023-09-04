using System.Threading.Tasks;
using Core.Business.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IUserService
    {
        JsonResult GetListUser(string keySearch, int pageNum, int pageSize);
        Task<JsonResult> GetDetailUserAsync(int id);
        Task<JsonResult> CreateUserAsync(UserViewModel model);
        Task<JsonResult> UpdateUserAsync(UserViewModel model);
        Task<JsonResult> DeEnabledUserAsync(int userId);
        Task<JsonResult> DeActiveUserAsync(int userId);
        Task<JsonResult> GetAllRole();
        JsonResult CheckUser(int userId);
    }
}
