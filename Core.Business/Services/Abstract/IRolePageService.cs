using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Core.Business.ViewModels.RolePage;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IRolePageService
    {
        Task<JsonResult> CreateOrUpdateRolePage(RolePageViewModelCreate model);
        //Task<JsonResult> CreateRolePage(RolePageViewModelCreate model);

        JsonResult GetRolePage(int roleId);
    }
}
