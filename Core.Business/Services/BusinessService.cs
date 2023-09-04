using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Business;
using Core.Business.ViewModels.Customer;
using Core.Business.ViewModels.User;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Security;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Enum = System.Enum;

namespace Core.Business.Services
{
    public class BusinessService : BaseService, IBusinessService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ILogActionService _logActionService;
        public IFileService _fileService;
        public INotifyService _notifyService;

        public BusinessService(
            ILogActionService logActionService,
            IFileService fileService,
            INotifyService notifyService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _logActionService = logActionService;
            _fileService = fileService;
            _notifyService = notifyService;
        }

        public JsonResult GetListBusiness(string keySearch, string province, string profession, string fieldOperation, int customerRole, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                if (!string.IsNullOrEmpty(province) || !string.IsNullOrWhiteSpace(province)) province = province.Trim();
                if (!string.IsNullOrEmpty(fieldOperation) || !string.IsNullOrWhiteSpace(fieldOperation)) fieldOperation = fieldOperation.Trim();
                var data = _unitOfWork.Repository<Proc_GetListBusiness>()
                    .ExecProcedure(Proc_GetListBusiness.GetEntityProc(keySearch, province, profession, fieldOperation, customerRole, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailBusinessAsync(int id)
        {
            try
            {

                var result = _unitOfWork.Repository<Proc_GetDetailBusiness>()
                    .ExecProcedure(Proc_GetDetailBusiness.GetEntityProc(id)).FirstOrDefault();
                
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailBusinessByCustomerIdAsync(int id)
        {
            try
            {
                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingleAsync(x => x.CustomerId == id && x.IsEnabled == true);
                var result = Mapper.Map<BusinessViewModel>(business);
                if (business.WardId.HasValue)
                {
                    result.DistrictId = _unitOfWork.RepositoryR<Wards>().GetSingle(x => x.Id == business.WardId).DistrictId;
                    result.ProvinceId = _unitOfWork.RepositoryR<Districts>().GetSingle(x => x.Id == result.DistrictId).ProvinceId;
                }

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }


        public async Task<JsonResult> GetDetailProfileBusinessAsync(int id)
        {
            try
            {
                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingleAsync(x => x.CustomerId == id && x.IsEnabled == true);
                var result = Mapper.Map<BusinessViewModel>(business);
                if (business.WardId.HasValue)
                {
                    result.DistrictId = _unitOfWork.RepositoryR<Wards>().GetSingle(x => x.Id == business.WardId).DistrictId;
                    result.ProvinceId = _unitOfWork.RepositoryR<Districts>().GetSingle(x => x.Id == result.DistrictId).ProvinceId;
                }

                result.ProfessionName = _unitOfWork.RepositoryR<Profession>()
                    .GetSingle(x => x.Id == business.ProfessionId).Name;
                if (business.FieldOperationsId != null)
                {
                    result.FieldOperationsName = _unitOfWork.RepositoryR<FieldOperations>()
                        .GetSingle(x => x.Id == business.FieldOperationsId).Name;
                }
                else
                {
                    result.FieldOperationsName = null;
                }
                
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateProfileBusinessAsync(BusinessViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Business.AddressNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessName) || string.IsNullOrWhiteSpace(model.BusinessName))
                    return JsonUtil.Error(ValidatorMessage.Business.NameNotEmpty);
                if (string.IsNullOrEmpty(model.TaxCode) || string.IsNullOrWhiteSpace(model.TaxCode))
                    return JsonUtil.Error(ValidatorMessage.Business.TaxCodeNotEmpty);
                if (string.IsNullOrEmpty(model.Position) || string.IsNullOrWhiteSpace(model.Position))
                    return JsonUtil.Error(ValidatorMessage.Business.PositionNotEmpty);

                model.Address = model.Address.Trim();
                model.BusinessName = model.BusinessName.Trim();
                model.TaxCode = model.TaxCode.Trim();
                model.Position = model.Position.Trim();

                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>().GetSingleAsync(x => x.Id == model.Id);
                if (!business.BusinessName.ToLower().Equals(model.BusinessName.ToLower()))
                    if (_unitOfWork.RepositoryR<Entity.Entities.Business>().Any(x => x.BusinessName.ToLower().Equals(model.BusinessName.ToLower())))
                        return JsonUtil.Error(ValidatorMessage.Business.UniqueName);
                if ((!string.IsNullOrEmpty(business.TaxCode) || !string.IsNullOrWhiteSpace(business.TaxCode)) && !business.TaxCode.ToLower().Equals(model.TaxCode.ToLower()))
                    if (_unitOfWork.RepositoryR<Entity.Entities.Business>().Any(x => x.TaxCode.ToLower().Equals(model.TaxCode.ToLower())))
                        return JsonUtil.Error(ValidatorMessage.Business.UniqueTaxCode);

                business.Address = model.Address;
                business.BusinessName = model.BusinessName;
                if (model.FieldOperationsId != 0)
                {
                    business.FieldOperationsId = model.FieldOperationsId;
                }
                else
                {
                    business.FieldOperationsId = null;
                }
                business.ProfessionId = model.ProfessionId;
                business.TaxCode = model.TaxCode;
                business.WardId = model.WardId;
                business.Description = model.Description;
                business.Position = model.Position;
                business.AvatarPath = model.AvatarPath;
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(business);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveBusinessAsync(int businessId)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == businessId);
                if (customer == null) return JsonUtil.Error(ValidatorMessage.Customer.NotExist);
                customer.IsActive = !customer.IsActive;

                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>().GetSingleAsync(x => x.CustomerId == businessId);
                if (business == null) return JsonUtil.Error(ValidatorMessage.Business.NotExist);
                business.IsActive = !business.IsActive;

                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);

                await _unitOfWork.CommitAsync();

                if (customer.IsActive == false)
                {
                    var notify = _notifyService.CreateNotifyWhenDeActiveCustomer(businessId);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnableBusinessAsync(int businessId)
        {
            try
            {
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == businessId);
                if (customer == null) return JsonUtil.Error(ValidatorMessage.Customer.NotExist);
                customer.IsEnabled = !customer.IsEnabled;
                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                await _unitOfWork.CommitAsync();

                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>().GetSingleAsync(x => x.CustomerId == businessId);
                if (business == null) return JsonUtil.Error(ValidatorMessage.Business.NotExist);
                business.IsEnabled = !business.IsEnabled;

                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                await _unitOfWork.CommitAsync();

                if (customer.IsEnabled == false)
                {
                    var notify = _notifyService.CreateNotifyWhenDeActiveCustomer(businessId);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DropdownCustomerRole()
        {
            try
            {
                List<CustomerRole> result = new List<CustomerRole>();
                CustomerRole total = new CustomerRole()
                {
                    Id = 0,
                    Name = "Tất cả",
                    IsEnabled = true,
                    Code = "Tất cả"
                };
                result = await _unitOfWork.RepositoryR<CustomerRole>().FindBy(x => x.IsEnabled == true)
                    .ToListAsync();
                result.Add(total);
                return JsonUtil.Success(result.OrderBy(x => x.Id));
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListBusinessPendingAsync(string keySearch, int status, int customerId, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListCustomerPending>()
                    .ExecProcedure(Proc_GetListCustomerPending.GetEntityProc(keySearch, status, customerId, pageNum, pageSize)).ToList();
                //if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                if (status == 0)
                {
                    if (!data.Any())
                    {
                        return JsonUtil.Success(new
                        {
                            LinkGroupChat = "",
                            result = data
                        }, "Success", 0);
                    }
                    else
                    {
                        return JsonUtil.Success(new
                        {
                            LinkGroupChat = "",
                            result = data
                        }, "Success", data.FirstOrDefault()?.Total);
                    }

                }
                else
                {
                    var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                        .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId;
                    var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);
                    if (!data.Any())
                    {
                        return JsonUtil.Success(new
                        {
                            LinkGroupChat = chapter.LinkGroupChat,
                            result = data
                        }, "Success", 0);
                    }
                    else
                    {
                        return JsonUtil.Success(new
                        {
                            LinkGroupChat = chapter.LinkGroupChat,
                            result = data
                        }, "Success", data.FirstOrDefault()?.Total);
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }


        public async Task<JsonResult> CreateBusinessAsync(BusinessViewModelCreateAdmin model, int userId)
        {
            try
            {
                if (model.CustomerRoleId == (int) EnumData.CustomerRoleEnum.FreeMember && model.ExpenseId != 0)
                    return JsonUtil.Error(ValidatorMessage.Customer.NotSetFreeMember);
                if (model.CustomerRoleId == (int) EnumData.CustomerRoleEnum.PremiumMember && model.ExpenseId == 0)
                    return JsonUtil.Error(ValidatorMessage.Customer.NotSetPremiumMember);
                if (model.ExpenseId != 0)
                {
                    var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == model.ExpenseId);
                    if (expense == null) return JsonUtil.Error("Gói bạn chọn vừa bị xoá, vui lòng chọn gói khác");
                    if (!expense.IsActive) return JsonUtil.Error("Gói bạn chọn vừa bị huỷ kích hoạt, vui lòng chọn gói khác");
                }


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
                if (string.IsNullOrEmpty(model.BusinessAddress) || string.IsNullOrWhiteSpace(model.BusinessAddress))
                    return JsonUtil.Error(ValidatorMessage.Business.AddressNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessName) || string.IsNullOrWhiteSpace(model.BusinessName))
                    return JsonUtil.Error(ValidatorMessage.Business.NameNotEmpty);
                if (string.IsNullOrEmpty(model.TaxCode) || string.IsNullOrWhiteSpace(model.TaxCode))
                    return JsonUtil.Error(ValidatorMessage.Business.TaxCodeNotEmpty);

                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.FullName = model.FullName.Trim();
                model.IdentityCard = model.IdentityCard.Trim();
                model.Address = model.Address.Trim();
                model.BusinessAddress = model.BusinessAddress.Trim();
                model.BusinessName = model.BusinessName.Trim();
                model.TaxCode = model.TaxCode.Trim();

                if(_unitOfWork.RepositoryR<Customer>().Any(x => x.PhoneNumber.ToLower().Equals(model.PhoneNumber.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Customer.UniquePhone);
                if (_unitOfWork.RepositoryR<Customer>().Any(x => x.Email.ToLower().Equals(model.Email.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmail);
                if (_unitOfWork.RepositoryR<Customer>().Any(x => x.IdentityCard.ToLower().Equals(model.IdentityCard.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Customer.UniqueIdCard);


                Customer cus = new Customer();
                cus.Id = 0;
                cus.Address = model.Address;
                cus.ProvinceId = model.ProvinceId;
                cus.Email = model.Email;
                cus.FullName = model.FullName;
                cus.PhoneNumber = model.PhoneNumber;
                cus.Gender = model.Gender;
                cus.IsActive = true;
                cus.IdentityCard = model.IdentityCard;
                cus.IsEnabled = true;
                cus.SecurityStamp = Guid.NewGuid().ToString();
                cus.PasswordHash = new Encryption().EncryptPassword(model.Password, cus.SecurityStamp);
                cus.IdentityCardPlace = model.IdentityCardPlaceId;
                cus.Birthday = model.Birthday.AddHours(7);
                cus.CustomerRoleId = model.CustomerRoleId;
                cus.SecurityStamp = Guid.NewGuid().ToString();
                cus.PasswordHash = new Encryption().EncryptPassword(model.Password, cus.SecurityStamp);
                cus.WardId = model.WardId; 
                cus.IdentityCardDate = model.IdentityCardDate;
                cus.StatusId = 1; //Free member
                _unitOfWork.RepositoryCRUD<Customer>().Insert(cus);
                _unitOfWork.Commit();

                Entity.Entities.Business business = new Entity.Entities.Business();
                business.Id = 0;
                business.Address = model.BusinessAddress;
                business.BusinessName = model.BusinessName;
                business.CustomerId = cus.Id;
                business.Position = model.Position;
                business.FieldOperationsId = model.FieldOperationsId;
                business.ProfessionId = model.ProfessionId;
                business.TaxCode = model.TaxCode;
                business.WardId = model.BusinessWardId;
                business.IsActive = true;
                business.IsEnabled = true;
                business.CustomerId = cus.Id;
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Insert(business);
                _unitOfWork.Commit();


                if (model.ExpenseId != 0)
                {
                    var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == model.ExpenseId);
                    if (expense == null) return JsonUtil.Error("Gói bạn chọn vừa bị xoá, vui lòng chọn gói khác");
                    if (!expense.IsActive) return JsonUtil.Error("Gói bạn chọn vừa bị huỷ kích hoạt, vui lòng chọn gói khác");

                    var membership = new MembershipAction();
                    membership.Id = 0;
                    membership.IsEnabled = true;
                    membership.CustomerId = cus.Id;
                    membership.ExtendDate = null;
                    membership.ExpenseId = model.ExpenseId;
                    membership.IsActive = true;
                    membership.EndDate = model.EndDate;

                    _unitOfWork.RepositoryCRUD<MembershipAction>().Insert(membership);
                    await _unitOfWork.CommitAsync();


                    var transaction = new Transaction();
                    transaction.CustomerId = cus.Id;
                    transaction.ExpenseId = model.ExpenseId;
                    transaction.StatusTransactionId = (int) EnumData.TransactionStatusEnum.Accepted;
                    transaction.Name = cus.FullName + " mua gói " + expense.Name;
                    transaction.DateActive = DateTime.Now;
                    transaction.Note = "Giao dịch được tạo từ admin";
                    _unitOfWork.RepositoryCRUD<Transaction>().Insert(transaction);
                    await _unitOfWork.CommitAsync();

                    var tmp = "";
                    for (int i = 0; i < (6 - transaction.Id.ToString().Length); i++)
                    {
                        tmp += "0";
                    }
                    var code = "PG" + String.Format("{0:MM}", transaction.CreatedWhen.GetValueOrDefault()) +
                               transaction.CreatedWhen.GetValueOrDefault().Year + tmp + transaction.Id;
                    transaction.Code = code;
                    _unitOfWork.RepositoryCRUD<Transaction>().Update(transaction);
                    await _unitOfWork.CommitAsync();
                }

                var value = (int) EnumData.LogActionEnum.Create;
                var action = ((EnumData.LogActionEnum) value).GetEnumDisplayName();
                var description = Extensions.GetDescription((EnumData.LogActionEnum)value);
                var logAction = await _logActionService.CreateLogAction(userId, action, description, null, cus.Id);
                var success = logAction.Value.GetType().GetProperty("isSuccess")?.GetValue(logAction.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return logAction;
                }
                return JsonUtil.Success(cus.Id);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateBusinessAsync(BusinessViewModelCreateAdmin model, int userId)
        {
            try
            {
                if (model.CustomerRoleId == (int)EnumData.CustomerRoleEnum.FreeMember && model.ExpenseId != 0)
                    return JsonUtil.Error(ValidatorMessage.Customer.NotSetFreeMember);

                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Customer.AddressNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessAddress) || string.IsNullOrWhiteSpace(model.BusinessAddress))
                    return JsonUtil.Error(ValidatorMessage.Business.AddressNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessName) || string.IsNullOrWhiteSpace(model.BusinessName))
                    return JsonUtil.Error(ValidatorMessage.Business.NameNotEmpty);
                if (string.IsNullOrEmpty(model.TaxCode) || string.IsNullOrWhiteSpace(model.TaxCode))
                    return JsonUtil.Error(ValidatorMessage.Business.TaxCodeNotEmpty);
                if (string.IsNullOrEmpty(model.Position) || string.IsNullOrWhiteSpace(model.Position))
                    return JsonUtil.Error(ValidatorMessage.Business.PositionNotEmpty);

                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.FullName = model.FullName.Trim();
                model.IdentityCard = model.IdentityCard.Trim();
                model.Address = model.Address.Trim();
                model.BusinessAddress = model.BusinessAddress.Trim();
                model.BusinessName = model.BusinessName.Trim();
                model.TaxCode = model.TaxCode.Trim();
                model.Position = model.Position.Trim();

                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>().GetSingleAsync(x => x.CustomerId == model.Id);
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
                cus.Gender = model.Gender;
                cus.IdentityCardPlace = model.IdentityCardPlaceId;
                cus.Birthday = model.Birthday.AddHours(7);
                if (model.ExpenseId != 0) cus.CustomerRoleId = (int) EnumData.CustomerRoleEnum.PremiumMember;
                cus.WardId = model.WardId;
                cus.IdentityCardDate = model.IdentityCardDate;
                cus.Description = model.Description;

                business.Address = model.BusinessAddress;
                business.BusinessName = model.BusinessName;
                business.FieldOperationsId = model.FieldOperationsId;
                business.ProfessionId = model.ProfessionId;
                business.TaxCode = model.TaxCode;
                business.Position = model.Position;
                business.WardId = model.BusinessWardId;
                business.Description = model.BusinessDescription;
                if (model.ParticipatingProvinceId != null) business.ParticipatingProvinceId = model.ParticipatingProvinceId;
                if (model.ParticipatingRegionId != null) business.ParticipatingRegionId = model.ParticipatingRegionId;
                if (model.ParticipatingChapterId != null)
                {
                    business.ParticipatingChapterId = model.ParticipatingChapterId;
                    cus.StatusId = (int) EnumData.CustomerStatusEnum.PendingChapter; // Sang chờ duyệt
                }

                _unitOfWork.RepositoryCRUD<Customer>().Update(cus);
                await _unitOfWork.CommitAsync();
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                await _unitOfWork.CommitAsync();

                if (model.ExpenseId != 0)
                {
                    if (model.MembershipId == 0)
                    {
                        var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == model.ExpenseId && x.IsActive == true);
                        if (expense == null) return JsonUtil.Error(ValidatorMessage.Expense.NotActiveOrEnable);

                        var membership = new MembershipAction();
                        membership.Id = 0;
                        membership.CustomerId = model.Id;
                        membership.ExtendDate = null;
                        membership.ExpenseId = model.ExpenseId;
                        membership.IsActive = true;
                        membership.EndDate = model.EndDate;

                        _unitOfWork.RepositoryCRUD<MembershipAction>().Insert(membership);
                        await _unitOfWork.CommitAsync();

                        var transaction = new Transaction();
                        transaction.CustomerId = model.Id;
                        transaction.ExpenseId = model.ExpenseId;
                        transaction.StatusTransactionId = (int) EnumData.TransactionStatusEnum.Accepted;
                        transaction.Name = cus.FullName + " mua gói " + expense.Name;
                        transaction.DateActive = DateTime.Now;
                        transaction.Note = "Giao dịch được tạo từ admin";
                        _unitOfWork.RepositoryCRUD<Transaction>().Insert(transaction);
                        await _unitOfWork.CommitAsync();

                        var tmp = "";
                        for (int i = 0; i < (6 - transaction.Id.ToString().Length); i++)
                        {
                            tmp += "0";
                        }
                        var code = "PG" + String.Format("{0:MM}", transaction.CreatedWhen.GetValueOrDefault()) +
                                   transaction.CreatedWhen.GetValueOrDefault().Year + tmp + transaction.Id;
                        transaction.Code = code;
                        _unitOfWork.RepositoryCRUD<Transaction>().Update(transaction);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        var membership = await _unitOfWork.RepositoryR<MembershipAction>()
                            .GetSingleAsync(x => x.Id == model.MembershipId);
                        membership.CustomerId = model.Id;
                        membership.ExtendDate = model.ExtendDate.GetValueOrDefault().AddHours(7);
                        membership.ExpenseId = model.ExpenseId;
                        membership.IsActive = true;
                        membership.IsEnabled = true;
                        //membership.EndDate = model.EndDate;
                        membership.CreatedWhen = model.CreatedWhen;

                        _unitOfWork.RepositoryCRUD<MembershipAction>().Update(membership);
                        await _unitOfWork.CommitAsync();
                    }
                }

                var value = (int) EnumData.LogActionEnum.Update;
                var action = ((EnumData.LogActionEnum)value).GetEnumDisplayName();
                var description = Extensions.GetDescription((EnumData.LogActionEnum)value);
                var logAction = await _logActionService.CreateLogAction(userId, action, description, null, model.Id);
                var success = logAction.Value.GetType().GetProperty("isSuccess")?.GetValue(logAction.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return logAction;
                }
                return JsonUtil.Success(cus.Id);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> SubscribeChapter(BusinessViewModelCreateMobile model)
        {
            try
            {
                var cus = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == model.Id);
                if (!_unitOfWork.RepositoryR<Chapter>()
                    .Any(x => x.Id == model.ParticipatingChapterId && x.IsActive == true))
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Customer.ChapterNotActiveEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Customer.ChapterNotActive);
                    }
                }

                var data = _unitOfWork.Repository<Proc_CheckUniqueFieldOperationsChapter>()
                    .ExecProcedure(Proc_CheckUniqueFieldOperationsChapter.GetEntityProc(model.ParticipatingChapterId.GetValueOrDefault(), model.FieldOperationsId)).ToList();
                if (data.Count > 0)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotSubscribeChapterEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotSubscribeChapter);
                    }
                    
                }

                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Customer.AddressNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessAddress) || string.IsNullOrWhiteSpace(model.BusinessAddress))
                    return JsonUtil.Error(ValidatorMessage.Business.AddressNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessName) || string.IsNullOrWhiteSpace(model.BusinessName))
                    return JsonUtil.Error(ValidatorMessage.Business.NameNotEmpty);
                if (string.IsNullOrEmpty(model.TaxCode) || string.IsNullOrWhiteSpace(model.TaxCode))
                    return JsonUtil.Error(ValidatorMessage.Business.TaxCodeNotEmpty);
                if (string.IsNullOrEmpty(model.Position) || string.IsNullOrWhiteSpace(model.Position))
                    return JsonUtil.Error(ValidatorMessage.Business.PositionNotEmpty);

                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.FullName = model.FullName.Trim();
                model.IdentityCard = model.IdentityCard.Trim();
                model.Address = model.Address.Trim();
                model.BusinessAddress = model.BusinessAddress.Trim();
                model.BusinessName = model.BusinessName.Trim();
                model.TaxCode = model.TaxCode.Trim();
                model.Position = model.Position.Trim();

                var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>().GetSingleAsync(x => x.CustomerId == model.Id);
                if ((!string.IsNullOrEmpty(cus.IdentityCard) || !string.IsNullOrWhiteSpace(cus.IdentityCard)) && !cus.IdentityCard.ToLower().Equals(model.IdentityCard.ToLower()))
                    if (_unitOfWork.RepositoryR<Customer>()
                        .Any(x => x.IdentityCard.ToLower().Equals(model.IdentityCard.ToLower())))
                    {
                        if (cus.Language.Equals("en"))
                        {
                            return JsonUtil.Error(ValidatorMessage.Customer.UniqueIdCardEnglish);
                        }
                        else
                        {
                            return JsonUtil.Error(ValidatorMessage.Customer.UniqueIdCard);
                        }
                    }

                cus.Address = model.Address;
                cus.ProvinceId = model.ProvinceId;
                cus.Gender = model.Gender;
                cus.IdentityCardPlace = model.IdentityCardPlace;
                cus.Birthday = model.Birthday;
                if (model.ExpenseId != 0) cus.CustomerRoleId = (int)EnumData.CustomerRoleEnum.PremiumMember;
                cus.WardId = model.WardId;
                cus.IdentityCardDate = model.IdentityCardDate;
                cus.IdentityCard = model.IdentityCard;
                cus.IdentityCard = model.IdentityCard;
                cus.Description = model.Description;

                business.Address = model.BusinessAddress;
                business.BusinessName = model.BusinessName;
                business.FieldOperationsId = model.FieldOperationsId;
                business.ProfessionId = model.ProfessionId;
                business.TaxCode = model.TaxCode;
                business.Position = model.Position;
                business.WardId = model.BusinessWardId;
                business.Description = model.BusinessDescription;
                business.ParticipatingProvinceId = model.ParticipatingProvinceId;
                business.ParticipatingRegionId = model.ParticipatingRegionId;
               
                business.ParticipatingChapterId = model.ParticipatingChapterId;
                cus.StatusId = (int)EnumData.CustomerStatusEnum.PendingChapter; // Sang chờ duyệt
                

                _unitOfWork.RepositoryCRUD<Customer>().Update(cus);
                await _unitOfWork.CommitAsync();
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(model.Id);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadAvatarBusiness(BusinessViewModelUploadAvatar model, int customerId)
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
                        "Business-Customer-" + customerId + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
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

        public JsonResult GenerateQrCodeCustomer(FileStreamResult streamResult, int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var QrPath = GenerateQrCode(streamResult, customerId.ToString(),
                    "QrCodeCustomer");

                customer.QrCodePath = QrPath;
                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public string GenerateQrCode(FileStreamResult streamResult, string fileName, string folderName)
        {
            try
            {
                string path = $@"{ApplicationEnvironment.ApplicationBasePath}{folderName}";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fullPath = Path.Combine(path, fileName + ".png");

                // string[] listFile = Directory.GetFiles(path);

                // foreach (var item in listFile)
                // {
                //     if (item == fullPath)
                //     {
                //         File.Delete(item);
                //     }
                // }

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    streamResult.FileStream.CopyTo(fileStream);
                }

                string result = folderName + '/' + fileName + ".png";

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public BusinessViewModelTextQrCode GetCustomer(int customerId)
        {
            var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
            var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                .GetSingle(x => x.CustomerId == customerId);
            var professionName = _unitOfWork.RepositoryR<Profession>().GetSingle(x => x.Id == business.ProfessionId).Name;
            var fieldOperationName = "";
            if (business.FieldOperationsId != null)
                fieldOperationName = _unitOfWork.RepositoryR<FieldOperations>()
                    .GetSingle(x => x.Id == business.FieldOperationsId).Name;
            string provinceName = "";
            if (customer.ProvinceId.HasValue)
                provinceName = _unitOfWork.RepositoryR<Provinces>().GetSingle(x => x.Id == customer.ProvinceId).Name;
            return new BusinessViewModelTextQrCode()
            {
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                BusinessName = business.BusinessName,
                ProvinceName = provinceName,
                ProfessionName = professionName,
                FieldOperationName = fieldOperationName
            };


        }

        public JsonResult ChangeProfessionBusiness(int customerId, int professionId, int fieldOperationsId)
        {
            try
            {
                var cus = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId);
                if (business.ParticipatingChapterId == null)
                {
                    business.ProfessionId = professionId;
                    business.FieldOperationsId = fieldOperationsId;

                    _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                    _unitOfWork.Commit();
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Success(true, "You have successfully changed your information");
                    }
                    else
                    {
                        return JsonUtil.Success(true, "Bạn đã thay đổi thông tin thành công");
                    }
                    
                }
                if (business.NewProfessionId != null || business.NewFieldOperationsId != null)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChangeEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChange);
                    }
                }
                business.NewProfessionId = professionId;
                if (fieldOperationsId == business.FieldOperationsId)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.UniqueFieldOperationsEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.UniqueFieldOperations);
                    }
                }
                var data = _unitOfWork.Repository<Proc_CheckUniqueFieldOperationsChapter>()
                    .ExecProcedure(Proc_CheckUniqueFieldOperationsChapter.GetEntityProc(business.ParticipatingChapterId.GetValueOrDefault(), fieldOperationsId)).ToList();
                if (data.Count > 0)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChangeFieldOperationsEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChangeFieldOperations);
                    }

                }
                business.NewFieldOperationsId = fieldOperationsId;

                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                _unitOfWork.Commit();
                if (cus.Language.Equals("en"))
                {
                    return JsonUtil.Success(true, "You have successfully requested to change information, please wait for approval");
                }
                else
                {
                    return JsonUtil.Success(true, "Bạn đã yêu cầu thay đổi thông tin thành công, vui lòng chờ duyệt");
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult ChangeFieldOperationsBusiness(int customerId, int fieldOperationsId)
        {
            try
            {
                var cus = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId);
                if (business.ParticipatingChapterId == null)
                {
                    business.FieldOperationsId = fieldOperationsId;

                    _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                    _unitOfWork.Commit();
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Success(true, "You have successfully changed your information");
                    }
                    else
                    {
                        return JsonUtil.Success(true, "Bạn đã thay đổi thông tin thành công");
                    }

                }
                if (business.NewProfessionId != null || business.NewFieldOperationsId != null)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChangeEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChange);
                    }
                }
                if (fieldOperationsId == business.FieldOperationsId)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.UniqueFieldOperationsEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.UniqueFieldOperations);
                    }
                }
                var data = _unitOfWork.Repository<Proc_CheckUniqueFieldOperationsChapter>()
                    .ExecProcedure(Proc_CheckUniqueFieldOperationsChapter.GetEntityProc(business.ParticipatingChapterId.GetValueOrDefault(), fieldOperationsId)).ToList();
                if (data.Count > 0)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChangeFieldOperationsEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Business.NotChangeFieldOperations);
                    }

                }
                business.NewFieldOperationsId = fieldOperationsId;

                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                _unitOfWork.Commit();
                if (cus.Language.Equals("en"))
                {
                    return JsonUtil.Success(true, "You have successfully requested to change information, please wait for approval");
                }
                else
                {
                    return JsonUtil.Success(true, "Bạn đã yêu cầu thay đổi thông tin thành công, vui lòng chờ duyệt");
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult ChangeProfessionAndFieldOperationsBusiness(int customerId, int active, string note, int currentId)
        {
            try
            {
                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId);

                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (active == 0) // Đồng ý
                {
                    if (business.NewProfessionId == null)
                    {
                        business.FieldOperationsId = business.NewFieldOperationsId;
                        business.NewFieldOperationsId = null;

                        var fieldOperations = _unitOfWork.RepositoryR<FieldOperations>()
                            .GetSingle(x => x.Id == business.FieldOperationsId);
                        if (string.IsNullOrEmpty(customer.Language) || customer.Language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.ChangeFieldOperations, fieldOperations.Name),
                                (int)EnumData.NotifyType.ChangeFieldOperations, fieldOperations.Id, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        else
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.ChangeFieldOperationsEnglish, fieldOperations.Code),
                                (int)EnumData.NotifyType.ChangeFieldOperations, fieldOperations.Id, null, null);
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
                        business.ProfessionId = business.NewProfessionId.GetValueOrDefault();
                        business.NewProfessionId = null;
                        business.FieldOperationsId = business.NewFieldOperationsId;
                        business.NewFieldOperationsId = null;

                        var profession = _unitOfWork.RepositoryR<Profession>()
                            .GetSingle(x => x.Id == business.ProfessionId);
                        if (string.IsNullOrEmpty(customer.Language) || customer.Language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.ChangeProfession, profession.Name),
                                (int)EnumData.NotifyType.ChangeProfession, profession.Id, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        else
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.ChangeProfessionEnglish, profession.Code),
                                (int)EnumData.NotifyType.ChangeProfession, profession.Id, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                    }
                }
                else // từ chối
                {
                    if (business.NewProfessionId == null)
                    {
                        if (string.IsNullOrEmpty(customer.Language) || customer.Language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.CancelChangeFieldOperations),
                                (int)EnumData.NotifyType.ChangeFieldOperations, business.NewFieldOperationsId.GetValueOrDefault(), currentId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        else
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.CancelChangeFieldOperationsEnglish),
                                (int)EnumData.NotifyType.ChangeFieldOperations, business.NewFieldOperationsId.GetValueOrDefault(), currentId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        business.NewFieldOperationsId = null;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(customer.Language) || customer.Language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.CancelChangeProfession),
                                (int)EnumData.NotifyType.ChangeProfession, business.NewProfessionId.GetValueOrDefault(), currentId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        else
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.CancelChangeProfessionEnglish),
                                (int)EnumData.NotifyType.ChangeProfession, business.NewProfessionId.GetValueOrDefault(), currentId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                        business.NewProfessionId = null;
                        business.NewFieldOperationsId = null;
                    }
                }
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Update(business);
                _unitOfWork.Commit();
                return JsonUtil.Success(customerId);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
