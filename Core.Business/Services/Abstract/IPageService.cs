using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IPageService
    {
        JsonResult GetListPage(int userId);
        JsonResult CheckPermission(string pathName, int userId);
        JsonResult CheckPermissionMobile(string pageName, int customerId);
    }
}
