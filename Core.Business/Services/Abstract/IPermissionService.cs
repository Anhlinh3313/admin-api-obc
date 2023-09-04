using System;
namespace Core.Business.Services.Abstract
{
    public interface IPermissionService
    {
		bool CheckUserPermission(int userId, string aliasPath);
        //dynamic GetPermissionByRoleId(int roleId);
    }
}
