using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.MembershipAction;
using Core.Business.ViewModels.MembershipDuesReport;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class MembershipDuesReportService : BaseService, IMembershipDuesReportService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public MembershipDuesReportService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetMembershipDuesReport(int chapterId, DateTime fromDate)
        {
            try
            {
                var getMembershipDuesReportAllMember = _unitOfWork.Repository<Proc_GetMembershipDuesReportAllMember>()
                    .ExecProcedure(Proc_GetMembershipDuesReportAllMember.GetEntityProc(chapterId)).ToList();
                var getMembershipDuesReportAllMemberExpired = _unitOfWork.Repository<Proc_GetMembershipDuesReportAllMemberExpired>()
                    .ExecProcedure(Proc_GetMembershipDuesReportAllMemberExpired.GetEntityProc(chapterId, fromDate)).ToList();
                var getMembershipDuesReportAllMemberLate = _unitOfWork.Repository<Proc_GetMembershipDuesReportAllMemberLate>()
                    .ExecProcedure(Proc_GetMembershipDuesReportAllMemberLate.GetEntityProc(chapterId, fromDate)).ToList();
                var getMembershipDuesReportAllNewMember = _unitOfWork.Repository<Proc_GetMembershipDuesReportAllNewMember>()
                    .ExecProcedure(Proc_GetMembershipDuesReportAllNewMember.GetEntityProc(chapterId, fromDate)).ToList();

                var resultMembershipDuesReportAllMemberExpired = new List<Proc_GetMembershipDuesReportAllMemberExpired>();
                var resultAllMemberExpired = new List<Proc_GetMembershipDuesReportAllMemberExpired>();
                foreach (var item in getMembershipDuesReportAllMemberExpired)
                {
                    int tmp = 0;
                    foreach (var itemNewMember in getMembershipDuesReportAllNewMember)
                    {
                        if (item.Id == itemNewMember.Id) tmp++;
                    }
                    if(tmp == 0) resultMembershipDuesReportAllMemberExpired.Add(item);
                }

                if (resultMembershipDuesReportAllMemberExpired.Count != 0)
                {
                    resultAllMemberExpired.Add(resultMembershipDuesReportAllMemberExpired[0]);
                    for (int i = 1; i < resultMembershipDuesReportAllMemberExpired.Count; i++)
                    {
                        int tmp = 0;
                        for (int j = 0; j < i; j++)
                        {
                            if (resultMembershipDuesReportAllMemberExpired[i].Id == resultMembershipDuesReportAllMemberExpired[j].Id) tmp++;
                        }
                        if (tmp == 0) resultAllMemberExpired.Add(resultMembershipDuesReportAllMemberExpired[i]);
                    }
                }

                MembershipDuesReportViewModel result = new MembershipDuesReportViewModel()
                {
                    AllMember = getMembershipDuesReportAllMember,
                    AllMemberExpired = resultAllMemberExpired,
                    AllMemberLate = getMembershipDuesReportAllMemberLate,
                    AllNewMember = getMembershipDuesReportAllNewMember
                };
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

    }
}
