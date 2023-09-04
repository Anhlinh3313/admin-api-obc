using System;
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

namespace Core.Business.Services
{
    public class ClassificationsService : BaseService, IClassificationsService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ClassificationsService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetClassifications()
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetClassifications>()
                    .ExecProcedure(Proc_GetClassifications.GetEntityProc()).ToList();

                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetClassificationsNotInChapter(int chapterId)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetClassificationsNotInChapter>()
                    .ExecProcedure(Proc_GetClassificationsNotInChapter.GetEntityProc(chapterId)).ToList();

                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
