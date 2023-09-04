using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.RolePage;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class RolePageService : BaseService, IRolePageService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public RolePageService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> CreateOrUpdateRolePage(RolePageViewModelCreate model)
        {
            try
            {
                foreach (var rolePage in model.RolePage)
                {
                    foreach (var rolePageViewModelViewItem in rolePage.RolePage)
                    {
                        var item = new RolePage()
                        {
                            Id = rolePageViewModelViewItem.RolePageId,
                            IsEdit = rolePageViewModelViewItem.IsEdit,
                            RoleId = rolePageViewModelViewItem.RoleId,
                            IsCreate = rolePageViewModelViewItem.IsCreate,
                            IsDelete = rolePageViewModelViewItem.IsDelete,
                            IsView = rolePageViewModelViewItem.IsView,
                            PageId = rolePageViewModelViewItem.PageId,
                            IsEnabled = true
                        };
                        if (rolePageViewModelViewItem.RolePageId == 0)
                        {
                            _unitOfWork.RepositoryCRUD<RolePage>().Insert(item);
                            await _unitOfWork.CommitAsync();
                        }
                        else
                        {
                            _unitOfWork.RepositoryCRUD<RolePage>().Update(item);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetRolePage(int roleId)
        {
            try
            {
                var result = new List<RolePageViewModelView>();
                var role = _unitOfWork.RepositoryR<Role>().GetSingle(x => x.Id == roleId);
                List<Page> pageParent = new List<Page>();
                if (role.RoleTypeId == 1) // web
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
                        var rolePage = _unitOfWork.RepositoryR<RolePage>().GetSingle(x => x.RoleId == roleId && x.PageId == itemPage.Id);
                        var listItemView = new List<RolePageViewModelViewItem>();
                        if (rolePage == null)
                        {
                            var itemView = new RolePageViewModelViewItem()
                            {
                                RoleId = roleId,
                                IsEdit = false,
                                IsDelete = false,
                                IsView = false,
                                IsCreate = false,
                                PageId = itemPage.Id,
                                PageName = itemPage.Name,
                                RolePageId = 0
                            };
                            listItemView.Add(itemView);
                            item.RolePage = listItemView.ToArray();
                        }
                        else
                        {
                            var itemView = new RolePageViewModelViewItem()
                            {
                                RoleId = rolePage.RoleId,
                                IsEdit = rolePage.IsEdit,
                                IsDelete = rolePage.IsDelete,
                                IsView = rolePage.IsView,
                                IsCreate = rolePage.IsCreate,
                                PageId = itemPage.Id,
                                PageName = itemPage.Name,
                                RolePageId = rolePage.Id
                            };
                            listItemView.Add(itemView);
                            item.RolePage = listItemView.ToArray();
                        }
                    }
                    else
                    {
                        var listItemView = new List<RolePageViewModelViewItem>();
                        foreach (var pageChild in pageChildren)
                        {
                            var rolePage = _unitOfWork.RepositoryR<RolePage>().GetSingle(x => x.RoleId == roleId && x.PageId == pageChild.Id);
                            if (rolePage == null)
                            {
                                var itemView = new RolePageViewModelViewItem()
                                {
                                    RoleId = roleId,
                                    IsEdit = false,
                                    IsDelete = false,
                                    IsView = false,
                                    IsCreate = false,
                                    PageId = pageChild.Id,
                                    PageName = pageChild.Name,
                                    RolePageId = 0
                                };
                                listItemView.Add(itemView);
                            }
                            else
                            {
                                var itemView = new RolePageViewModelViewItem()
                                {
                                    RoleId = rolePage.RoleId,
                                    IsEdit = rolePage.IsEdit,
                                    IsDelete = rolePage.IsDelete,
                                    IsView = rolePage.IsView,
                                    IsCreate = rolePage.IsCreate,
                                    PageId = pageChild.Id,
                                    PageName = pageChild.Name,
                                    RolePageId = rolePage.Id
                                };
                                listItemView.Add(itemView);
                            }
                        }
                        item.RolePage = listItemView.ToArray();
                    }

                    result.Add(item);
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
