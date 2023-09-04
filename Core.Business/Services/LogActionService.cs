using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.LogAction;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class LogActionService : BaseService, ILogActionService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public LogActionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> GetListLogAction(int customerId)
        {
            try
            {
                var log = await _unitOfWork.RepositoryR<LogAction>().FindBy(x => x.CustomerId == customerId && x.ChapterId == null).ToListAsync();
                var result = new List<LogActionViewModel>();
                foreach (var logAction in log)
                {
                    var user = await _unitOfWork.RepositoryR<User>().GetSingleNotEnabledAsync(x => x.Id == logAction.UserId);
                    var item = new LogActionViewModel()
                    {
                        Id = logAction.Id,
                        IsEnabled = logAction.IsEnabled,
                        Description = logAction.Description,
                        UserName = user.UserName,
                        CreatedWhen = logAction.CreatedWhen.GetValueOrDefault(),
                        Note = logAction.Note,
                        ActionName = logAction.ActionName
                    };
                    result.Add(item);
                }
                
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateLogAction(int userId, string action, string description, string note, int customerId)
        {
            try
            {
                if ((string.IsNullOrEmpty(action) || string.IsNullOrWhiteSpace(action)) &&
                    (string.IsNullOrEmpty(note) || string.IsNullOrWhiteSpace(note)))
                    return JsonUtil.Error("Vui lòng nhập ghi chú hoặc nhấn huỷ");
                if ((!string.IsNullOrEmpty(note) || !string.IsNullOrWhiteSpace(note)))
                    note = note.Trim();
                var logAction = new LogAction()
                {
                    ActionName = action,
                    Note = note,
                    UserId = userId,
                    Description = description,
                    CustomerId = customerId
                };
                _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(logAction);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
