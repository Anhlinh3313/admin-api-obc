using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Page;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class PageService : BaseService, IPageService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public PageService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetListPage(int userId)
        {
            try
            {
                var page = _unitOfWork.RepositoryR<Page>().FindBy(x => x.ParentId == null && x.Environment.Equals("Web")).OrderBy(x => x.PageOrder).ToList();
                var roleUser = _unitOfWork.RepositoryR<User>().GetSingle(x => x.Id == userId);
                var result = new List<PageViewModel>();
                foreach (var item in page)
                {
                    var itemResult = new PageViewModel();
                    if (item.Type.ToLower().Equals("collapsable"))
                    {
                        itemResult = new PageViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Code = item.Code,
                            Type = item.Type,
                            Icon = item.Icon,
                            Url = item.Url
                        };
                        var pageChildren = _unitOfWork.RepositoryR<Page>().FindBy(x => x.ParentId == item.Id).OrderBy(x => x.PageOrder).ToList();
                        var children = new List<PageViewModel>();
                        foreach (var itemPageChildren in pageChildren)
                        {
                            var checkPermission = _unitOfWork.RepositoryR<RolePage>().GetSingle(x =>
                                x.PageId == itemPageChildren.Id && x.RoleId == roleUser.RoleId);
                            if (checkPermission != null)
                            {
                                if (checkPermission.IsView == true)
                                {
                                    var itemChildren = new PageViewModel()
                                    {
                                        Id = itemPageChildren.Id,
                                        Name = itemPageChildren.Name,
                                        Code = itemPageChildren.Code,
                                        Type = itemPageChildren.Type,
                                        Icon = itemPageChildren.Icon,
                                        Url = itemPageChildren.Url
                                    };
                                    children.Add(itemChildren);
                                }
                            }
                        }

                        itemResult.Children = children.ToArray();
                        if (itemResult.Children.Length != 0)
                        {
                            result.Add(itemResult);
                        }
                    }
                    else
                    {
                        var checkPermission = _unitOfWork.RepositoryR<RolePage>().GetSingle(x =>
                            x.PageId == item.Id && x.RoleId == roleUser.RoleId);
                        if (checkPermission != null)
                        {
                            if (checkPermission.IsView == true)
                            {
                                itemResult = new PageViewModel()
                                {
                                    Id = item.Id,
                                    Name = item.Name,
                                    Code = item.Code,
                                    Type = item.Type,
                                    Icon = item.Icon,
                                    Url = item.Url
                                };
                                result.Add(itemResult);
                            }
                        }
                    }

                }

                return JsonUtil.Success(result.ToArray());
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckPermission(string pathName, int userId)
        {
            try
            {
                if (pathName.Equals("/")) pathName = "/user";
                string[] arrPathName = pathName.Split('/');
                string pathSearch = "/" + arrPathName[1];
                var roleUser = _unitOfWork.RepositoryR<User>().GetSingle(x => x.Id == userId);
                var page = _unitOfWork.RepositoryR<Page>().GetSingle(x => x.PathName.Equals(pathSearch));
                var checkPermission = _unitOfWork.RepositoryR<RolePage>().GetSingle(x =>
                    x.PageId == page.Id && x.RoleId == roleUser.RoleId);

                return JsonUtil.Success(checkPermission);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckPermissionMobile(string pageName, int customerId)
        {
            try
            {
                var page = _unitOfWork.RepositoryR<Page>().GetSingle(x => x.Name.Equals(pageName));
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (customer.RoleId == null) return JsonUtil.Error("Bạn không có quyền để sử dụng tính năng này");
                var rolePage = _unitOfWork.RepositoryR<RolePage>().GetSingle(x => x.PageId == page.Id && x.RoleId == customer.RoleId);

                return JsonUtil.Success(rolePage);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
