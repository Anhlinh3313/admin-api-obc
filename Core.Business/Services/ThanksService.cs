using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Thanks;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class ThanksService : BaseService, IThanksService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public INotifyService _notifyService { get; set; }
        public ThanksService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            INotifyService notifyService,
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _notifyService = notifyService;
        }

        public JsonResult GetListThanks(string keySearch, DateTime fromDate, DateTime toDate, string type, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListThanks>()
                    .ExecProcedure(Proc_GetListThanks.GetEntityProc(keySearch, fromDate, toDate, type, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateThanks(ThanksViewModelCreate model, int customerId)
        {
            try
            {
                Thanks thank = new Thanks()
                {
                    Note = model.Note,
                    ReceiverId = model.ReceiverId,
                    Type = model.Type,
                    CustomerId = customerId,
                    Value = model.Value
                };

                _unitOfWork.RepositoryCRUD<Thanks>().Insert(thank);
                await _unitOfWork.CommitAsync();

                string languageReceiver = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == model.ReceiverId).Language;
                if (string.IsNullOrEmpty(languageReceiver) || languageReceiver.Equals("vi"))
                {
                    var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                    var notify = _notifyService.CreateNotify(model.ReceiverId,
                        string.Format(ValidatorMessage.ContentNotify.ThanksFor, customerName),
                        (int)EnumData.NotifyType.Thanks, thank.Id, null, null);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }
                else
                {
                    var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                    var notify = _notifyService.CreateNotify(model.ReceiverId,
                        string.Format(ValidatorMessage.ContentNotify.ThanksForEnglish, customerName),
                        (int)EnumData.NotifyType.Thanks, thank.Id, null, null);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    return JsonUtil.Success(new
                    {
                        ReceiverName = model.ReceiverName,
                        Value = model.Value,
                        Type = model.Type,
                        Note = model.Note,
                        CreatedWhen = thank.CreatedWhen
                    });
                }
                else
                {
                    string typeThanks = "";
                    if (thank.Type.Equals("Nội bộ"))
                    {
                        typeThanks = "Internal";
                    }
                    else
                    {
                        typeThanks = "External Chapter";
                    }
                    return JsonUtil.Success(new
                    {
                        ReceiverName = model.ReceiverName,
                        Value = model.Value,
                        Type = typeThanks,
                        Note = model.Note,
                        CreatedWhen = thank.CreatedWhen
                    });
                }


            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetThanksReceiver(int thanksId, int customerId)
        {
            try
            {
                var thank = _unitOfWork.RepositoryR<Thanks>().GetSingle(x => x.Id == thanksId);
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == thank.CustomerId);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    return JsonUtil.Success(new
                    {
                        GiverId = customer.Id,
                        GiverName = customer.FullName,
                        Value = thank.Value,
                        Type = thank.Type,
                        Note = thank.Note,
                        CreatedWhen = thank.CreatedWhen,
                        ActionTypeId = 6
                    });
                }
                else
                {
                    string typeThanks = "";
                    if (thank.Type.Equals("Nội bộ"))
                    {
                        typeThanks = "Internal";
                    }
                    else
                    {
                        typeThanks = "External Chapter";
                    }
                    return JsonUtil.Success(new
                    {
                        GiverId = customer.Id,
                        GiverName = customer.FullName,
                        Value = thank.Value,
                        Type = typeThanks,
                        Note = thank.Note,
                        CreatedWhen = thank.CreatedWhen,
                        ActionTypeId = 6
                    });
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetThanksGive(int thanksId, int customerId)
        {
            try
            {
                var thank = _unitOfWork.RepositoryR<Thanks>().GetSingle(x => x.Id == thanksId);
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == thank.ReceiverId);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    return JsonUtil.Success(new
                    {
                        ReceiverId = customer.Id,
                        ReceiverName = customer.FullName,
                        Value = thank.Value,
                        Type = thank.Type,
                        Note = thank.Note,
                        CreatedWhen = thank.CreatedWhen,
                        ActionTypeId = 5
                    });
                }
                else
                {
                    string typeThanks = "";
                    if (thank.Type.Equals("Nội bộ"))
                    {
                        typeThanks = "Internal";
                    }
                    else
                    {
                        typeThanks = "External Chapter";
                    }
                    return JsonUtil.Success(new
                    {
                        ReceiverId = customer.Id,
                        ReceiverName = customer.FullName,
                        Value = thank.Value,
                        Type = typeThanks,
                        Note = thank.Note,
                        CreatedWhen = thank.CreatedWhen,
                        ActionTypeId = 5
                    });
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
