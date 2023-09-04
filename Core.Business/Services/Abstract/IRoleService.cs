using System.Threading.Tasks;
using Core.Business.ViewModels.Role;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IRoleService
    {
        JsonResult GetListRoleAsync(string keySearch, int pageNum, int pageSize);
        Task<JsonResult> GetRoleDetailAsync(int id);
        Task<JsonResult> CreateRoleAsync(RoleViewModelCreate model);
        Task<JsonResult> UpdateRoleAsync(RoleViewModelCreate model);
        Task<JsonResult> DeEnabledAsync(int id);
        Task<JsonResult> GetAllRoleType();
        JsonResult GetRoleWithModuleName(string moduleName);
    }
}
