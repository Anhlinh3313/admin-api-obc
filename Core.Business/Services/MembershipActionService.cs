using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.MembershipAction;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Expense = Core.Entity.Entities.Expense;

namespace Core.Business.Services
{
    public class MembershipActionService : BaseService, IMembershipActionService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IAccountService _accountService { get; set; }
        public MembershipActionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IAccountService accountService,
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _accountService = accountService;
        }

        public async Task<JsonResult> GetMembershipActionWithCustomerId(int id)
        {
            try
            {
                var membership = await _unitOfWork.RepositoryR<MembershipAction>()
                    .FindBy(x => x.CustomerId == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == id);
                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingleAsync(x => x.CustomerId == id);
                if (membership == null) return JsonUtil.Success(null, "Null");
                if (membership.EndDate < DateTime.Now && membership.ExtendDate == null)
                {
                    customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                    customer.RoleId = null;
                    customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    await _unitOfWork.CommitAsync();

                    membership.IsEnabled = false;
                    _unitOfWork.RepositoryCRUD<MembershipAction>().Update(membership);
                    await _unitOfWork.CommitAsync();

                    LogAction logAction = new LogAction()
                    {
                        ChapterId = business.ParticipatingChapterId,
                        CustomerId = id,
                        ActionName = "Hết hạn"
                    };
                    _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                    await _unitOfWork.CommitAsync();
                    return JsonUtil.Success(null, "Null");
                }
                if (membership.ExtendDate < DateTime.Now && membership.ExtendDate != null)
                {
                    customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                    customer.RoleId = null;
                    customer.CustomerRoleId = (int) EnumData.CustomerRoleEnum.FreeMember;
                    _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                    await _unitOfWork.CommitAsync();

                    membership.IsEnabled = false;
                    _unitOfWork.RepositoryCRUD<MembershipAction>().Update(membership);
                    await _unitOfWork.CommitAsync();

                    LogAction logAction = new LogAction()
                    {
                        ChapterId = business.ParticipatingChapterId,
                        CustomerId = id,
                        ActionName = "Hết hạn"
                    };
                    _unitOfWork.RepositoryCRUD<LogAction>().Insert(logAction);
                    await _unitOfWork.CommitAsync();
                    return JsonUtil.Success(null, "Null");
                }
                var expense = await _unitOfWork.RepositoryR<Expense>()
                    .GetSingleNotEnabledAsync(x => x.Id == membership.ExpenseId);
                bool isExtend = true;
                if (membership.ExtendDate != null) isExtend = false;
                if (customer.CustomerRoleId == (int) EnumData.CustomerRoleEnum.FreeMember)
                {
                    isExtend = false;
                }
                else
                {
                    var transaction = _unitOfWork.RepositoryR<Transaction>().FindBy(x => x.CustomerId == id)
                        .OrderByDescending(x => x.Id).FirstOrDefault();
                    if (transaction.StatusTransactionId == (int)EnumData.TransactionStatusEnum.PendingActive) isExtend = false;
                }

                if (membership.ExtendDate == null)
                {
                    return JsonUtil.Success(new
                    {
                        Id = membership.Id,
                        IsEnabled = membership.IsEnabled,
                        IsActive = membership.IsActive,
                        CreatedWhen = membership.CreatedWhen.GetValueOrDefault(),
                        CustomerId = membership.CustomerId,
                        ExpenseId = membership.ExpenseId,
                        ExpenseName = expense.Name,
                        EndDate = membership.EndDate,
                        Time = expense.Duration + " " + expense.TimeType,
                        ExtendDate = membership.ExtendDate,
                        IsExtend = isExtend
                    });
                }
                else
                {
                    return JsonUtil.Success(new
                    {
                        Id = membership.Id,
                        IsEnabled = membership.IsEnabled,
                        IsActive = membership.IsActive,
                        CreatedWhen = membership.CreatedWhen.GetValueOrDefault(),
                        CustomerId = membership.CustomerId,
                        ExpenseId = membership.ExpenseId,
                        ExpenseName = expense.Name,
                        EndDate = membership.ExtendDate,
                        Time = expense.Duration + " " + expense.TimeType,
                        ExtendDate = membership.ExtendDate,
                        IsExtend = isExtend
                    });
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetMembershipWithExpenseId(int id)
        {
            try
            {
                if (id == 0) return JsonUtil.Success(new {});
                var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == id);
                if (expense == null) return JsonUtil.Error("Gói bạn chọn vừa bị xoá, vui lòng chọn gói khác");
                if (!expense.IsActive) return JsonUtil.Error("Gói bạn chọn vừa bị huỷ kích hoạt, vui lòng chọn gói khác");
                var membership = new MembershipActionViewModel();
                membership.CreatedWhen = DateTime.Now;
                if (expense.TimeType.ToLower().Contains("ngày"))
                    membership.EndDate = membership.CreatedWhen.AddDays(expense.Duration);
                if (expense.TimeType.ToLower().Contains("tháng"))
                    membership.EndDate = membership.CreatedWhen.AddMonths(expense.Duration);
                if (expense.TimeType.ToLower().Contains("năm"))
                    membership.EndDate = membership.CreatedWhen.AddYears(expense.Duration);
                membership.Time = expense.Duration + " " + expense.TimeType;
                membership.ExpenseId = id;
                return JsonUtil.Success(membership);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateMembershipAction(MembershipActionViewModel model)
        {
            try
            {
                var membership = new MembershipAction();
                if (model.Id == 0)
                {
                    membership.Id = 0;
                    membership.CustomerId = model.CustomerId;
                    membership.ExtendDate = null;
                    membership.ExpenseId = model.ExpenseId;
                    membership.IsActive = true;
                    membership.EndDate = model.EndDate;

                    _unitOfWork.RepositoryCRUD<MembershipAction>().Insert(membership);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    membership.Id = model.Id;
                    membership.CustomerId = model.CustomerId;
                    membership.ExtendDate = model.ExtendDate;
                    membership.ExpenseId = model.ExpenseId;
                    membership.IsActive = true;
                    //membership.EndDate = model.EndDate;
                    membership.CreatedWhen = model.CreatedWhen;

                    _unitOfWork.RepositoryCRUD<MembershipAction>().Update(membership);
                    await _unitOfWork.CommitAsync();
                }

                return JsonUtil.Success(membership);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllCustomerExpired()
        {
            try
            {
                var membershipAction = _unitOfWork.RepositoryR<MembershipAction>().GetAll().ToList();
                foreach (var item in membershipAction)
                {
                    var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == item.CustomerId);
                    if (customer != null)
                    {
                        string formatPhoneNumber = customer.PhoneNumber.Substring(0, 1);
                        if (formatPhoneNumber == "0")
                        {
                            formatPhoneNumber = customer.PhoneNumber.Substring(1, customer.PhoneNumber.Length - 1);
                        }
                        else
                        {
                            formatPhoneNumber = customer.PhoneNumber;
                        }
                        var introduce = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault();
                        if (item.ExtendDate == null)
                        {
                            TimeSpan time = item.EndDate - DateTime.Now;
                            string body =
                                $"Quyen thanh vien cua Anh/Chi sap het han. Anh/Chi vui long thanh toan truoc ngay {item.EndDate.ToString("dd/MM/yyyy")}. Hotline ho tro: {introduce.PhoneNumber}";
                            if (time.Days == 30)
                            {
                                _accountService.SendMailCustomer(customer.Id, "Nhắc nhớ gia hạn quyền thành viên", body);
                                _accountService.SendOTPSOAPViettel(formatPhoneNumber, body);
                            }
                            else if (time.Days == 5)
                            {
                                _accountService.SendMailCustomer(customer.Id, "Nhắc nhớ gia hạn quyền thành viên", body);
                                _accountService.SendOTPSOAPViettel(formatPhoneNumber, body);
                            }
                        }
                        else
                        {
                            TimeSpan time = item.ExtendDate.GetValueOrDefault() - DateTime.Now;
                            string body =
                                $"Quyen thanh vien cua Anh/Chi sap het han. Anh/Chi vui long thanh toan truoc ngay {item.ExtendDate.GetValueOrDefault().ToString("dd/MM/yyyy")}. Hotline ho tro: {introduce.PhoneNumber}";
                            if (time.Days == 30)
                            {
                                _accountService.SendMailCustomer(customer.Id, "Nhắc nhớ gia hạn quyền thành viên", body);
                                _accountService.SendOTPSOAPViettel(formatPhoneNumber, body);
                            }
                            else if (time.Days == 5)
                            {
                                _accountService.SendMailCustomer(customer.Id, "Nhắc nhớ gia hạn quyền thành viên", body);
                                _accountService.SendOTPSOAPViettel(formatPhoneNumber, body);
                            }
                        }
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckMembershipActionExpired()
        {
            try
            {
                var membership = _unitOfWork.RepositoryR<MembershipAction>().GetAll().ToList();
                foreach (var item in membership)
                {
                    if (item.ExtendDate == null)
                    {
                        if (item.EndDate < DateTime.Now)
                        {
                            var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == item.CustomerId);
                            if (customer != null)
                            {
                                customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                                if (customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                                    customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                                customer.RoleId = null;
                                _unitOfWork.RepositoryCRUD<Customer>().UpdateNotCurrentUserId(customer);
                                _unitOfWork.Commit();

                                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                    .GetSingle(x => x.CustomerId == item.CustomerId);

                                item.IsEnabled = false;
                                _unitOfWork.RepositoryCRUD<MembershipAction>().UpdateNotCurrentUserId(item);
                                _unitOfWork.Commit();

                                LogAction logAction = new LogAction()
                                {
                                    ChapterId = business.ParticipatingChapterId,
                                    CustomerId = customer.Id,
                                    ActionName = "Hết hạn"
                                };
                                _unitOfWork.RepositoryCRUD<LogAction>().InsertNotCurrentUserId(logAction);
                                _unitOfWork.Commit();
                            }
                        }
                    }
                    else
                    {
                        if (item.ExtendDate < DateTime.Now)
                        {
                            var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == item.CustomerId);
                            if (customer != null)
                            {
                                customer.CustomerRoleId = (int)EnumData.CustomerRoleEnum.FreeMember;
                                if (customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                                    customer.StatusId = (int)EnumData.CustomerStatusEnum.AcceptedChapter;
                                customer.RoleId = null;
                                _unitOfWork.RepositoryCRUD<Customer>().UpdateNotCurrentUserId(customer);
                                _unitOfWork.Commit();

                                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                    .GetSingle(x => x.CustomerId == item.CustomerId);

                                item.IsEnabled = false;
                                _unitOfWork.RepositoryCRUD<MembershipAction>().UpdateNotCurrentUserId(item);
                                _unitOfWork.Commit();

                                LogAction logAction = new LogAction()
                                {
                                    ChapterId = business.ParticipatingChapterId,
                                    CustomerId = customer.Id,
                                    ActionName = "Hết hạn"
                                };
                                _unitOfWork.RepositoryCRUD<LogAction>().InsertNotCurrentUserId(logAction);
                                _unitOfWork.Commit();
                            }
                            
                        }
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
