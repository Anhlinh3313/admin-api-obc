using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.ChapterMemberCompany;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class ChapterMemberCompanyService : BaseService, IChapterMemberCompanyService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ChapterMemberCompanyService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetChapterMemberCompanyWeb(int chapterId, DateTime? dateSearch)
        {
            try
            {
                if (dateSearch == null) return JsonUtil.Error("Ngày không được để trống");
                var data = _unitOfWork.Repository<Proc_GetChapterMemberCompany>()
                    .ExecProcedure(Proc_GetChapterMemberCompany.GetEntityProc(chapterId, dateSearch.GetValueOrDefault())).ToList();

                List<ChapterMemberCompanyViewModel> result = new List<ChapterMemberCompanyViewModel>();
                for (int i = 0; i < data.Count; i++)
                {
                    ChapterMemberCompanyViewModel itemResult = new ChapterMemberCompanyViewModel();
                    if (i > 0)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (data[j].CustomerId == data[i].CustomerId)
                            {
                                result[result.Count - 1].Note.Add(data[i].Note);
                                j = i;
                            }
                        }

                        if (data[i].CustomerId != data[i - 1].CustomerId)
                        {
                            itemResult = new ChapterMemberCompanyViewModel()
                            {
                                Id = data[i].Id,
                                CustomerId = data[i].CustomerId,
                                FullName = data[i].FullName,
                                BusinessName = data[i].BusinessName,
                                ProfessionName = data[i].ProfessionName,
                                Note = new List<string>()
                            };
                            itemResult.Note.Add(data[i].Note);
                            result.Add(itemResult);
                        }
                    }
                    else
                    {
                        itemResult = new ChapterMemberCompanyViewModel()
                        {
                            Id = data[i].Id,
                            CustomerId = data[i].CustomerId,
                            FullName = data[i].FullName,
                            BusinessName = data[i].BusinessName,
                            ProfessionName = data[i].ProfessionName,
                            Note = new List<string>()
                        };
                        itemResult.Note.Add(data[i].Note);
                        result.Add(itemResult);
                    }
                }
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetChapterMemberCompany(int chapterId, DateTime dateSearch)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetChapterMemberCompany>()
                    .ExecProcedure(Proc_GetChapterMemberCompany.GetEntityProc(chapterId, dateSearch)).ToList();

                List<Proc_GetChapterMemberCompany> result = new List<Proc_GetChapterMemberCompany>();
                for (int i = 0; i < data.Count; i++)
                {
                    Proc_GetChapterMemberCompany itemResult = new Proc_GetChapterMemberCompany();
                    if (i > 0)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (data[j].CustomerId == data[i].CustomerId)
                            {
                                result[result.Count - 1].Note = result[result.Count - 1].Note + " \r\n" + data[i].Note;
                                j = i;
                            }
                        }

                        if (i < data.Count - 1)
                        {
                            if (data[i].CustomerId != data[i + 1].CustomerId)
                            {
                                itemResult = data[i + 1];
                                result.Add(itemResult);
                            }
                        }
                    }
                    else
                    {
                        itemResult = data[i];
                        result.Add(itemResult);
                    }
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
