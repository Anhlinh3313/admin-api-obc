using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Core.Business.ViewModels.Role;
using Core.Business.ViewModels.RolePage;

namespace Core.Business.Services
{
    public class RoleService : BaseService, IRoleService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public RoleService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetListRoleAsync(string keySearch, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListRole>()
                    .ExecProcedure(Proc_GetListRole.GetEntityProc(keySearch,pageNum,pageSize)).ToList();
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetRoleDetailAsync(int id)
        {
            try
            {
                var role = await _unitOfWork.RepositoryR<Role>().GetSingleAsync(x => x.Id == id);
                return JsonUtil.Success(role);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateRoleAsync(RoleViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Role.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Role.NameNotEmpty);
                if (model.RoleTypeId == null || model.RoleTypeId == 0)
                    return JsonUtil.Error(ValidatorMessage.Role.RoleTypeNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                if (_unitOfWork.RepositoryR<Role>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Role.UniqueCode);
                if (_unitOfWork.RepositoryR<Role>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Role.UniqueName);

                //var role = await _iGeneralRawService.Create<Role, RoleViewModelCreate>(model);
                Role role = new Role()
                {
                    Code = model.Code,
                    Name = model.Name,
                    RoleTypeId = model.RoleTypeId
                };

                _unitOfWork.RepositoryCRUD<Role>().Insert(role);
                await _unitOfWork.CommitAsync();

                List<Page> pageParent = new List<Page>();
                if (model.RoleTypeId == 1) // web
                {

                    pageParent = _unitOfWork.RepositoryR<Page>().FindBy(x => x.ParentId == null && x.Environment.Equals("Web")).OrderBy(x => x.PageOrder).ToList();

                }
                else //mobile
                {
                    pageParent = _unitOfWork.RepositoryR<Page>().FindBy(x => x.ParentId == null && x.Environment.Equals("Mobile")).OrderBy(x => x.PageOrder).ToList();

                }

                foreach (var itemPage in pageParent)
                {
                    var item = new RolePageViewModelView();
                    item.PageName = itemPage.Name;
                    var pageChildren = _unitOfWork.RepositoryR<Page>().FindBy(x => x.ParentId == itemPage.Id).OrderBy(x => x.PageOrder).ToList();
                    if (pageChildren.Count <= 0)
                    {
                        var rolePage = new RolePage()
                        {
                            IsEdit = false,
                            IsDelete = false,
                            IsView = false,
                            IsCreate = false,
                            PageId = itemPage.Id,
                            RoleId = role.Id
                        };
                        _unitOfWork.RepositoryCRUD<RolePage>().Insert(rolePage);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        foreach (var pageChild in pageChildren)
                        {
                            var rolePage = new RolePage()
                            {
                                IsEdit = false,
                                IsDelete = false,
                                IsView = false,
                                IsCreate = false,
                                PageId = pageChild.Id,
                                RoleId = role.Id
                            };
                            _unitOfWork.RepositoryCRUD<RolePage>().Insert(rolePage);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }

                return JsonUtil.Success(role);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateRoleAsync(RoleViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Role.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Role.NameNotEmpty);
                if (model.RoleTypeId == null || model.RoleTypeId == 0)
                    return JsonUtil.Error(ValidatorMessage.Role.RoleTypeNotEmpty);
                
                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                var roleOld = await _unitOfWork.RepositoryR<Role>().GetSingleAsync(x => x.Id == model.Id);
                if (!roleOld.Code.ToLower().Equals(model.Code.ToLower()))
                    if (_unitOfWork.RepositoryR<Role>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Role.UniqueCode);
                if (!roleOld.Name.ToLower().Equals(model.Name.ToLower()))
                    if (_unitOfWork.RepositoryR<Role>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Role.UniqueName);
                if (roleOld.RoleTypeId != model.RoleTypeId)
                {
                    var user = await _unitOfWork.RepositoryR<User>().AnyAsync(x => x.RoleId == model.Id);
                    if (user) return JsonUtil.Error(ValidatorMessage.Role.NotChange);
                    var customer = await _unitOfWork.RepositoryR<Customer>().AnyAsync(x => x.RoleId == model.Id);
                    if (customer) return JsonUtil.Error(ValidatorMessage.Role.NotChange);
                }

                model.IsEnabled = true;
                var role = await _iGeneralRawService.Update<Role, RoleViewModelCreate>(model);
                return JsonUtil.Success(role);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.RepositoryR<User>().AnyAsync(x => x.RoleId == id);
                if (user) return JsonUtil.Error(ValidatorMessage.Role.NotDestroy);
                var customer = await _unitOfWork.RepositoryR<Customer>().AnyAsync(x => x.RoleId == id);
                if (customer) return JsonUtil.Error(ValidatorMessage.Role.NotDestroy);
                var role = await _unitOfWork.RepositoryR<Role>().GetSingleAsync(x => x.Id == id);
                if (role == null) return JsonUtil.Error(ValidatorMessage.Role.NotExist);
                role.IsEnabled = !role.IsEnabled;

                var rolePage = _unitOfWork.RepositoryR<RolePage>().FindBy(x => x.RoleId == id).ToList();
                foreach (var item in rolePage)
                {
                    _unitOfWork.RepositoryCRUD<RolePage>().Delete(item);
                    await _unitOfWork.CommitAsync();
                }

                _unitOfWork.RepositoryCRUD<Role>().Update(role);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllRoleType()
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<RoleType>().FindBy(x => x.IsEnabled == true).ToListAsync();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetRoleWithModuleName(string moduleName)
        {
            try
            {
                var result = new List<Role>();
                if(moduleName.Trim().ToLower().Equals("web admin"))
                {
                    result = _unitOfWork.RepositoryR<Role>().FindBy(x => x.RoleTypeId == 1).ToList();
                }
                else
                {
                    result = _unitOfWork.RepositoryR<Role>().FindBy(x => x.RoleTypeId == 2).ToList();
                }

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
