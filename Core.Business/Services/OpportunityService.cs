using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Role;
using Core.Data.Abstract;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.ViewModels.Opportunity;
using Core.Entity.Entities;
using EnumData = Core.Business.ViewModels.EnumData;

namespace Core.Business.Services
{
    public class OpportunityService : BaseService, IOpportunityService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public INotifyService _notifyService { get; set; }
        public OpportunityService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            INotifyService notifyService,
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _notifyService = notifyService;
        }

        public JsonResult GetListOpportunity(string keySearch, DateTime fromDate, DateTime toDate, string type, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListOpportunity>()
                    .ExecProcedure(Proc_GetListOpportunity.GetEntityProc(keySearch, fromDate, toDate, type, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateOpportunity(OpportunityViewModelCreate model, int customerId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrWhiteSpace(model.FullName))
                        return JsonUtil.Error(ValidatorMessage.Customer.FullNameNotEmpty);
                    if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                        return JsonUtil.Error(ValidatorMessage.Customer.PhoneNumberNotEmpty);
                    if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                        return JsonUtil.Error(ValidatorMessage.Customer.EmailNotEmpty);
                    if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                        return JsonUtil.Error(ValidatorMessage.Customer.AddressNotEmpty);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrWhiteSpace(model.FullName))
                        return JsonUtil.Error(ValidatorMessage.Customer.FullNameNotEmptyEnglish);
                    if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                        return JsonUtil.Error(ValidatorMessage.Customer.PhoneNumberNotEmptyEnglish);
                    if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                        return JsonUtil.Error(ValidatorMessage.Customer.EmailNotEmptyEnglish);
                    if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                        return JsonUtil.Error(ValidatorMessage.Customer.AddressNotEmptyEnglish);
                }

                model.FullName = model.FullName.Trim();
                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.Address = model.Address.Trim();

                Opportunity opportunity = new Opportunity();
                opportunity.Address = model.Address;
                opportunity.CustomerId = customerId;
                opportunity.Email = model.Email;
                opportunity.Information = model.Information;
                opportunity.Level = model.Level;
                opportunity.Name = model.FullName;
                opportunity.Note = model.Note;
                opportunity.PhoneNumber = model.PhoneNumber;
                opportunity.ReceiverId = model.ReceiverId;
                opportunity.Type = model.Type;
                opportunity.StatusOpportunityId = (int) EnumData.StatusOpportunity.NotContact;

                _unitOfWork.RepositoryCRUD<Opportunity>().Insert(opportunity);
                await _unitOfWork.CommitAsync();

                string languageReceiver = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == model.ReceiverId).Language;
                if (languageReceiver != null)
                {
                    if (languageReceiver.Equals("vi"))
                    {
                        var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                        var notify = _notifyService.CreateNotify(model.ReceiverId,
                            string.Format(ValidatorMessage.ContentNotify.OpportunityFor, customerName),
                            (int)EnumData.NotifyType.Opportunity, opportunity.Id, null, null);
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
                            string.Format(ValidatorMessage.ContentNotify.OpportunityForEnglish, customerName),
                            (int)EnumData.NotifyType.Opportunity, opportunity.Id, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }
                }
                else
                {
                    var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                    var notify = _notifyService.CreateNotify(model.ReceiverId,
                        string.Format(ValidatorMessage.ContentNotify.OpportunityFor, customerName),
                        (int)EnumData.NotifyType.Opportunity, opportunity.Id, null, null);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }


                var data = _unitOfWork.Repository<Proc_ReviewCreateOpportunity>()
                    .ExecProcedure(Proc_ReviewCreateOpportunity.GetEntityProc(opportunity.Id, customerId)).FirstOrDefault();

                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult ChangeStatusOpportunity(int id, int statusId, string note, int customerId)
        {
            try
            {
                var opportunity = _unitOfWork.RepositoryR<Opportunity>().GetSingle(x => x.Id == id);
                opportunity.StatusOpportunityId = statusId;
                opportunity.StatusComment = note;

                _unitOfWork.RepositoryCRUD<Opportunity>().Update(opportunity);
                _unitOfWork.Commit();

                var data = GetOpportunityReceiver(customerId, id);

                return data;
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetOpportunityReceiver(int customerId, int opportunityId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    var data = _unitOfWork.Repository<Proc_GetOpportunityReceiver>()
                        .ExecProcedure(Proc_GetOpportunityReceiver.GetEntityProc(opportunityId)).FirstOrDefault();

                    return JsonUtil.Success(data);
                }
                else
                {
                    var data = _unitOfWork.Repository<Proc_GetOpportunityReceiver>()
                        .ExecProcedure(Proc_GetOpportunityReceiver.GetEntityProc(opportunityId)).FirstOrDefault();
                    var result = new Proc_GetOpportunityReceiver()
                    {
                        GiverId = data.GiverId,
                        GiverFullName = data.GiverFullName,
                        Gender = data.Gender,
                        FullName = data.FullName,
                        Birthday = data.Birthday,
                        PhoneNumber = data.PhoneNumber,
                        Email = data.Email,
                        Address = data.Address,
                        Note = data.Note,
                        StatusName = data.StatusCode,
                        Level = data.Level,
                        OpportunityId = data.OpportunityId,
                        ActionTypeId = data.ActionTypeId
                    };

                    if (data.Type.Equals("Nội bộ"))
                    {
                        result.Type = "Internal";
                    }
                    else
                    {
                        result.Type = "External Chapter";
                    }
                    return JsonUtil.Success(result);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetOpportunityGive(int customerId, int opportunityId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    var data = _unitOfWork.Repository<Proc_GetOpportunityGive>()
                        .ExecProcedure(Proc_GetOpportunityGive.GetEntityProc(opportunityId)).FirstOrDefault();

                    return JsonUtil.Success(data);
                }
                else
                {
                    var data = _unitOfWork.Repository<Proc_GetOpportunityGive>()
                        .ExecProcedure(Proc_GetOpportunityGive.GetEntityProc(opportunityId)).FirstOrDefault();
                    var result = new Proc_GetOpportunityGive()
                    {
                        ReceiverId = data.ReceiverId,
                        ReceiverFullName = data.ReceiverFullName,
                        Gender = data.Gender,
                        FullName = data.FullName,
                        Birthday = data.Birthday,
                        PhoneNumber = data.PhoneNumber,
                        Email = data.Email,
                        Address = data.Address,
                        Note = data.Note,
                        StatusName = data.StatusCode,
                        Level = data.Level,
                        ActionTypeId = data.ActionTypeId
                    };

                    if (data.Type.Equals("Nội bộ"))
                    {
                        result.Type = "Internal";
                    }
                    else
                    {
                        result.Type = "External Chapter";
                    }
                    return JsonUtil.Success(result);
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetStatusOpportunity(int customerId)
        {
            try
            {
                var status = _unitOfWork.RepositoryR<StatusOpportunity>().GetAll().ToList();
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    List<StatusOpportunityModel> result = new List<StatusOpportunityModel>();
                    foreach (var item in status)
                    {
                        StatusOpportunityModel itemResult = new StatusOpportunityModel()
                        {
                            Id = item.Id,
                            Name = item.Name
                        };
                        result.Add(itemResult);
                    }
                    return JsonUtil.Success(result);
                }
                else
                {
                    List<StatusOpportunityModel> result = new List<StatusOpportunityModel>();
                    foreach (var item in status)
                    {
                        StatusOpportunityModel itemResult = new StatusOpportunityModel()
                        {
                            Id = item.Id,
                            Name = item.Code
                        };
                        result.Add(itemResult);
                    }
                    return JsonUtil.Success(result);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
