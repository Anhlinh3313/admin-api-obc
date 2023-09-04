using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.User;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class UserService : BaseService, IUserService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        private ISignalRHubService _signalRHub;
        private readonly IHubContext<SignalRHubService> _hubContext;

        public UserService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            ISignalRHubService signalRHub,
            IHubContext<SignalRHubService> hubContext,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _signalRHub = signalRHub;
            _hubContext = hubContext;
        }

        public async Task<JsonResult> CreateUserAsync(UserViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrWhiteSpace(model.UserName))
                    return JsonUtil.Error(ValidatorMessage.Account.UserNameNotEmpty);
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                    return JsonUtil.Error(ValidatorMessage.Account.PassWordNotEmpty);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrWhiteSpace(model.FullName))
                    return JsonUtil.Error(ValidatorMessage.Account.FullNameNotEmpty);
                if (model.RoleId == null || model.RoleId == 0)
                    JsonUtil.Error(ValidatorMessage.Account.RoleNotEmpty);
                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                    return JsonUtil.Error(ValidatorMessage.Account.PhonenumberNotEmpty);
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return JsonUtil.Error(ValidatorMessage.Account.EmailNotEmpty);
                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Account.AddressNotEmpty);

                model.UserName = model.UserName.Trim();
                model.Password = model.Password.Trim();
                model.FullName = model.FullName.Trim();
                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.Address = model.Address.Trim();

                if (_unitOfWork.RepositoryR<User>().Any(x => x.UserName.ToLower().Equals(model.UserName.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Account.UniqueUserName);
                if (_unitOfWork.RepositoryR<User>().Any(x => x.PhoneNumber.Equals(model.PhoneNumber)))
                    return JsonUtil.Error(ValidatorMessage.Account.UniquePhone);
                if (_unitOfWork.RepositoryR<User>().Any(x => x.Email.ToLower().Equals(model.Email.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Account.UniqueEmail);

                model.Code = model.UserName;
                model.IsActive = true;
                model.Birthday = model.Birthday.GetValueOrDefault().AddHours(7);;
                var result = await _iGeneralRawService.Create<User, UserViewModel>(model);
                return JsonUtil.Success(result);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error("Error " + ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailUserAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<User>().GetSingleAsync(x => x.Id == id);
                if (result == null) return JsonUtil.Error(ValidatorMessage.Account.NotExist);
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error("Error " + ex.Message);
            }
        }

        public JsonResult GetListUser(string keySearch, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch))
                    keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListUser>().ExecProcedure(Proc_GetListUser.GetEntityProc(keySearch, pageNum, pageSize)).ToList();
                if (data.Count <= 0) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success",data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error("Error " + ex.Message);
            }
        }

        public async Task<JsonResult> UpdateUserAsync(UserViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrWhiteSpace(model.UserName))
                    return JsonUtil.Error(ValidatorMessage.Account.UserNameNotEmpty);
                //if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                //    return JsonUtil.Error(ValidatorMessage.Account.PassWordNotEmpty);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrWhiteSpace(model.FullName))
                    return JsonUtil.Error(ValidatorMessage.Account.FullNameNotEmpty);
                if (model.RoleId == null || model.RoleId == 0)
                    JsonUtil.Error(ValidatorMessage.Account.RoleNotEmpty);
                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                    return JsonUtil.Error(ValidatorMessage.Account.PhonenumberNotEmpty);
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return JsonUtil.Error(ValidatorMessage.Account.EmailNotEmpty);
                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Account.AddressNotEmpty);

                model.UserName = model.UserName.Trim();
                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.Address = model.Address.Trim();
                model.FullName = model.FullName.Trim();

                var user = await _unitOfWork.RepositoryR<User>().GetSingleAsync(x => x.Id == model.Id && x.IsEnabled == true);
                if (!user.UserName.ToLower().Equals(model.UserName.ToLower()))
                    if (_unitOfWork.RepositoryR<User>().Any(x => x.UserName.ToLower().Equals(model.UserName.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Account.UniqueUserName);
                if (!user.PhoneNumber.Equals(model.PhoneNumber))
                    if (_unitOfWork.RepositoryR<User>().Any(x => x.PhoneNumber.Equals(model.PhoneNumber) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Account.UniquePhone);
                if (!user.Email.ToLower().Equals(model.Email.ToLower()))
                    if (_unitOfWork.RepositoryR<User>().Any(x => x.Email.ToLower().Equals(model.Email.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Account.UniqueEmail);

                model.Code = model.UserName;

                if (model.IsActive == null) model.IsActive = user.IsActive;
                model.IsEnabled = true;
                model.Birthday = model.Birthday.GetValueOrDefault().AddHours(7);
                var result = await _iGeneralRawService.Update<User, UserViewModel>(model);
                return JsonUtil.Success(result);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error("Error " + ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.RepositoryR<User>().GetSingleAsync(x => x.Id == userId);
                if (user == null) return JsonUtil.Error(ValidatorMessage.Account.NotExist);
                user.IsEnabled = !user.IsEnabled;
                _unitOfWork.RepositoryCRUD<User>().Update(user);
                await _unitOfWork.CommitAsync();

                if (!user.IsEnabled)
                {
                    var obj = new { title = "Thông báo tài khoản", Content = "Tài khoản của bạn đã bị xóa, vui lòng liên hệ admin để biết thêm chi tiết!" };

                    await _signalRHub.SendNotifications(userId.ToString(), "Xóa tài khoản", obj);

                }
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error("Error " + ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.RepositoryR<User>().GetSingleAsync(x => x.Id == userId);
                if (user == null) return JsonUtil.Error(ValidatorMessage.Account.NotExist);
                user.IsActive = !user.IsActive;
                _unitOfWork.RepositoryCRUD<User>().Update(user);
                await _unitOfWork.CommitAsync();
                if (!user.IsActive) 
                {
                    var obj = new { title = "Thông báo tài khoản", Content = "Tài khoản của bạn đã bị hủy kích hoạt, vui lòng liên hệ admin để biết thêm chi tiết!" };

                    await _signalRHub.SendNotifications(userId.ToString(), "Hủy kích hoạt", obj);

                }
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllRole()
        {
            try
            {
                var listRole = await _unitOfWork.RepositoryR<Role>().FindBy(x => x.RoleTypeId == 1 && x.IsEnabled == true).ToListAsync();
                return JsonUtil.Success(listRole);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckUser(int userId)
        {
            try
            {
                var user = _unitOfWork.RepositoryR<User>().GetSingle(x => x.IsActive == true && x.Id == userId);
                if (user == null)
                {
                    return JsonUtil.Success(false);
                }
                else
                {
                    return JsonUtil.Success(true);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
