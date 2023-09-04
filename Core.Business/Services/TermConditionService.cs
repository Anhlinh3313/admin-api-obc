using System;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Core.Business.ViewModels.TermCondition;

namespace Core.Business.Services
{
    public class TermConditionService : BaseService, ITermConditionService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public TermConditionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> GetListTermConditionAsync()
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<TermCondition>().GetAll().ToListAsync();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateTermConditionAsync(TermConditionViewModel model)
        {
            try
            {
                var checkTermCondition = _unitOfWork.RepositoryR<TermCondition>().Any();
                if (!string.IsNullOrEmpty(model.Content) || !string.IsNullOrWhiteSpace(model.Content))
                    model.Content = model.Content.Trim();
                if (!checkTermCondition)
                {
                    TermCondition termCondition = Mapper.Map<TermCondition>(model);
                    termCondition.Id = 0;
                    _unitOfWork.RepositoryCRUD<TermCondition>().Insert(termCondition);
                    await _unitOfWork.CommitAsync();
                    return JsonUtil.Success(Mapper.Map<TermConditionViewModel>(termCondition));
                }
                else
                {
                    var termCondition = await _unitOfWork.RepositoryR<TermCondition>().GetSingleAsync(x => x.Id > 0);
                    model.Id = termCondition.Id;
                    model.IsEnabled = true;

                    termCondition = Mapper.Map(model, termCondition);
                    _unitOfWork.RepositoryCRUD<TermCondition>().Update(termCondition);
                    await _unitOfWork.CommitAsync();
                    return JsonUtil.Success(Mapper.Map<TermConditionViewModel>(model));
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
