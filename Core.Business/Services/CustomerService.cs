using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Customer;
using Core.Business.ViewModels.User;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EnumData = Core.Business.ViewModels.EnumData;

namespace Core.Business.Services
{
    public class CustomerService : BaseService, ICustomerService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IFileService _fileService;
        public INotifyService _notifyService;
        public IAccountService _accountService;
        private readonly SendMailOTP _iSendMailOTP;
        private readonly SendMail _iSendMail;


        public CustomerService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IFileService fileService,
            IAccountService accountService,
            INotifyService notifyService,
            IOptions<SendMail> sendMail,
            IOptions<SendMailOTP> sendMailOTP,
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _fileService = fileService;
            _notifyService = notifyService;
            _iSendMail = sendMail.Value;
            _iSendMailOTP = sendMailOTP.Value;
            _accountService = accountService;
        }

        public async Task<JsonResult> GetDetailProfileCustomerAsync(int id)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>()
                    .GetSingleAsync(x => x.Id == id);
                var statusName = _unitOfWork.RepositoryR<Status>().GetSingle(x => x.Id == customer.StatusId);
                var result = Mapper.Map<CustomerViewModel>(customer);
                result.StatusName = statusName.Name;
                if (customer.WardId.HasValue)
                    result.DistrictId = _unitOfWork.RepositoryR<Wards>().GetSingle(x => x.Id == customer.WardId).DistrictId;
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateCustomerProfileAsync(CustomerViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                    return JsonUtil.Error(ValidatorMessage.Customer.PhoneNumberNotEmpty);
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return JsonUtil.Error(ValidatorMessage.Customer.EmailNotEmpty);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrWhiteSpace(model.FullName))
                    return JsonUtil.Error(ValidatorMessage.Customer.FullNameNotEmpty);
                if (string.IsNullOrEmpty(model.IdentityCard) || string.IsNullOrWhiteSpace(model.IdentityCard))
                    return JsonUtil.Error(ValidatorMessage.Customer.IdCardNotEmpty);
                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Customer.AddressNotEmpty);

                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.FullName = model.FullName.Trim();
                model.IdentityCard = model.IdentityCard.Trim();

                var cus = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == model.Id);
                if (!cus.PhoneNumber.ToLower().Equals(model.PhoneNumber.ToLower()))
                    if (_unitOfWork.RepositoryR<Customer>().Any(x => x.PhoneNumber.ToLower().Equals(model.PhoneNumber.ToLower())))
                        return JsonUtil.Error(ValidatorMessage.Customer.UniquePhone);
                if (!cus.Email.ToLower().Equals(model.Email.ToLower()))
                    if (_unitOfWork.RepositoryR<Customer>().Any(x => x.Email.ToLower().Equals(model.Email.ToLower())))
                        return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmail);
                if ((!string.IsNullOrEmpty(cus.IdentityCard) || !string.IsNullOrWhiteSpace(cus.IdentityCard)) && !cus.IdentityCard.ToLower().Equals(model.IdentityCard.ToLower()))
                    if (_unitOfWork.RepositoryR<Customer>().Any(x => x.IdentityCard.ToLower().Equals(model.IdentityCard.ToLower())))
                        return JsonUtil.Error(ValidatorMessage.Customer.UniqueIdCard);

                cus.Address = model.Address;
                cus.ProvinceId = model.ProvinceId;
                cus.Email = model.Email;
                cus.FullName = model.FullName;
                cus.PhoneNumber = model.PhoneNumber;
                cus.IdentityCard = model.IdentityCard;
                cus.IdentityCardPlace = model.IdentityCardPlace.GetValueOrDefault();
                cus.Birthday = model.Birthday.GetValueOrDefault();
                cus.WardId = model.WardId;
                cus.IdentityCardDate = model.IdentityCardDate.GetValueOrDefault();
                cus.Description = model.Description;
                cus.AvatarPath = model.AvatarPath;
                _unitOfWork.RepositoryCRUD<Customer>().Update(cus);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(cus);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> AcceptPremium(int id, string note, int? active)
        {
            try
            {
                var transaction = await _unitOfWork.RepositoryR<Transaction>().GetSingleAsync(x => x.Id == id);
                if (transaction.StatusTransactionId == (int) EnumData.TransactionStatusEnum.Accepted)
                    return JsonUtil.Error("Giao dịch đã được kích hoạt trước đó, vui lòng làm mới lại trang");
                if (transaction.StatusTransactionId == (int)EnumData.TransactionStatusEnum.Cancel)
                    return JsonUtil.Error("Giao dịch đã được từ chối trước đó, vui lòng làm mới lại trang");
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == transaction.CustomerId);
                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingleAsync(x => x.CustomerId == customer.Id);
                //var membership = _unitOfWork.RepositoryR<MembershipAction>().FindBy(x =>
                //    x.CustomerId == transaction.CustomerId && x.ExpenseId == transaction.ExpenseId).FirstOrDefault();
                
                if (customer == null) return JsonUtil.Error(ValidatorMessage.Customer.NotExist);
                if (active != null)
                {
                    customer.StatusId = (int) EnumData.CustomerStatusEnum.Active; //Đã kích hoạt
                    customer.CustomerRoleId = (int) EnumData.CustomerRoleEnum.PremiumMember; // Premium
                    transaction.Note = note;
                    transaction.StatusTransactionId = (int) EnumData.TransactionStatusEnum.Accepted;
                    transaction.DateActive = DateTime.Now;

                    business.DateJoin = DateTime.Now;


                    if(customer.Language != null){
                        if (customer.Language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(customer.Id,
                                string.Format(ValidatorMessage.ContentNotify.AcceptPremium),
                                (int)EnumData.NotifyType.Customer, 0, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        else
                        {
                            var notify = _notifyService.CreateNotify(customer.Id,
                                string.Format(ValidatorMessage.ContentNotify.AcceptPremiumEnglish),
                                (int)EnumData.NotifyType.Customer, 0, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                    }else{
                        var notify = _notifyService.CreateNotify(customer.Id,
                            string.Format(ValidatorMessage.ContentNotify.AcceptPremium),
                            (int)EnumData.NotifyType.Customer, 0, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }

                    

                    string formatPhoneNumber = customer.PhoneNumber.Substring(0, 1);
                    if (formatPhoneNumber == "0")
                    {
                        formatPhoneNumber = customer.PhoneNumber.Substring(1, customer.PhoneNumber.Length - 1);
                    }
                    else
                    {
                        formatPhoneNumber = customer.PhoneNumber;
                    }

                    var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                    string content = $"Chuc mung Anh/Chi da tro thanh thanh vien chinh thuc cua OBC. Hotline ho tro: {hotline}";
                    

                    _accountService.SendMailCustomer(id, "Trở thành thành viên OBC", content);
                    var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);
                    

                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<Transaction>().Update(transaction);
                    await _unitOfWork.CommitAsync();

                    var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == transaction.ExpenseId && x.IsActive == true);
                    var memberOld = _unitOfWork.RepositoryR<MembershipAction>().GetSingle(x => x.CustomerId == transaction.CustomerId);
                    if (memberOld != null)
                    {
                        memberOld.IsEnabled = false;
                        _unitOfWork.RepositoryCRUD<MembershipAction>().Update(memberOld);
                        await _unitOfWork.CommitAsync();
                    }

                    var membership = new MembershipAction();
                    membership.Id = 0;
                    membership.IsEnabled = true;
                    membership.CustomerId = transaction.CustomerId;
                    membership.ExtendDate = null;
                    membership.ExpenseId = transaction.ExpenseId;
                    membership.IsActive = true;

                    membership.CreatedWhen = DateTime.Now;
                    if (expense.TimeType.ToLower().Contains("ngày"))
                        membership.EndDate = membership.CreatedWhen.GetValueOrDefault().AddDays(expense.Duration);
                    if (expense.TimeType.ToLower().Contains("tháng"))
                        membership.EndDate = membership.CreatedWhen.GetValueOrDefault().AddMonths(expense.Duration);
                    if (expense.TimeType.ToLower().Contains("năm"))
                        membership.EndDate = membership.CreatedWhen.GetValueOrDefault().AddYears(expense.Duration);

                    _unitOfWork.RepositoryCRUD<MembershipAction>().Insert(membership);
                    await _unitOfWork.CommitAsync();

                    LogAction logAction = new LogAction()
                    {
                        ChapterId = business.ParticipatingChapterId,
                        CustomerId = transaction.CustomerId,
                        ActionName = "New member",
                        MembershipActionId = membership.Id
                    };
                    _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                    await _unitOfWork.CommitAsync();

                    var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                        .FindBy(x => x.ChapterId == business.ParticipatingChapterId & x.Time.AddHours(1) > DateTime.Now).OrderByDescending(x => x.Id).ToList();
                    if (meetingChapter.Count != 0)
                    {
                        foreach (var item in meetingChapter)
                        {
                            if (!_unitOfWork.RepositoryR<MeetingChapterCheckIn>().AnyNotIsEnabled(x =>
                                x.MeetingChapterId == item.Id & x.CustomerId == transaction.CustomerId))
                            {
                                MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                {
                                    MeetingChapterId = item.Id,
                                    CustomerId = transaction.CustomerId,
                                    IsEnabled = false
                                };
                                _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabled(meetingChapterCheckIn);
                                await _unitOfWork.CommitAsync();
                            }
                        }
                    }

                    if (sendSms == false)
                    {
                        return JsonUtil.Success(true, "Đã có lỗi xảy ra khi gửi sms, vui lòng kiểm tra lại!");
                    }
                    else
                    {
                        return JsonUtil.Success(true);
                    }
                    
                }
                else
                {

                    var membershipAction = _unitOfWork.RepositoryR<MembershipAction>().FindBy(x =>
                        x.CustomerId == transaction.CustomerId && x.ExpenseId == transaction.ExpenseId).OrderByDescending(x => x.Id).FirstOrDefault();



                    transaction.Note = note;
                    transaction.StatusTransactionId = (int) EnumData.TransactionStatusEnum.Cancel;
                    customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;

                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<Transaction>().Update(transaction);
                    await _unitOfWork.CommitAsync();

                    return JsonUtil.Success(true);
                }
                

               
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> AcceptChapter(int id, string note, int? active, int customerId)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == id);
                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>().GetSingle(x => x.CustomerId == id);
                var chapter = await _unitOfWork.RepositoryR<Chapter>()
                    .GetSingleAsync(x => x.Id == business.ParticipatingChapterId.GetValueOrDefault());
                if (customer == null) return JsonUtil.Error(ValidatorMessage.Customer.NotExist);

                if (active != null)
                {
                    var data = _unitOfWork.Repository<Proc_CheckUniqueFieldOperationsChapter>()
                        .ExecProcedure(Proc_CheckUniqueFieldOperationsChapter.GetEntityProc(business.ParticipatingChapterId.GetValueOrDefault(), business.FieldOperationsId.GetValueOrDefault())).ToList();
                    if (data.Count > 0) { return JsonUtil.Error(ValidatorMessage.Business.NotAcceptChapter); }


                    if (customer.CustomerRoleId == (int) EnumData.CustomerRoleEnum.PremiumMember)
                    {
                        customer.StatusId = (int)EnumData.CustomerStatusEnum.Active; // Thành viên

                        if(customer.Language != null){
                            if (customer.Language.Equals("vi"))
                            {
                                var notify = _notifyService.CreateNotify(id,
                                    string.Format(ValidatorMessage.ContentNotify.AcceptPremium),
                                    (int)EnumData.NotifyType.Customer, 0, null, null);
                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }
                            else
                            {
                                var notify = _notifyService.CreateNotify(id,
                                    string.Format(ValidatorMessage.ContentNotify.AcceptPremiumEnglish),
                                    (int)EnumData.NotifyType.Customer, 0, null, null);
                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }
                        }else{
                            var notify = _notifyService.CreateNotify(id,
                                string.Format(ValidatorMessage.ContentNotify.AcceptPremium),
                                (int)EnumData.NotifyType.Customer, 0, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        

                        string formatPhoneNumber = customer.PhoneNumber.Substring(0, 1);
                        if (formatPhoneNumber == "0")
                        {
                            formatPhoneNumber = customer.PhoneNumber.Substring(1, customer.PhoneNumber.Length - 1);
                        }
                        else
                        {
                            formatPhoneNumber = customer.PhoneNumber;
                        }

                        var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                        string content = $"Chuc mung Anh/Chi da tro thanh thanh vien chinh thuc cua OBC. Hotline ho tro: {hotline}";
                       

                        _accountService.SendMailCustomer(id, "Trở thành thành viên OBC", content);

                        var sendSms = !_accountService.SendOTPSOAPViettel(formatPhoneNumber, content);

                        _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                        await _unitOfWork.CommitAsync();
                        LogAction logAction = new LogAction()
                        {
                            ChapterId = business.ParticipatingChapterId,
                            CustomerId = id,
                            ActionName = "New member",
                            MembershipActionId = _unitOfWork.RepositoryR<MembershipAction>()
                                    .FindBy(x => x.CustomerId == id).OrderByDescending(x => x.Id).FirstOrDefault().Id
                        };
                        _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                        await _unitOfWork.CommitAsync();

                        var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                            .FindBy(x => x.ChapterId == chapter.Id & x.Time.AddHours(1) > DateTime.Now).OrderByDescending(x => x.Id).ToList();
                        if (meetingChapter.Count != 0)
                        {
                            foreach (var item in meetingChapter)
                            {
                                if (!_unitOfWork.RepositoryR<MeetingChapterCheckIn>().AnyNotIsEnabled(x =>
                                    x.MeetingChapterId == item.Id & x.CustomerId == id))
                                {
                                    MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                    {
                                        MeetingChapterId = item.Id,
                                        CustomerId = id,
                                        IsEnabled = false
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabled(meetingChapterCheckIn);
                                    await _unitOfWork.CommitAsync();
                                }
                            }
                        }

                        if (sendSms == false)
                        {

                            customer.Note = note;

                            business.DateJoin = DateTime.Now;

                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            await _unitOfWork.CommitAsync();
                            _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                            await _unitOfWork.CommitAsync();
                            return JsonUtil.Success(true, "Đã có lỗi xảy ra khi gửi sms, vui lòng kiểm tra lại!");
                        }
                        else
                        {

                            customer.Note = note;

                            business.DateJoin = DateTime.Now;

                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            await _unitOfWork.CommitAsync();
                            _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                            await _unitOfWork.CommitAsync();
                            return JsonUtil.Success(true);
                        }
                    }
                    else
                    {
                        customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter; //Đã kích 

                        if(customer.Language != null){
                            if (customer.Language.Equals("vi"))
                            {
                                var notify = _notifyService.CreateNotify(id,
                                    string.Format(ValidatorMessage.ContentNotify.AcceptChapter, chapter.Name),
                                    (int)EnumData.NotifyType.Customer, business.ParticipatingChapterId.GetValueOrDefault(), null, null);
                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }
                            else
                            {
                                var notify = _notifyService.CreateNotify(id,
                                    string.Format(ValidatorMessage.ContentNotify.AcceptChapterEnglish, chapter.Name),
                                    (int)EnumData.NotifyType.Customer, business.ParticipatingChapterId.GetValueOrDefault(), null, null);
                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }
                        }else{
                            var notify = _notifyService.CreateNotify(id,
                                string.Format(ValidatorMessage.ContentNotify.AcceptChapter, chapter.Name),
                                (int)EnumData.NotifyType.Customer, business.ParticipatingChapterId.GetValueOrDefault(), null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }

                        customer.Note = note;

                        business.DateJoin = DateTime.Now;

                        _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                        await _unitOfWork.CommitAsync();
                        _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                        await _unitOfWork.CommitAsync();

                        return JsonUtil.Success(true);
                    }
                }
                else
                {

                    if(customer.Language != null){
                        if (customer.Language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(id,
                                string.Format(ValidatorMessage.ContentNotify.CancelChapter, chapter.Name),
                                (int)EnumData.NotifyType.Customer, business.ParticipatingChapterId.GetValueOrDefault(), customerId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        else
                        {
                            var notify = _notifyService.CreateNotify(id,
                                string.Format(ValidatorMessage.ContentNotify.CancelChapterEnglish, chapter.Name),
                                (int)EnumData.NotifyType.Customer, business.ParticipatingChapterId.GetValueOrDefault(), customerId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                    }else{
                        var notify = _notifyService.CreateNotify(id,
                            string.Format(ValidatorMessage.ContentNotify.CancelChapter, chapter.Name),
                            (int)EnumData.NotifyType.Customer, business.ParticipatingChapterId.GetValueOrDefault(), customerId, note);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }


                    var transaction = await _unitOfWork.RepositoryR<Transaction>().FindBy(x => x.CustomerId == id)
                        .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    if (transaction != null && transaction.StatusTransactionId == (int)EnumData.TransactionStatusEnum.PendingActive)
                    {
                        transaction.Note = note;
                        transaction.StatusTransactionId = (int)EnumData.TransactionStatusEnum.Cancel;
                        _unitOfWork.RepositoryCRUD<Transaction>().Update(transaction);
                        await _unitOfWork.CommitAsync();
                    }
                    customer.Note = note;
                    customer.StatusId = (int)EnumData.CustomerStatusEnum.FreeMember;
                    business.ParticipatingChapterId = null;
                    business.ParticipatingProvinceId = null;
                    business.ParticipatingRegionId = null;

                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    await _unitOfWork.CommitAsync();
                    _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                    await _unitOfWork.CommitAsync();

                    return JsonUtil.Success(true);

                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CancelMember(int id, string note)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == id);
                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingleAsync(x => x.CustomerId == id);
                customer.StatusId = (int) EnumData.CustomerStatusEnum.FreeMember;
                customer.RoleId = null;
                //customer.CustomerRoleId = (int) EnumData.CustomerRoleEnum.FreeMember;


                LogAction logAction = new LogAction()
                {
                    ChapterId = business.ParticipatingChapterId,
                    CustomerId = id,
                    ActionName = "Rời tổ chức"
                };

                business.ParticipatingChapterId = null;
                business.ParticipatingProvinceId = null;
                business.ParticipatingRegionId = null;

                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                await _unitOfWork.CommitAsync();
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                await _unitOfWork.CommitAsync();
                _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                await _unitOfWork.CommitAsync();



                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListCustomerWaitingActive(int id, string keySearch, DateTime fromDate, DateTime toDate, int chapterId, int statusId,
            int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListCustomerWaitingActive>()
                    .ExecProcedure(Proc_GetListCustomerWaitingActive.GetEntityProc(id, keySearch, fromDate, toDate.AddDays(1), chapterId, statusId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListCustomerMember(int chapterId, string keySearch, string fieldOperations, string status, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                if (!string.IsNullOrEmpty(fieldOperations) || !string.IsNullOrWhiteSpace(fieldOperations)) fieldOperations = fieldOperations.Trim();
                string statusNew;
                if (!string.IsNullOrEmpty(status) || !string.IsNullOrWhiteSpace(status))
                {
                    int[] statusList = Array.ConvertAll(status.Split(',').Reverse().ToArray(), int.Parse);
                    if (Array.IndexOf(statusList, 0) >= 0)
                    {
                        statusNew = "0";
                    }
                    else
                    {
                        statusNew = status;
                    }
                }
                else
                {
                    statusNew = "";
                }
                var data = _unitOfWork.Repository<Proc_GetListMemberChapter>()
                    .ExecProcedure(Proc_GetListMemberChapter.GetEntityProc(chapterId,keySearch, fieldOperations, statusNew, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> ChangeCustomerRole(int customerId, int? roleId)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == customerId);
                if(customer.CustomerRoleId != (int) EnumData.CustomerRoleEnum.PremiumMember) 
                    return JsonUtil.Error(ValidatorMessage.Customer.NotChangeRole);
                customer.RoleId = roleId;
                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetListRoleMemberChapter()
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<Role>().FindBy(x => x.RoleTypeId == 2).ToListAsync();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetInformationCustomer(int id)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == id);
                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingleAsync(x => x.CustomerId == id);
                var status = await _unitOfWork.RepositoryR<Status>().GetSingleAsync(x => x.Id == customer.StatusId);
                var linkGroupChat = await _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefaultAsync();
                var assess = _unitOfWork.RepositoryR<AssessCustomer>().FindBy(x => x.CustomerId == id).ToList();
                float avgAssess;
                if (assess.Count == 0)
                {
                    avgAssess = 0;
                }
                else
                {
                    avgAssess = (float)assess.Select(x => x.Value).Sum() / assess.Count;
                }
                
                Chapter chapter = new Chapter();
                if (business.ParticipatingChapterId != null)
                   chapter = _unitOfWork.RepositoryR<Chapter>()
                        .GetSingle(x => x.Id == business.ParticipatingChapterId);

                if(customer.Language != null){
                    if (customer.Language.Equals("vi"))
                    {
                        if (customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                        {
                            var member = await _unitOfWork.RepositoryR<MembershipAction>()
                                .FindBy(x => x.CustomerId == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                            if (member == null)
                            {
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Name,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }

                            if (member.EndDate < DateTime.Now && member.ExtendDate == null)
                            {
                                customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                                customer.RoleId = null;
                                customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                                await _unitOfWork.CommitAsync();

                                member.IsEnabled = false;
                                _unitOfWork.RepositoryCRUD<MembershipAction>().Update(member);
                                await _unitOfWork.CommitAsync();

                                LogAction logAction = new LogAction()
                                {
                                    ChapterId = business.ParticipatingChapterId,
                                    CustomerId = id,
                                    ActionName = "Hết hạn"
                                };
                                _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                                await _unitOfWork.CommitAsync();
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Name,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }

                            if (member.ExtendDate != null && member.ExtendDate < DateTime.Now)
                            {
                                customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                                customer.RoleId = null;
                                customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                                await _unitOfWork.CommitAsync();

                                member.IsEnabled = false;
                                _unitOfWork.RepositoryCRUD<MembershipAction>().Update(member);
                                await _unitOfWork.CommitAsync();

                                LogAction logAction = new LogAction()
                                {
                                    ChapterId = business.ParticipatingChapterId,
                                    CustomerId = id,
                                    ActionName = "Hết hạn"
                                };
                                _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                                await _unitOfWork.CommitAsync();
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Name,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }

                            if (member.ExtendDate != null)
                            {
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Name,
                                    Time = member.ExtendDate,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }
                            else
                            {
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Name,
                                    Time = member.EndDate,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }
                           
                        }

                        return JsonUtil.Success(new
                        {
                            Id = id,
                            CustomerName = customer.FullName,
                            BusinessName = business.BusinessName,
                            ChapterName = chapter.Name,
                            StatusId = customer.StatusId,
                            StatusName = status.Name,
                            AvatarPath = customer.AvatarPath,
                            LinkGroupChat = linkGroupChat.LinkGroupChat,
                            SumAssess = assess.Count,
                            Assess = avgAssess
                        });
                    }
                    else
                    {
                        if (customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                        {
                            var member = await _unitOfWork.RepositoryR<MembershipAction>()
                                .FindBy(x => x.CustomerId == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                            if (member == null)
                            {
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Code,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }

                            if (member.EndDate < DateTime.Now && member.ExtendDate == null)
                            {
                                customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                                customer.RoleId = null;
                                customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                                await _unitOfWork.CommitAsync();

                                member.IsEnabled = false;
                                _unitOfWork.RepositoryCRUD<MembershipAction>().Update(member);
                                await _unitOfWork.CommitAsync();

                                LogAction logAction = new LogAction()
                                {
                                    ChapterId = business.ParticipatingChapterId,
                                    CustomerId = id,
                                    ActionName = "Hết hạn"
                                };
                                _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                                await _unitOfWork.CommitAsync();

                                status = await _unitOfWork.RepositoryR<Status>().GetSingleAsync(x => x.Id == customer.StatusId);
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Code,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }

                            if (member.ExtendDate != null && member.ExtendDate < DateTime.Now)
                            {
                                customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                                customer.RoleId = null;
                                customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                                await _unitOfWork.CommitAsync();

                                member.IsEnabled = false;
                                _unitOfWork.RepositoryCRUD<MembershipAction>().Update(member);
                                await _unitOfWork.CommitAsync();

                                LogAction logAction = new LogAction()
                                {
                                    ChapterId = business.ParticipatingChapterId,
                                    CustomerId = id,
                                    ActionName = "Hết hạn"
                                };
                                _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                                await _unitOfWork.CommitAsync();

                                status = await _unitOfWork.RepositoryR<Status>().GetSingleAsync(x => x.Id == customer.StatusId);
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Code,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }

                            if (member.ExtendDate != null)
                            {
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Code,
                                    Time = member.ExtendDate,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }
                            else
                            {
                                return JsonUtil.Success(new
                                {
                                    Id = id,
                                    CustomerName = customer.FullName,
                                    BusinessName = business.BusinessName,
                                    ChapterName = chapter.Name,
                                    StatusId = customer.StatusId,
                                    StatusName = status.Code,
                                    Time = member.EndDate,
                                    AvatarPath = customer.AvatarPath,
                                    LinkGroupChat = linkGroupChat.LinkGroupChat,
                                    SumAssess = assess.Count,
                                    Assess = avgAssess
                                });
                            }
                        }

                        return JsonUtil.Success(new
                        {
                            Id = id,
                            CustomerName = customer.FullName,
                            BusinessName = business.BusinessName,
                            ChapterName = chapter.Name,
                            StatusId = customer.StatusId,
                            StatusName = status.Code,
                            AvatarPath = customer.AvatarPath,
                            LinkGroupChat = linkGroupChat.LinkGroupChat,
                            SumAssess = assess.Count,
                            Assess = avgAssess
                        });
                    }
                }else{
                    if (customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                    {
                        var member = await _unitOfWork.RepositoryR<MembershipAction>()
                            .FindBy(x => x.CustomerId == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                        if (member == null)
                        {
                            return JsonUtil.Success(new
                            {
                                Id = id,
                                CustomerName = customer.FullName,
                                BusinessName = business.BusinessName,
                                ChapterName = chapter.Name,
                                StatusId = customer.StatusId,
                                StatusName = status.Name,
                                AvatarPath = customer.AvatarPath,
                                LinkGroupChat = linkGroupChat.LinkGroupChat,
                                SumAssess = assess.Count,
                                Assess = avgAssess
                            });
                        }

                        if (member.EndDate < DateTime.Now && member.ExtendDate == null)
                        {
                            customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                            customer.RoleId = null;
                            customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            await _unitOfWork.CommitAsync();

                            member.IsEnabled = false;
                            _unitOfWork.RepositoryCRUD<MembershipAction>().Update(member);
                            await _unitOfWork.CommitAsync();

                            LogAction logAction = new LogAction()
                            {
                                ChapterId = business.ParticipatingChapterId,
                                CustomerId = id,
                                ActionName = "Hết hạn"
                            };
                            _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                            await _unitOfWork.CommitAsync();
                            return JsonUtil.Success(new
                            {
                                Id = id,
                                CustomerName = customer.FullName,
                                BusinessName = business.BusinessName,
                                ChapterName = chapter.Name,
                                StatusId = customer.StatusId,
                                StatusName = status.Name,
                                AvatarPath = customer.AvatarPath,
                                LinkGroupChat = linkGroupChat.LinkGroupChat,
                                SumAssess = assess.Count,
                                Assess = avgAssess
                            });
                        }

                        if (member.ExtendDate != null && member.ExtendDate < DateTime.Now)
                        {
                            customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                            customer.RoleId = null;
                            customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            await _unitOfWork.CommitAsync();

                            member.IsEnabled = false;
                            _unitOfWork.RepositoryCRUD<MembershipAction>().Update(member);
                            await _unitOfWork.CommitAsync();

                            LogAction logAction = new LogAction()
                            {
                                ChapterId = business.ParticipatingChapterId,
                                CustomerId = id,
                                ActionName = "Hết hạn"
                            };
                            _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                            await _unitOfWork.CommitAsync();
                            return JsonUtil.Success(new
                            {
                                Id = id,
                                CustomerName = customer.FullName,
                                BusinessName = business.BusinessName,
                                ChapterName = chapter.Name,
                                StatusId = customer.StatusId,
                                StatusName = status.Name,
                                AvatarPath = customer.AvatarPath,
                                LinkGroupChat = linkGroupChat.LinkGroupChat,
                                SumAssess = assess.Count,
                                Assess = avgAssess
                            });
                        }

                        if (member.ExtendDate != null)
                        {
                            return JsonUtil.Success(new
                            {
                                Id = id,
                                CustomerName = customer.FullName,
                                BusinessName = business.BusinessName,
                                ChapterName = chapter.Name,
                                StatusId = customer.StatusId,
                                StatusName = status.Name,
                                Time = member.ExtendDate,
                                AvatarPath = customer.AvatarPath,
                                LinkGroupChat = linkGroupChat.LinkGroupChat,
                                SumAssess = assess.Count,
                                Assess = avgAssess
                            });
                        }
                        else
                        {
                            return JsonUtil.Success(new
                            {
                                Id = id,
                                CustomerName = customer.FullName,
                                BusinessName = business.BusinessName,
                                ChapterName = chapter.Name,
                                StatusId = customer.StatusId,
                                StatusName = status.Name,
                                Time = member.EndDate,
                                AvatarPath = customer.AvatarPath,
                                LinkGroupChat = linkGroupChat.LinkGroupChat,
                                SumAssess = assess.Count,
                                Assess = avgAssess
                            });
                        }
                    }

                    return JsonUtil.Success(new
                    {
                        Id = id,
                        CustomerName = customer.FullName,
                        BusinessName = business.BusinessName,
                        ChapterName = chapter.Name,
                        StatusId = customer.StatusId,
                        StatusName = status.Name,
                        AvatarPath = customer.AvatarPath,
                        LinkGroupChat = linkGroupChat.LinkGroupChat,
                        SumAssess = assess.Count,
                        Assess = avgAssess
                    });
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListCustomerOutOfChapter(int customerId, string fullName, string businessName, string fieldOperationsName, string provinceName, string keySearch, int type)
        {
            try
            {
                if (!string.IsNullOrEmpty(fullName) || !string.IsNullOrWhiteSpace(fullName)) fullName = fullName.Trim();
                if (!string.IsNullOrEmpty(businessName) || !string.IsNullOrWhiteSpace(businessName)) businessName = businessName.Trim();
                if (!string.IsNullOrEmpty(fieldOperationsName) || !string.IsNullOrWhiteSpace(fieldOperationsName)) fieldOperationsName = fieldOperationsName.Trim();
                if (!string.IsNullOrEmpty(provinceName) || !string.IsNullOrWhiteSpace(provinceName)) provinceName = provinceName.Trim();
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListCustomerOutOfChapter>()
                    .ExecProcedure(Proc_GetListCustomerOutOfChapter.GetEntityProc(customerId, fullName, businessName, fieldOperationsName, provinceName,keySearch,type)).ToList();
                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetContractCustomer(int id)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == id);
                if (customer.WardId == null)
                {
                    var province = _unitOfWork.RepositoryR<Provinces>().GetSingle(x => x.Id == customer.ProvinceId);
                    return JsonUtil.Success(new
                    {
                        Id = customer.Id,
                        FullName = customer.FullName,
                        PhoneNumber = customer.PhoneNumber,
                        Email = customer.Email,
                        Address = province.FullName
                    });
                }
                else
                {
                    var ward = _unitOfWork.RepositoryR<Wards>().GetSingle(x => x.Id == customer.WardId);
                    var district = _unitOfWork.RepositoryR<Districts>().GetSingle(x => x.Id == ward.DistrictId);
                    var province = _unitOfWork.RepositoryR<Provinces>().GetSingle(x => x.Id == district.ProvinceId);

                    return JsonUtil.Success(new
                    {
                        Id = customer.Id,
                        FullName = customer.FullName,
                        PhoneNumber = customer.PhoneNumber,
                        Email = customer.Email,
                        Address = customer.Address + ", " + ward.FullName + ", " + district.FullName + ", " + province.FullName
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetCustomerProfile(int id, int currentUserId, string phoneNumber)
        {
            try
            {
                if (id == 0) id = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.PhoneNumber == phoneNumber).Id;
                var data = _unitOfWork.Repository<Proc_GetCustomerProfile>()
                    .ExecProcedure(Proc_GetCustomerProfile.GetEntityProc(id)).FirstOrDefault();
                var assess = _unitOfWork.RepositoryR<AssessCustomer>()
                    .FindBy(x => x.CreatedBy == currentUserId && x.CustomerId == id).OrderByDescending(x => x.Id)
                    .FirstOrDefault();
                var assessed = false;
                var currentCustomer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentUserId);
                string language = currentCustomer.Language;
                if (_unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == data.CustomerId).StatusId !=
                    (int) EnumData.CustomerStatusEnum.Active)
                    assessed = true;
                var chapterCustomer = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == id).ParticipatingChapterId.GetValueOrDefault(0);
                var chapterCurrent = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == currentUserId).ParticipatingChapterId.GetValueOrDefault(0);
                if (assess == null)
                {
                    var checkOpportunity = _unitOfWork.RepositoryR<Opportunity>().Any(x =>
                        ((x.CustomerId == currentUserId && x.ReceiverId == id) ||
                         (x.ReceiverId == currentUserId && x.CustomerId == id)));
                    var checkFaceToFace = _unitOfWork.RepositoryR<FaceToFace>().Any(x =>
                        ((x.CustomerId == currentUserId && x.ReceiverId == id) ||
                         (x.ReceiverId == currentUserId && x.CustomerId == id)));
                    var checkThanks = _unitOfWork.RepositoryR<Thanks>().Any(x =>
                        ((x.CustomerId == currentUserId && x.ReceiverId == id) ||
                         (x.ReceiverId == currentUserId && x.CustomerId == id)));

                    if (checkFaceToFace == false && checkOpportunity == false && checkThanks == false) assessed = true;
                }
                if (assess != null)
                {
                    var checkOpportunity = _unitOfWork.RepositoryR<Opportunity>().Any(x =>
                        ((x.CustomerId == currentUserId && x.ReceiverId == id) ||
                         (x.ReceiverId == currentUserId && x.CustomerId == id)) && x.CreatedWhen > assess.CreatedWhen);
                    var checkFaceToFace = _unitOfWork.RepositoryR<FaceToFace>().Any(x =>
                        ((x.CustomerId == currentUserId && x.ReceiverId == id) ||
                         (x.ReceiverId == currentUserId && x.CustomerId == id)) && x.CreatedWhen > assess.CreatedWhen);
                    var checkThanks = _unitOfWork.RepositoryR<Thanks>().Any(x =>
                        ((x.CustomerId == currentUserId && x.ReceiverId == id) ||
                         (x.ReceiverId == currentUserId && x.CustomerId == id)) && x.CreatedWhen > assess.CreatedWhen);

                    if (checkFaceToFace == false && checkOpportunity == false && checkThanks == false) assessed = true;
                }


                //var checkAssess = _unitOfWork.RepositoryR<AssessCustomer>()
                //    .Any(x => x.CreatedBy == currentUserId && x.CustomerId == id);


                if (language != null){
                    if (language.Equals("vi"))
                    {
                        CustomerViewModelProfile result = new CustomerViewModelProfile()
                        {
                            CustomerId = data.CustomerId,
                            Email = data.Email,
                            PhoneNumber = data.PhoneNumber,
                            BusinessName = data.BusinessName,
                            Address = data.Address,
                            Position = data.Position,
                            Birthday = data.Birthday,
                            ChapterName = data.ChapterName,
                            AvatarPath = data.AvatarPath,
                            CustomerName = data.CustomerName,
                            FieldOperationName = data.FieldOperationName,
                            Gender = data.Gender,
                            ProfessionName = data.ProfessionName,
                            QrCodePath = data.QrCodePath,
                            Assessed = assessed,
                            StatusId = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == id).StatusId
                        };
                        if (chapterCustomer != chapterCurrent)
                        {
                            result.ChapterType = "Ngoài chapter";
                        }
                        else
                        {
                            result.ChapterType = "Nội bộ";
                        }

                        if (currentCustomer.StatusId != (int) EnumData.CustomerStatusEnum.Active)
                        {
                            result.StatusId = 1;
                        }
                        if (chapterCustomer == 0) result.StatusId = 1;
                        return JsonUtil.Success(result);
                    }
                    else
                    {
                        CustomerViewModelProfile result = new CustomerViewModelProfile()
                        {
                            CustomerId = data.CustomerId,
                            Email = data.Email,
                            PhoneNumber = data.PhoneNumber,
                            BusinessName = data.BusinessName,
                            Address = data.Address,
                            Position = data.Position,
                            Birthday = data.Birthday,
                            ChapterName = data.ChapterName,
                            AvatarPath = data.AvatarPath,
                            CustomerName = data.CustomerName,
                            FieldOperationName = data.FieldOperationCode,
                            Gender = data.Gender,
                            ProfessionName = data.ProfessionCode,
                            QrCodePath = data.QrCodePath,
                            Assessed = assessed,
                            StatusId = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == id).StatusId
                        };
                        if (chapterCustomer != chapterCurrent)
                        {
                            result.ChapterType = "External Chapter";
                        }
                        else
                        {
                            result.ChapterType = "Internal";
                        }
                        if (currentCustomer.StatusId != (int)EnumData.CustomerStatusEnum.Active)
                        {
                            result.StatusId = 1;
                        }
                        if (chapterCustomer == 0) result.StatusId = 1;
                        return JsonUtil.Success(result);
                    }
                }else{
                    CustomerViewModelProfile result = new CustomerViewModelProfile()
                    {
                        CustomerId = data.CustomerId,
                        Email = data.Email,
                        PhoneNumber = data.PhoneNumber,
                        BusinessName = data.BusinessName,
                        Address = data.Address,
                        Position = data.Position,
                        Birthday = data.Birthday,
                        ChapterName = data.ChapterName,
                        AvatarPath = data.AvatarPath,
                        CustomerName = data.CustomerName,
                        FieldOperationName = data.FieldOperationName,
                        Gender = data.Gender,
                        ProfessionName = data.ProfessionName,
                        QrCodePath = data.QrCodePath,
                        Assessed = assessed,
                        StatusId = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == id).StatusId
                    };
                    if (chapterCustomer != chapterCurrent)
                    {
                        result.ChapterType = "Ngoài chapter";
                    }
                    else
                    {
                        result.ChapterType = "Nội bộ";
                    }
                    if (currentCustomer.StatusId != (int)EnumData.CustomerStatusEnum.Active)
                    {
                        result.StatusId = 1;
                    }
                    if (chapterCustomer == 0) result.StatusId = 1;
                    return JsonUtil.Success(result);
                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadAvatarCustomer(CustomerViewModelUploadAvatar model, int customerId)
        {
            try
            {
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (language.Equals("en"))
                {
                    if (model.File == null || model.File.Length == 0)
                        return JsonUtil.Error("File not selected");
                    else
                    {
                        if (model.File.ContentType.Split('/')[0] != "image")
                        {
                            return JsonUtil.Error("Only pictures allowed, please choose again!");
                        }

                        if (model.File.Length > 10000000)
                        {
                            return JsonUtil.Error("File size exceeds the allowed limit, please choose again!");
                        }
                    }
                }
                else
                {
                    if (model.File == null || model.File.Length == 0)
                        return JsonUtil.Error("Vui lòng chọn tệp");
                    else
                    {
                        if (model.File.ContentType.Split('/')[0] != "image")
                        {
                            return JsonUtil.Error("Chỉ cho phép hình ảnh, vui lòng chọn lại!");
                        }

                        if (model.File.Length > 10000000)
                        {
                            return JsonUtil.Error("Dung lượng tệp quá giới hạn cho phép, vui lòng chọn lại!");
                        }
                    }
                }
                
                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "Avatar",
                        "Customer-" + customerId + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;

                return JsonUtil.Success(link);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetCustomerOutOfChapterWithQrCode(int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var ward = _unitOfWork.RepositoryR<Wards>().GetSingle(x => x.Id == customer.WardId);
                var district = _unitOfWork.RepositoryR<Districts>().GetSingle(x => x.Id == ward.DistrictId);
                var province = _unitOfWork.RepositoryR<Provinces>().GetSingle(x => x.Id == district.ProvinceId);
                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId);
                var fieldOperations = _unitOfWork.RepositoryR<FieldOperations>().GetSingle(x => x.Id == business.FieldOperationsId);

                return JsonUtil.Success(new
                {
                    FullName = customer.FullName,
                    BusinessName = business.BusinessName,
                    FieldOperationsName = fieldOperations.Name,
                    ProvinceName = province.Name
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult UpdateExpoPushTokenCustomer(int customerId, string expoPushToken, string language)
        {
            try
            {
                if (string.IsNullOrEmpty(expoPushToken) || string.IsNullOrWhiteSpace(expoPushToken))
                    return JsonUtil.Error("false");
                if (string.IsNullOrEmpty(language) || string.IsNullOrWhiteSpace(language))
                    language = "vi";
                var checkExpoToken =
                    _unitOfWork.RepositoryR<Customer>().FindBy(x => x.ExpoPushToken == expoPushToken).ToList();
                if (checkExpoToken.Count > 0)
                {
                    foreach (var item in checkExpoToken)
                    {
                        item.ExpoPushToken = "";
                        _unitOfWork.RepositoryCRUD<Customer>().Update(item);
                        _unitOfWork.Commit();
                    }
                }
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (customer.ExpoPushToken != "" || customer.ExpoPushToken != null)
                {
                    var notify = _notifyService.CreateNotifyWhenCustomerLogInDeviceDifferent(customerId);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }
                customer.ExpoPushToken = expoPushToken;
                customer.Language = language;

                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult DeleteExpoPushTokenCustomer(int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                customer.ExpoPushToken = "";

                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetIndicators(int customerId)
        {
            try
            {
                var result = new CustomerViewModelViewIndicators();
                result.Indicators = _unitOfWork.Repository<Proc_GetIndicators>()
                    .ExecProcedure(Proc_GetIndicators.GetEntityProc(customerId)).ToList();

                var customerEvent = _unitOfWork.RepositoryR<CustomerEvent>().FindBy(x => x.CustomerId == customerId)
                    .ToList();
                var listEventModel = new List<EventModel>();
                foreach (var cusEvent in customerEvent)
                {
                    var eventModel = new EventModel()
                    {
                        EventId = cusEvent.EventId,
                        EventName = _unitOfWork.RepositoryR<Event>().GetSingleNotEnabled(x => x.Id == cusEvent.EventId).Name,
                        CreatedWhen = cusEvent.CreatedWhen.GetValueOrDefault()
                    };

                    listEventModel.Add(eventModel);
                }

                result.Event = listEventModel;
                var customerCourse = _unitOfWork.RepositoryR<CustomerCourse>().FindBy(x => x.CustomerId == customerId)
                    .ToList();
                var listEducation = new List<Education>();
                foreach (var cusCourse in customerCourse)
                {
                    var education = new Education()
                    {
                        EducationId = cusCourse.CourseId,
                        EducationName = _unitOfWork.RepositoryR<Course>().GetSingleNotEnabled(x => x.Id == cusCourse.CourseId).Name,
                        CreatedWhen = cusCourse.CreatedWhen.GetValueOrDefault()
                    };

                    listEducation.Add(education);
                }

                result.Education = listEducation;

                var listMeetingChapterModel = new List<EventModel>();
                var data = _unitOfWork.Repository<Proc_GetMeetingChapterWithCustomerId>()
                    .ExecProcedure(Proc_GetMeetingChapterWithCustomerId.GetEntityProc(customerId)).ToList();
                foreach (var item in data)
                {
                    EventModel itemMeetingChapter = new EventModel()
                    {
                        EventId = item.Id,
                        CreatedWhen = item.CreatedWhen,
                        EventName = item.Name
                    };

                    listMeetingChapterModel.Add(itemMeetingChapter);
                }

                result.MeetingChapter = listMeetingChapterModel;
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult AssessCustomer(int customerId, int value, string comment)
        {
            try
            {
                var assessCustomer = new AssessCustomer()
                {
                    CustomerId = customerId,
                    Value = value,
                    Comment = comment
                };

                _unitOfWork.RepositoryCRUD<AssessCustomer>().Insert(assessCustomer);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListTopCustomer(int customerId, int pageNum)
        {
            try
            {
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;

                var data = _unitOfWork.Repository<Proc_GetListTopCustomer>()
                    .ExecProcedure(Proc_GetListTopCustomer.GetEntityProc(pageNum, 5, language)).ToList();
                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult UpdateLanguageCustomer(int customerId, string language)
        {
            try
            {
                if (string.IsNullOrEmpty(language) || string.IsNullOrWhiteSpace(language))
                    language = "vi";
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                customer.Language = language;

                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckCustomer(int customerId, string expoPushToken)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>()
                    .GetSingle(x => x.Id == customerId && x.IsActive == true);
                if (customer == null)
                {
                    return JsonUtil.Success(false, "");
                }
                else
                {
                    var checkExpoToken = _unitOfWork.RepositoryR<Customer>()
                        .GetSingle(x => x.Id == customerId);
                    if (!checkExpoToken.ExpoPushToken.Equals(expoPushToken))
                    {
                        return JsonUtil.Success(false, "logged in another device");
                    }
                    else
                    {
                        return JsonUtil.Success(true);
                    }
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllAssessCustomer(int customerId, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListAssessCustomer>()
                    .ExecProcedure(Proc_GetListAssessCustomer.GetEntityProc(customerId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAvgAssessCustomer(int customerId)
        {
            try
            {
                var listAssessCustomer = _unitOfWork.RepositoryR<AssessCustomer>()
                    .FindBy(x => x.CustomerId == customerId).ToList();
                var sumAssessCustomer = _unitOfWork.RepositoryR<AssessCustomer>()
                    .FindBy(x => x.CustomerId == customerId).Sum(x => x.Value);
                var avgAssess = Math.Round((sumAssessCustomer * 1.0) / listAssessCustomer.Count, 2);

                return JsonUtil.Success(new
                {
                    AvgAssess = avgAssess,
                    SumAssess = listAssessCustomer.Count
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SendMailOTPChangeEmailCustomer(int customerId, string newEmail, int sendAgain)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (sendAgain == 0)
                {
                    var checkEmail = _unitOfWork.RepositoryR<Customer>().Any(x => x.Email.Equals(newEmail));
                    if (checkEmail)
                    {
                        if (customer.Language.Equals("en"))
                        {
                            return JsonUtil.Error("Your new email is already in the system, please enter another email");
                        }
                        else
                        {
                            return JsonUtil.Error("Email mới của bạn đã có trong hệ thống, vui lòng nhập email khác");
                        }
                    }
                    customer.NewEmail = newEmail;

                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    _unitOfWork.Commit();
                }

                DateTime currentDate = DateTime.Now;
                var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();
                if (customer.VerificationDateTime == null || customer.VerificationCode == null)
                {
                    customer.VerificationCode = codeResetPassWord;
                    customer.VerificationDateTime = currentDate;
                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    _unitOfWork.Commit();
                }
                else
                {
                    dynamic resetPassWordSentat = customer.VerificationDateTime;
                    if (resetPassWordSentat != null)
                    {
                        TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);
                        if (timeConfirmResetPassWord.TotalMinutes <= 1)
                        {
                            if (customer.Language != null)
                            {
                                if (customer.Language.Equals("en"))
                                {
                                    return JsonUtil.Error("You are sending email continuously, please resend in 1 minute.");
                                }
                                else
                                {
                                    return JsonUtil.Error("Bạn đang gửi mail liên tục, bạn vui lòng gửi lại sau 1 phút.");
                                }
                            }
                            else
                            {
                                return JsonUtil.Error("Bạn đang gửi mail liên tục, bạn vui lòng gửi lại sau 1 phút.");
                            }

                        }
                        else
                        {
                            customer.VerificationCode = codeResetPassWord;
                            customer.VerificationDateTime = currentDate;
                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            _unitOfWork.CommitAsync();
                        }
                    }
                }

                var emailRecipient = new EmailRecipient(
                    customer.Id,
                    customer.NewEmail,
                    customer.FullName,
                    customer.VerificationCode, customer.PhoneNumber, ""
                   );

                if (customer.Language != null)
                {
                    if (customer.Language.Equals("en"))
                    {
                        var result = _accountService.SendEmail(_iSendMailOTP, emailRecipient, " Change Email ");
                        if (sendAgain == 0)
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Sent confirmation of email change to " + customer.NewEmail);
                        }
                        else
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"OTP code sent to confirm email exchange to " + customer.NewEmail);
                        }
                        
                    }
                    else
                    {
                        var result = _accountService.SendEmail(_iSendMailOTP, emailRecipient, " Đổi Email ");
                        if (sendAgain == 0)
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Đã gửi xác nhận đổi email đến " + customer.NewEmail);
                        }
                        else
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Đã gửi lại mã OTP xác nhận đổi email đến " + customer.NewEmail);
                        }
                       
                    }
                }
                else
                {
                    var result = _accountService.SendEmail(_iSendMailOTP, emailRecipient, " Đổi email ");
                    if (sendAgain == 0)
                    {
                        return JsonUtil.Success(new
                            {
                                OTP = codeResetPassWord,
                                CustomerId = customer.Id
                            }, $"Đã gửi xác nhận đổi email đến " + customer.NewEmail);
                    }
                    else
                    {
                        return JsonUtil.Success(new
                            {
                                OTP = codeResetPassWord,
                                CustomerId = customer.Id
                            }, $"Đã gửi lại mã OTP xác nhận đổi email đến " + customer.NewEmail);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> SendOTPByPhoneChangePhoneCustomer(int customerId, string newPhoneNumber, int sendAgain)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (sendAgain == 0)
                {
                    var checkPhoneNumber =
                        _unitOfWork.RepositoryR<Customer>().Any(x => x.PhoneNumber.Equals(newPhoneNumber));
                    if (checkPhoneNumber)
                    {
                        if (customer.Language.Equals("en"))
                        {
                            return JsonUtil.Error("Your new phone number is already in the system, please enter another phone number");
                        }
                        else
                        {
                            return JsonUtil.Error("Số điện thoại mới của bạn đã có trong hệ thống, vui lòng nhập số điện thoại khác");
                        }
                    }
                    customer.NewPhoneNumber = newPhoneNumber;

                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    _unitOfWork.Commit();
                }


                DateTime currentDate = DateTime.Now;
                var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();

                string formatPhoneNumber = newPhoneNumber.Substring(0, 1);
                if (formatPhoneNumber == "0")
                {
                    formatPhoneNumber = newPhoneNumber.Substring(1, newPhoneNumber.Length - 1);
                }
                else
                {
                    formatPhoneNumber = newPhoneNumber;
                }

                if (customer.VerificationDateTime == null || customer.VerificationCode == null)
                {
                    customer.VerificationCode = codeResetPassWord;
                    customer.VerificationDateTime = currentDate;
                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    dynamic resetPassWordSentat = customer.VerificationDateTime;
                    if (resetPassWordSentat != null)
                    {
                        TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);
                        if (timeConfirmResetPassWord.TotalMinutes <= 1)
                        {
                            if (customer.Language != null)
                            {
                                if (customer.Language.Equals("en"))
                                {
                                    return JsonUtil.Error("You are sending OTP continuously, please resend in 1 minute.");
                                }
                                else
                                {
                                    return JsonUtil.Error("Bạn đang gửi OTP liên tục, bạn vui lòng gửi lại sau 1 phút.");
                                }
                            }
                            else
                            {
                                return JsonUtil.Error("Bạn đang gửi OTP liên tục, bạn vui lòng gửi lại sau 1 phút.");
                            }

                        }
                        else
                        {
                            customer.VerificationCode = codeResetPassWord;
                            customer.VerificationDateTime = currentDate;
                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }

                if (customer.Language != null)
                {
                    if (customer.Language.Equals("en"))
                    {
                        string content = $"Your OTP is {codeResetPassWord}. This code will expire in 90 seconds. Note: You do not disclose this authentication code to anyone.";
                        if (!_accountService.SendOTPSOAPViettel(formatPhoneNumber, content))
                            return JsonUtil.Error("Something went wrong, please check again!");

                        if (sendAgain == 0)
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Sent confirmation of phone number change to " + customer.NewPhoneNumber);
                        }
                        else
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Sent OTP code to confirm phone number change to " + customer.NewPhoneNumber);
                        }
                        
                    }
                    else
                    {
                        string content = $"Ma OTP cua Anh/Chi la {codeResetPassWord}. Ma nay se het han trong 90 giay. Luu y: Anh/Chi khong tiet lo ma xac thuc nay cho bat ki ai.";
                        if (!_accountService.SendOTPSOAPViettel(formatPhoneNumber, content))
                            return JsonUtil.Error("Đã có lỗi xảy ra, vui lòng kiểm tra lại!");
                        if (sendAgain == 0)
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Đã gửi xác nhận đổi số điện thoại đến " + customer.NewPhoneNumber);
                        }
                        else
                        {
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Đã gửi lại mã OTP xác nhận đổi số điện thoại đến " + customer.NewPhoneNumber);
                        }
                        
                    }
                }
                else
                {
                    string content = $"Ma OTP cua Anh/Chi la {codeResetPassWord}. Ma nay se het han trong 90 giay. Luu y: Anh/Chi khong tiet lo ma xac thuc nay cho bat ki ai.";
                    if (!_accountService.SendOTPSOAPViettel(formatPhoneNumber, content))
                        return JsonUtil.Error("Đã có lỗi xảy ra, vui lòng kiểm tra lại!");
                    if (sendAgain == 0)
                    {
                        return JsonUtil.Success(new
                            {
                                OTP = codeResetPassWord,
                                CustomerId = customer.Id
                            }, $"Đã gửi xác nhận đổi số điện thoại đến " + customer.NewPhoneNumber);
                    }
                    else
                    {
                        return JsonUtil.Success(new
                            {
                                OTP = codeResetPassWord,
                                CustomerId = customer.Id
                            }, $"Đã gửi lại mã OTP xác nhận đổi số điện thoại đến " + customer.NewPhoneNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckCodeValidChangEmailOrPhoneCustomer(int customerId, string code, string type)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);

                if (customer.Language.Equals("en"))
                {
                    DateTime currentDate = DateTime.Now;
                    dynamic resetPassWordSentat = customer.VerificationDateTime;
                    TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);

                    if (timeConfirmResetPassWord.TotalMinutes > 1.5)
                    {
                        return JsonUtil.Error("The code has expired.");
                    }

                    if (!customer.VerificationCode.Equals(code))
                    {
                        return JsonUtil.Error("Code does not match");
                    }
                    else
                    {
                        if (type.ToLower().Equals("phone"))
                        {
                            customer.PhoneNumber = customer.NewPhoneNumber;
                            customer.NewPhoneNumber = "";

                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            _unitOfWork.Commit();
                            return JsonUtil.Success(customerId, "Change phone number successfully");
                        }
                        else
                        {
                            customer.Email = customer.NewEmail;
                            customer.NewEmail = "";

                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            _unitOfWork.Commit();
                            return JsonUtil.Success(customerId, "Change email successfully");
                        }
                        
                    }
                }
                else
                {
                    DateTime currentDate = DateTime.Now;
                    dynamic resetPassWordSentat = customer.VerificationDateTime;
                    TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);

                    if (timeConfirmResetPassWord.TotalMinutes > 1.5)
                    {
                        return JsonUtil.Error("Mã đã hết hạn sử dụng.");
                    }

                    if (!customer.VerificationCode.Equals(code))
                    {
                        return JsonUtil.Error("Mã không khớp");
                    }
                    else
                    {
                        if (type.ToLower().Equals("phone"))
                        {
                            customer.PhoneNumber = customer.NewPhoneNumber;
                            customer.NewPhoneNumber = "";
                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            _unitOfWork.Commit();
                            return JsonUtil.Success(customerId, "Đổi số điện thoại thành công");
                        }
                        else
                        {
                            customer.Email = customer.NewEmail;
                            customer.NewEmail = "";
                            _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                            _unitOfWork.Commit();
                            return JsonUtil.Success(customerId, "Đổi email thành công");
                        }
                        
                    }
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
