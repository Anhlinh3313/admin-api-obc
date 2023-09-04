using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Status;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EnumData = Core.Business.ViewModels.EnumData;

namespace Core.Business.Services
{
    public class StatusService : BaseService, IStatusService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public StatusService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }


        public async Task<JsonResult> GetAllStatusTransactionAsync()
        {
            try
            {
                var status = await _unitOfWork.RepositoryR<StatusTransaction>()
                    .FindBy(x => x.IsEnabled == true).ToListAsync();
                var mapping = Mapper.Map<List<StatusViewModel>>(status);
                mapping.Add(new StatusViewModel()
                {
                    Id = 0,
                    IsEnabled = true,
                    Name = "Tất cả",
                    Code = "Tất cả"
                });
                var result = mapping.OrderBy(x => x.Id).ToList();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetStatusCustomerInformationAsync()
        {
            try
            {
                var status = await _unitOfWork.RepositoryR<Status>()
                    .FindBy(x => x.Id >= (int)EnumData.CustomerStatusEnum.PendingChapter).ToListAsync();
                var mapping = Mapper.Map<List<StatusViewModel>>(status);
                mapping.Add(new StatusViewModel()
                {
                    Id = 0,
                    IsEnabled = true,
                    Name = "Tất cả",
                    Code = "Tất cả"
                });
                var result = mapping.OrderBy(x => x.Id).ToList();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
