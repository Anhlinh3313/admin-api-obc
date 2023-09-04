using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Entity.Procedures;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class PermissionService : BaseService, IPermissionService
    {
        public PermissionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, IOptions<AppSettings> optionsAccessor, 
                                 IUnitOfWork unitOfWork) : base(logger, optionsAccessor, unitOfWork)
        {
        }

        public bool CheckUserPermission(int userId, string aliasPath)
		{
            //Console.WriteLine($"CheckUserPermission: userId: {userId}, aliasPath: {aliasPath}");
            var data = _unitOfWork.Repository<Proc_Permission>().ExecProcedureSingle(Proc_Permission.GetEntityProc(userId, aliasPath));

			if (data != null)
			{
				return data.Result;
			}
            return false;
		}

  //      public dynamic GetPermissionByRoleId(int roleId)
  //      {
  //          return JsonUtil.Success(_unitOfWork.RepositoryR<RolePage>().GetAll(x => x.RoleId == roleId));
  //      }

  //      public dynamic UpdatePermission(RolePermissionViewModel viewModel)
		//{
		//	if (viewModel.PagePermissions != null)
		//	{
  //              int[] pageIdsAll = viewModel.PagePermissions.Select(x => x.PageId).ToArray();
  //              List<RolePage> rolePagesExist = _unitOfWork.RepositoryR<RolePage>().FindBy(x => x.RoleId.Equals(viewModel.RoleId) && pageIdsAll.Contains(x.PageId)).ToList();
		//		int[] pageIdsExist = rolePagesExist.Select(x => x.PageId).ToArray();

  //              foreach (var item in viewModel.PagePermissions)
		//		{
		//			//Create new when not found exist Page Id
		//			if (!pageIdsExist.Contains(item.PageId))
		//			{
		//				RolePage rolePage = new RolePage();
		//				rolePage.PageId = item.PageId;
  //                      rolePage.RoleId = viewModel.RoleId;
  //                      rolePage.IsAccess = item.IsAccess;
  //                      rolePage.IsAdd = item.IsAdd;
  //                      rolePage.IsEdit = item.IsEdit;
  //                      rolePage.IsDelete = item.IsDelete;
  //                      _unitOfWork.RepositoryCRUD<RolePage>().Insert(rolePage);
		//			}
		//			else
		//			{
		//				RolePage rolePage = rolePagesExist.Single(r => r.PageId == item.PageId);
		//				rolePage.IsAccess = item.IsAccess;
		//				rolePage.IsAdd = item.IsAdd;
		//				rolePage.IsEdit = item.IsEdit;
		//				rolePage.IsDelete = item.IsDelete;
		//				_unitOfWork.RepositoryCRUD<RolePage>().Update(rolePage);
		//			}
		//		}
		//	}

  //          return JsonUtil.Success();
		//}
    }
}
