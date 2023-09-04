using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Home;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class HomeService : BaseService, IHomeService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public HomeService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetTypeSearchHomeMobile(int currentUserId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentUserId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    List<HomeViewModel> result = new List<HomeViewModel>()
                    {
                        new HomeViewModel(){TypeId = 1, TypeName = "Chapter"},
                        new HomeViewModel(){TypeId = 2, TypeName = "Thành viên khác Chapter"}
                    };
                    return JsonUtil.Success(result);
                }
                else
                {
                    List<HomeViewModel> result = new List<HomeViewModel>()
                    {
                        new HomeViewModel(){TypeId = 1, TypeName = "Chapter"},
                        new HomeViewModel(){TypeId = 2, TypeName = "Other Members Chapter"}
                    };
                    return JsonUtil.Success(result);
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListSearchHomeMobile(string keySearch, string typeId, int customerId, int pageNum, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(keySearch) || string.IsNullOrWhiteSpace(keySearch))
                {
                    return JsonUtil.Success(new List<Proc_GetListSearchHomeMobile>(), "Success");
                }
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                int type = 0;
                if (string.IsNullOrEmpty(typeId) || string.IsNullOrWhiteSpace(typeId))
                {
                    type = 0;
                }
                else
                {
                    List<string> listTypeString = typeId.Split(',').ToList();
                    if (listTypeString.Count == 1) type = int.Parse(listTypeString[0]);
                }

                var data = _unitOfWork.Repository<Proc_GetListSearchHomeMobile>()
                    .ExecProcedure(Proc_GetListSearchHomeMobile.GetEntityProc(keySearch, type, customerId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
