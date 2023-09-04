using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Accounts;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Security;
using Core.Infrastructure.Utils;
using Core.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.Business.ViewModels.Business;
using Core.Business.ViewModels.Customer;
using System.Xml;
using System.IO;

namespace Core.Business.Services
{
    public partial class AccountService : BaseService, IAccountService
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IEncryptionService _iEncryptionService;
        private readonly SendMail _iSendMail;
        private readonly SendMailOTP _iSendMailOTP;
        private readonly SendMailGuests _iSendMailGuests;
        public IGeneralService _igeneralRawService { get; }

        public AccountService(
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IUnitOfWork unitOfWork,
            IOptions<JwtIssuerOptions> jwtOptions,
            IGeneralService igeneralRawService,
            IOptions<SendMail> sendMail,
            IOptions<SendMailOTP> sendMailOTP,
            IOptions<SendMailGuests> sendMailGuests,
            IEncryptionService iEncryptionService) : base(logger, optionsAccessor, unitOfWork)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
            _iEncryptionService = iEncryptionService;
            _igeneralRawService = igeneralRawService;
            _iSendMail = sendMail.Value;
            _iSendMailOTP = sendMailOTP.Value;
            _iSendMailGuests = sendMailGuests.Value;
        }
        public User VerifyToken(int userId)
        {
            var usersadmin = _unitOfWork.RepositoryR<User>().GetSingle(x => x.Id == userId);
            // Check if ID doesn't exist
            if (usersadmin == null)
                return null;

            // Get user's ID successfully
            return usersadmin;
        }
        //public async Task<dynamic> CreateAccount(CreateAccountViewModel model)
        //{
        //    User user = Mapper.Map<User>(model);
        //    _unitOfWork.RepositoryCRUD<User>().InsertAndUpdate(user);
        //    await _unitOfWork.CommitAsync();
        //    return JsonUtil.Success(ConvertUserToUserInfoViewModel(user));
        //}

        //public async Task<dynamic> UpdateAccount(UpdateAccountViewModel model)
        //{
        //    User user = GetUser(model.Id);
        //    user = Mapper.Map(model, user);
        //    _unitOfWork.RepositoryCRUD<User>().Update(user);
        //    await _unitOfWork.CommitAsync();
        //    return JsonUtil.Success(ConvertUserToUserInfoViewModel(user));
        //}

        //public async Task<dynamic> ChangePassWord(ChangePassWordViewModel model)
        //{
        //    User user = GetUser(model.UserId);
        //    if (!Util.IsNull(user.IsPassWordBasic))
        //    {
        //        user.IsPassWordBasic = false;
        //    }
        //    user = Mapper.Map(model, user);
        //    _unitOfWork.RepositoryCRUD<User>().Update(user);
        //    await _unitOfWork.CommitAsync();
        //    return JsonUtil.Success(ConvertUserToUserInfoViewModel(user));
        //}

        //public async Task<dynamic> DeleteAccount(BasicViewModel model)
        //{
        //    User user = GetUser(model.Id);

        //    if (user != null)
        //    {
        //        user.IsEnabled = false;
        //        _unitOfWork.RepositoryCRUD<User>().Update(user);
        //        await _unitOfWork.CommitAsync();
        //        return JsonUtil.Success();
        //    }

        //    return JsonUtil.Error(ValidatorMessage.Account.NotExist);

        //}

        //public dynamic GetAccountInfo(int id)
        //{
        //    var user = GetUser(id);

        //    if (user != null)
        //    {
        //        return JsonUtil.Success(ConvertUserToUserInfoViewModel(user));
        //    }

        //    return JsonUtil.Error(ValidatorMessage.Account.NotExist);
        //}

        //public dynamic GetAccountList()
        //{
        //    return JsonUtil.Success(Mapper.Map<IEnumerable<UserInfoViewModel>>(_unitOfWork.RepositoryR<User>().FindBy(x => !x.IsEnabled && x.IsEnabled)));
        //}

        public async Task<JsonResult> SignIn(SignInViewModel model)
        {
            try
            {
                var checkExists = _unitOfWork.RepositoryCRUD<User>().AnyNotIsEnabled(x => x.UserName == (model.UserName) && x.IsEnabled != false);
                if (!checkExists) return JsonUtil.Error(ValidatorMessage.Account.NotExist);

                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrWhiteSpace(model.UserName))
                    return JsonUtil.Error(ValidatorMessage.Account.UserNameNotEmpty);
                if (string.IsNullOrEmpty(model.PassWord) || string.IsNullOrWhiteSpace(model.PassWord))
                    return JsonUtil.Error(ValidatorMessage.Account.PassWordNotEmpty);

                DateTime datetime = DateTime.Now;
                model.HourLogin = datetime.Hour;
                model.MinuteLogin = datetime.Minute;

                var user = _unitOfWork.RepositoryCRUD<User>().GetSingle(x => x.UserName == (model.UserName));
                if (user == null)
                {
                    return JsonUtil.Error(ValidatorMessage.Account.InvalidUserName);
                }

                if (!user.IsActive)
                    return JsonUtil.Error(ValidatorMessage.Account.DeActiveAccount);
                var checkPass = _iEncryptionService.EncryptPassword(model.PassWord, user.SecurityStamp);
                if (user.PasswordHash != checkPass)
                {
                    return JsonUtil.Error(ValidatorMessage.Account.InvalidPassWord);
                }
                var role = await _unitOfWork.RepositoryR<Role>().GetSingleNotEnabledAsync(x => x.Id == user.RoleId);
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.CHash, _iEncryptionService.HashSHA256(user.SecurityStamp)),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            };

                // Create the JWT security token and encode it.
                var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // Serialize and return the response
                return JsonUtil.Success(new SignInViewModelResponse()
                {
                    UserId = user.Id.ToString(),
                    UserName = user.UserName,
                    UserFullName = user.FullName,
                    RoleName = role.Name,
                    Token = encodedJwt,
                    Expires = (int)_jwtOptions.ValidFor.TotalSeconds,
                    ExpiresDate = DateTime.Now.AddDays(_jwtOptions.ValidFor.Days)
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error("Error " + ex);
            }
        }

        private User GetUser(int id)
        {
            return _unitOfWork.RepositoryR<User>().GetSingle(x => x.Id == id);
        }

        public async Task<JsonResult> SignInMobile(SignInMobileViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EmailOrPhone) || string.IsNullOrWhiteSpace(model.EmailOrPhone))
                    return JsonUtil.Error(ValidatorMessage.Account.NotEmpty);
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                    return JsonUtil.Error(ValidatorMessage.Account.PassWordNotEmpty);
                var checkExists = _unitOfWork.RepositoryCRUD<Customer>().AnyNotIsEnabled(x => (x.Email == model.EmailOrPhone || x.PhoneNumber == model.EmailOrPhone) && x.IsEnabled != false);

                if (model.Language != null)
                {
                    if (model.Language.Equals("en"))
                    {
                        if (!checkExists) return JsonUtil.Error(ValidatorMessage.Account.NotExistEnglish);
                    }
                    else
                    {
                        if (!checkExists) return JsonUtil.Error(ValidatorMessage.Account.NotExist);
                    }
                }
                else
                {
                    if (!checkExists) return JsonUtil.Error(ValidatorMessage.Account.NotExist);
                }


                DateTime datetime = DateTime.Now;
                model.HourLogin = datetime.Hour;
                var user = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Email.ToLower().Equals(model.EmailOrPhone.ToLower()) || x.PhoneNumber == model.EmailOrPhone);
                if (model.Language != null)
                {
                    if (model.Language.Equals("en"))
                    {
                        if (user == null)
                        {
                            return JsonUtil.Error(ValidatorMessage.Account.InvalidUserNameEnglish);
                        }
                        if (!user.IsActive)
                            return JsonUtil.Error(ValidatorMessage.Account.DeActiveAccountEnglish);
                    }
                    else
                    {
                        if (user == null)
                        {
                            return JsonUtil.Error(ValidatorMessage.Account.InvalidUserName);
                        }
                        if (!user.IsActive)
                            return JsonUtil.Error(ValidatorMessage.Account.DeActiveAccount);
                    }
                }
                else
                {
                    if (user == null)
                    {
                        return JsonUtil.Error(ValidatorMessage.Account.InvalidUserName);
                    }
                    if (!user.IsActive)
                        return JsonUtil.Error(ValidatorMessage.Account.DeActiveAccount);
                }

                
                var checkPass = _iEncryptionService.EncryptPassword(model.Password, user.SecurityStamp);

                if (model.Language != null)
                {
                    if (model.Language.Equals("en"))
                    {
                        if (user.PasswordHash != checkPass)
                        {
                            return JsonUtil.Error(ValidatorMessage.Account.InvalidPassWordEnglish);
                        }
                    }
                    else
                    {
                        if (user.PasswordHash != checkPass)
                        {
                            return JsonUtil.Error(ValidatorMessage.Account.InvalidPassWord);
                        }
                    }
                }
                else
                {
                    if (user.PasswordHash != checkPass)
                    {
                        return JsonUtil.Error(ValidatorMessage.Account.InvalidPassWord);
                    }
                }

                
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.CHash, _iEncryptionService.HashSHA256(user.SecurityStamp)),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            };

                // Create the JWT security token and encode it.
                var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // Serialize and return the response
                return JsonUtil.Success(new
                {
                    userId = user.Id.ToString(),
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    userFullName = user.FullName,
                    token = encodedJwt,
                    expires = (int)_jwtOptions.ValidFor.TotalSeconds,
                    expiresDate = DateTime.Now.AddDays(_jwtOptions.ValidFor.Days)
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public  JsonResult SignUpMobile(SignUpMobileViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                    return JsonUtil.Error(ValidatorMessage.Customer.PhoneNumberNotEmpty);
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return JsonUtil.Error(ValidatorMessage.Customer.EmailNotEmpty);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrWhiteSpace(model.FullName))
                    return JsonUtil.Error(ValidatorMessage.Customer.FullNameNotEmpty);
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password))
                    return JsonUtil.Error(ValidatorMessage.Customer.PassWordNotEmpty);
                if (string.IsNullOrEmpty(model.BusinessName) || string.IsNullOrWhiteSpace(model.BusinessName))
                    return JsonUtil.Error(ValidatorMessage.Business.NameNotEmpty);
                if (string.IsNullOrEmpty(model.Position) || string.IsNullOrWhiteSpace(model.Position))
                    return JsonUtil.Error(ValidatorMessage.Business.PositionNotEmpty);

                model.BusinessName = model.BusinessName.Trim();
                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.FullName = model.FullName.Trim();
                model.Position = model.Position.Trim();
                model.Password = model.Password.Trim();

                if (_unitOfWork.RepositoryR<Customer>()
                    .Any(x => x.PhoneNumber.ToLower().Equals(model.PhoneNumber.ToLower())))
                {
                    if (model.Language != null)
                    {
                        if (model.Language.Equals("en"))
                        {
                            return JsonUtil.Error(ValidatorMessage.Customer.UniquePhoneEnglish);
                        }
                        else
                        {
                            return JsonUtil.Error(ValidatorMessage.Customer.UniquePhone);
                        }
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Customer.UniquePhone);
                    }
                }


                if (_unitOfWork.RepositoryR<Customer>().Any(x => x.Email.ToLower().Equals(model.Email.ToLower())))
                {
                    if (model.Language != null)
                    {
                        if (model.Language.Equals("en"))
                        {
                            return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmailEnglish);
                        }
                        else
                        {
                            return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmail);
                        }
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmail);
                    }
                    
                }
                    

                CustomerViewModelCreate cus = new CustomerViewModelCreate()
                {
                    Id = 0,
                    ProvinceId = model.ProvinceId,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    IsEnabled = true,
                    Birthday = model.Birthday,
                    Gender = model.Gender,
                    IsActive = true,
                    CustomerRoleId = 2
                };
                Customer customer = Mapper.Map<Customer>(cus);
                customer.Id = 0;
                customer.SecurityStamp = Guid.NewGuid().ToString();
                customer.PasswordHash = new Encryption().EncryptPassword(cus.Password, customer.SecurityStamp);
                customer.StatusId = 1; //Free Member
                _unitOfWork.RepositoryCRUD<Customer>().Insert(customer);
                _unitOfWork.Commit();
                
                Entity.Entities.Business businessViewModelCreate = new Entity.Entities.Business()
                {
                    Id = 0,
                    Position = model.Position,
                    BusinessName = model.BusinessName,
                    IsEnabled = true,
                    IsActive = true,
                    ProfessionId = model.ProfessionId,
                    CustomerId = customer.Id
                };
                _unitOfWork.RepositoryCRUD<Entity.Entities.Business>().Insert(businessViewModelCreate);
                _unitOfWork.Commit();
                return JsonUtil.Success(customer);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        //private UserInfoViewModel ConvertUserToUserInfoViewModel(User user)
        //{
        //    return Mapper.Map<UserInfoViewModel>(user);
        //}

        //public ResponseViewModel UpdateUserSystem(UpdateAccountViewModel viewModel)
        //{
        //    User user = _unitOfWork.RepositoryR<User>().GetSingle(viewModel.Id);
        //    if (user == null)
        //    {
        //        return ResponseViewModel.CreateError("Tài khoản không tồn tại");
        //    }

        //    if (viewModel.PassWord != null)
        //    {
        //        user.PasswordHash = new Encryption().EncryptPassword(viewModel.PassWord, user.SecurityStamp);
        //    }

        //    user.Code = viewModel.Code;
        //    user.Name = viewModel.Name;
        //    user.PhoneNumber = viewModel.PhoneNumber;
        //    user.Email = viewModel.Email;
        //    user.DepartmentId = viewModel.DepartmentId;
        //    user.Address = viewModel.Address;
        //    user.ProvinceId = viewModel.ProvinceId;
        //    user.DistrictId = viewModel.DistrictId;
        //    user.WardId = viewModel.WardId;
        //    user.UserTypeId = viewModel.UserTypeId;
        //    user.IdentityCard = viewModel.IdentityCard;
        //    user.BirthDate = viewModel.BirthDate;
        //    user.Gender = viewModel.Gender;
        //    user.IsBlocked = viewModel.IsBlocked;
        //    user.AvatarPath = viewModel.AvatarPath;
        //    user.BranchId = viewModel.BranchId;
        //    _unitOfWork.RepositoryCRUD<User>().Update(user);
        //    _unitOfWork.Commit();

        //    if (viewModel.RoleIds != null && viewModel.RoleIds.Count > 0)
        //    {
        //        _unitOfWork.RepositoryCRUD<UserRole>().DeleteWhere(r => r.UserId == viewModel.Id);
        //        foreach (int roleId in viewModel.RoleIds)
        //        {
        //            UserRole userRole = new UserRole();
        //            userRole.UserId = user.Id;
        //            userRole.RoleId = roleId;
        //            _unitOfWork.RepositoryCRUD<UserRole>().Insert(userRole);
        //        }
        //        _unitOfWork.Commit();

        //    }

        //    return ResponseViewModel.CreateSuccess(user);
        //}

        //public ResponseViewModel CreateUser(CreateAccountViewModel viewModel)
        //{
        //    viewModel.IsPassWordBasic = true;
        //    User user = Mapper.Map<User>(viewModel);
        //    _unitOfWork.RepositoryCRUD<User>().Insert(user);
        //    _unitOfWork.Commit();

        //    if (viewModel.RoleIds != null && viewModel.RoleIds.Count > 0)
        //    {
        //        _unitOfWork.RepositoryCRUD<UserRole>().DeleteWhere(r => r.UserId == viewModel.Id);

        //        foreach (int roleId in viewModel.RoleIds)
        //        {
        //            UserRole userRole = new UserRole();
        //            userRole.UserId = user.Id;
        //            userRole.RoleId = roleId;
        //            _unitOfWork.RepositoryCRUD<UserRole>().Insert(userRole);
        //        }
        //        _unitOfWork.Commit();
        //    }


        //    return ResponseViewModel.CreateSuccess(user);
        //}

        public JsonResult SendMailForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = _unitOfWork.RepositoryR<User>().GetSingle(x => x.Email.Equals(model.Email));

                if (user != null)
                {
                    DateTime currentDate = DateTime.Now;
                    var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();
                    //var codeResetPassWord = _iEncryptionService.HashSHA256(code.ToString());
                    if (user.VerificationDateTime == null || user.VerificationCode == null)
                    {
                        user.VerificationCode = codeResetPassWord;
                        user.VerificationDateTime = currentDate;
                        _unitOfWork.RepositoryCRUD<User>().Update(user);
                        _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        dynamic resetPassWordSentat = user.VerificationDateTime;
                        if (resetPassWordSentat != null)
                        {
                            TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);
                            if (timeConfirmResetPassWord.TotalMinutes <= 1)
                            {
                                
                                return JsonUtil.Error("Bạn đang gửi mail liên tục, bạn vui lòng gửi lại sau 1 phút.");
                                
                                
                            }
                            else
                            {
                                user.VerificationCode = codeResetPassWord;
                            user.VerificationDateTime = currentDate;
                            _unitOfWork.RepositoryCRUD<User>().Update(user);
                            _unitOfWork.CommitAsync();
                            }
                        }
                    }

                    var emailRecipient = new EmailRecipient(
                           user.Id,
                           user.Email,
                           user.FullName,
                           user.VerificationCode, user.PhoneNumber, ""
                       );
                    
                    var result = SendEmail(_iSendMail, emailRecipient, " Quên mật khẩu ");
                    return JsonUtil.Success(user.Id, $"Đã gửi xác nhận đổi mật khẩu đến " + user.Email);
                    
                   
                }
                else
                {
                    return JsonUtil.Error("Email không tồn tại");
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SendMailForgotPasswordMobile(ForgotPasswordMobileViewModel model)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Email.Equals(model.Email));

                if (customer != null)
                {
                    DateTime currentDate = DateTime.Now;
                    var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();
                    //var codeResetPassWord = _iEncryptionService.HashSHA256(code.ToString());
                    if (customer.VerificationDateTime == null || customer.VerificationCode == null)
                    {
                        customer.VerificationCode = codeResetPassWord;
                        customer.VerificationDateTime = currentDate;
                        _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                        _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        dynamic resetPassWordSentat = customer.VerificationDateTime;
                        if (resetPassWordSentat != null)
                        {
                            TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);
                            if (timeConfirmResetPassWord.TotalMinutes <= 1)
                            {
                                if(model.Language != null)
                                {
                                    if (model.Language.Equals("en"))
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
                        customer.Email,
                        customer.FullName,
                        customer.VerificationCode, customer.PhoneNumber, ""
                       );

                    if (model.Language != null)
                    {
                        if (model.Language.Equals("en"))
                        {
                            var result = SendEmail(_iSendMail, emailRecipient, " Forgot password ");
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Sent confirmation of password change to " + customer.Email);
                        }
                        else
                        {
                            var result = SendEmail(_iSendMail, emailRecipient, " Quên mật khẩu ");
                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Đã gửi xác nhận đổi mật khẩu đến " + customer.Email);
                        }
                    }
                    else
                    {
                        var result = SendEmail(_iSendMail, emailRecipient, " Quên mật khẩu ");
                        return JsonUtil.Success(new
                            {
                                OTP = codeResetPassWord,
                                CustomerId = customer.Id
                            }, $"Đã gửi xác nhận đổi mật khẩu đến " + customer.Email);
                    }

                }
                else
                {
                    if (model.Language != null)
                    {
                        if (model.Language.Equals("en"))
                        {
                            return JsonUtil.Error("Email does not exist");
                        }
                        else
                        {
                            return JsonUtil.Error("Email không tồn tại");
                        }
                    }
                    else
                    {
                        return JsonUtil.Error("Email không tồn tại");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public bool SendOTPSOAPViettel(string phoneNumber, string content)
        {
            var _url = "http://apismsbrand.viettel.vn:8998/bulkapi";
            var _action = "";
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(phoneNumber, content);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            string error = soapResult.Split("<result>")[1].Substring(0, 1);
            if (error == "0")
                return false;
            return true;

        }

        public JsonResult SendMailGuests(int guestsId, string content, string body)
        {
            try
            {
                var guests = _unitOfWork.RepositoryR<Guests>().GetSingle(x => x.Id == guestsId);
                if (guests != null)
                {
                    var emailRecipient = new EmailRecipient(
                        guests.Id,
                        guests.Email,
                        guests.Name,
                        "", guests.PhoneNumber, ""
                    );
                    
                    
                    var result = SendEmailWithBody(_iSendMailGuests, emailRecipient, content, body);
                    return JsonUtil.Success(true, $"Đã gửi xác nhận đến khách mời " + guests.Name);
                }
                else
                {
                    return JsonUtil.Error("Email không tồn tại");
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SendMailCustomer(int customerId, string content, string body)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (customer != null)
                {
                    var emailRecipient = new EmailRecipient(
                        customer.Id,
                        customer.Email,
                        customer.FullName,
                        "", customer.PhoneNumber, ""
                    );


                    var result = SendEmailWithBody(_iSendMailGuests, emailRecipient, content, body);
                    return JsonUtil.Success(true, "Đã gửi xác nhận đến email " + customer.Email);
                }
                else
                {
                    return JsonUtil.Error("Email không tồn tại");
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> SendOTPByPhoneNumberForgotPass(string phoneNumber, string language)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.PhoneNumber.Equals(phoneNumber));

                if (customer != null)
                {
                    DateTime currentDate = DateTime.Now;
                    var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();

                    string formatPhoneNumber = phoneNumber.Substring(0, 1);
                    if(formatPhoneNumber == "0")
                    {
                        formatPhoneNumber = phoneNumber.Substring(1, phoneNumber.Length - 1);
                    }
                    else
                    {
                        formatPhoneNumber = phoneNumber;
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
                                if (language != null)
                                {
                                    if (language.Equals("en"))
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

                    if(language != null)
                    {
                        if (language.Equals("en"))
                        {
                            string content = $"Your OTP is {codeResetPassWord}. This code will expire in 90 seconds. Note: You do not disclose this authentication code to anyone.";
                            if (!SendOTPSOAPViettel(formatPhoneNumber, content))
                                return JsonUtil.Error("Something went wrong, please check again!");

                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Sent confirmation of password change to " + customer.PhoneNumber);
                        }
                        else
                        {
                            string content = $"Ma OTP cua Anh/Chi la {codeResetPassWord}. Ma nay se het han trong 90 giay. Luu y: Anh/Chi khong tiet lo ma xac thuc nay cho bat ki ai.";
                            if (!SendOTPSOAPViettel(formatPhoneNumber, content))
                                return JsonUtil.Error("Đã có lỗi xảy ra, vui lòng kiểm tra lại!");

                            return JsonUtil.Success(new
                                {
                                    OTP = codeResetPassWord,
                                    CustomerId = customer.Id
                                }, $"Đã gửi xác nhận đổi mật khẩu đến " + customer.PhoneNumber);
                        }
                    }
                    else
                    {
                        string content = $"Ma OTP cua Anh/Chi la {codeResetPassWord}. Ma nay se het han trong 90 giay. Luu y: Anh/Chi khong tiet lo ma xac thuc nay cho bat ki ai.";
                        if (!SendOTPSOAPViettel(formatPhoneNumber, content))
                            return JsonUtil.Error("Đã có lỗi xảy ra, vui lòng kiểm tra lại!");

                        return JsonUtil.Success(new
                            {
                                OTP = codeResetPassWord,
                                CustomerId = customer.Id
                            }, $"Đã gửi xác nhận đổi mật khẩu đến " + customer.PhoneNumber);

                    }

                    
                }
                else
                {
                    if(language != null)
                    {
                        if (language.Equals("en"))
                        {
                            return JsonUtil.Error("Phone number does not exist");
                        }
                        else
                        {
                            return JsonUtil.Error("Số điện thoại không tồn tại");
                        }
                    }
                    else
                    {
                        return JsonUtil.Error("Số điện thoại không tồn tại");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        private static XmlDocument CreateSoapEnvelope(string phoneNumber, string content)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:impl=""http://impl.bulkSms.ws/"">
                    " + "\n" +
                    @"    <soapenv:Header/>
                    " + "\n" +
                    @"    <soapenv:Body>
                    " + "\n" +
                    @"        <impl:wsCpMt>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <User>obc</User>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <Password>123456aA@</Password>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <CPCode>OBC</CPCode>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <RequestID>1</RequestID>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <UserID>84" + phoneNumber + @"</UserID>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <ReceiverID>84" + phoneNumber + @"</ReceiverID>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <ServiceID>OBC</ServiceID>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <CommandCode>bulksms</CommandCode>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <Content>" + content + @"</Content>
                    " + "\n" +
                    @"            <!--Optional:-->
                    " + "\n" +
                    @"            <ContentType>0</ContentType>
                    " + "\n" +
                    @"        </impl:wsCpMt>
                    " + "\n" +
                    @"    </soapenv:Body>
                    " + "\n" +
                    @"</soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        public JsonResult SendMailOTP(ForgotPasswordViewModel model)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();
                var emailRecipient = new EmailRecipient(
                        0,
                        model.Email,
                        "",
                        codeResetPassWord, "", ""
                    );
                if(model.Language != null)
                {
                    if (model.Language.Equals("vi") || string.IsNullOrEmpty(model.Language))
                    {
                        var result = SendEmail(_iSendMailOTP, emailRecipient, " Mã xác nhận OTP ");
                        DataResponse data = new DataResponse(codeResetPassWord, currentDate);
                        return JsonUtil.Success(data, $"Đã gửi mã xác nhận đến " + model.Email);
                    }
                    else
                    {
                        var result = SendEmail(_iSendMailOTP, emailRecipient, " Verification code OTP ");
                        DataResponse data = new DataResponse(codeResetPassWord, currentDate);
                        return JsonUtil.Success(data, $"Verification code sent to " + model.Email);
                    }
                }
                else
                {
                    var result = SendEmail(_iSendMailOTP, emailRecipient, " Mã xác nhận OTP ");
                    DataResponse data = new DataResponse(codeResetPassWord, currentDate);
                    return JsonUtil.Success(data, $"Đã gửi mã xác nhận đến " + model.Email);
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SendOTPByPhoneNumberRegister(string phoneNumber, string language)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                var codeResetPassWord = RandomUtil.RandomNumber(1000, 9999).ToString();
                string formatPhoneNumber = phoneNumber.Substring(0, 1);

                if (formatPhoneNumber == "0")
                {
                    formatPhoneNumber = phoneNumber.Substring(1, phoneNumber.Length - 1);
                }
                else
                {
                    formatPhoneNumber = phoneNumber;
                }

                if(language!= null)
                {
                    if (language.Equals("vi") || string.IsNullOrEmpty(language))
                    {
                        string content = $"Ma OTP cua Anh/Chi la {codeResetPassWord}. Ma nay se het han trong 90 giay. Luu y: Anh/Chi khong tiet lo ma xac thuc nay cho bat ki ai.";
                        if (!SendOTPSOAPViettel(formatPhoneNumber, content))
                            return JsonUtil.Error("Đã có lỗi xảy ra, vui lòng kiểm tra lại!");

                        DataResponse data = new DataResponse(codeResetPassWord, currentDate);
                        return JsonUtil.Success(data, $"Đã gửi mã xác nhận đến " + phoneNumber);
                    }
                    else
                    {
                        string content = $"Your OTP is {codeResetPassWord}. This code will expire in 90 days. Note: You do not disclose this authentication code to anyone.";
                        if (!SendOTPSOAPViettel(formatPhoneNumber, content))
                            return JsonUtil.Error("Something went wrong, please check again!");

                        DataResponse data = new DataResponse(codeResetPassWord, currentDate);
                        return JsonUtil.Success(data, $"Verification code sent to " + phoneNumber);
                    }
                }
                else
                {
                    string content = $"Ma OTP cua Anh/Chi la {codeResetPassWord}. Ma nay se het han trong 90 giay. Luu y: Anh/Chi khong tiet lo ma xac thuc nay cho bat ki ai.";
                    if (!SendOTPSOAPViettel(formatPhoneNumber, content))
                        return JsonUtil.Error("Đã có lỗi xảy ra, vui lòng kiểm tra lại!");

                    DataResponse data = new DataResponse(codeResetPassWord, currentDate);
                    return JsonUtil.Success(data, $"Đã gửi mã xác nhận đến " + phoneNumber);
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }


        public string SendEmail(dynamic sendEmailOptions, EmailRecipient emailRecipient, string subject)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                    delegate (
                        object s,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors
                    )
                    {
                        return true;
                    };
            string content;
            //string url = "/forgot-password";
            string HTMLpath = sendEmailOptions.Path;
            //string subject = " Quên mật khẩu ";
            string _mailFrom = sendEmailOptions.MailFrom;

            MailMessage mess = new MailMessage();
            mess.Priority = MailPriority.High;
            mess.From = new MailAddress(_mailFrom, sendEmailOptions.MailFromDisplayName);
            mess.To.Add(new MailAddress(emailRecipient.Email));
            mess.Subject = subject;
            mess.IsBodyHtml = true;
            mess.BodyEncoding = UTF8Encoding.UTF8;
            //cc mail
            //MailAddress copy = new MailAddress("cod@jetlink.vn");
            //mess.CC.Add(copy);
            var otpResetPassword = emailRecipient.CodeResetPassWord;
            //var aTag = "<a href=" + urlConfirmResetPassWord + ">Xác thực tài khoản</a>";
            //mess.Body = "Click vào link bên dưới để xác nhận đổi mật khẩu cho tài khoản " + "<br/>" + "Email: " + emailRecipient.Email + "<br/>" + "Tên: " + emailRecipient.Name + "<br/>" + urlConfirmResetPassWord;


            content = System.IO.File.ReadAllText(HTMLpath);
            content = content.Replace("{{Email}}", emailRecipient.Email);
            content = content.Replace("{{Name}}", emailRecipient.Name);
            content = content.Replace("{{OTP}}", otpResetPassword);
            content = content.Replace("{{SDT}}", emailRecipient.PhoneNumber);
            content = content.Replace("{{CONTENT}}", emailRecipient.Content);

            mess.Body = content;

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = sendEmailOptions.Host;
            client.Port = sendEmailOptions.Port;
            client.EnableSsl = sendEmailOptions.EnableSsl;
            client.Credentials = new NetworkCredential(sendEmailOptions.MailFrom, sendEmailOptions.PassWordMailFrom);
            client.Send(mess);
            return "TRUE";
        }

        public string SendEmailWithBody(dynamic sendEmailOptions, EmailRecipient emailRecipient, string subject, string body)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                    delegate (
                        object s,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors
                    )
                    {
                        return true;
                    };
            string content;
            //string url = "/forgot-password";
            string HTMLpath = sendEmailOptions.Path;
            //string subject = " Quên mật khẩu ";
            string _mailFrom = sendEmailOptions.MailFrom;

            MailMessage mess = new MailMessage();
            mess.Priority = MailPriority.High;
            mess.From = new MailAddress(_mailFrom, sendEmailOptions.MailFromDisplayName);
            mess.To.Add(new MailAddress(emailRecipient.Email));
            mess.Subject = subject;
            mess.IsBodyHtml = true;
            mess.BodyEncoding = UTF8Encoding.UTF8;
            //cc mail
            //MailAddress copy = new MailAddress("cod@jetlink.vn");
            //mess.CC.Add(copy);
            var otpResetPassword = emailRecipient.CodeResetPassWord;
            //var aTag = "<a href=" + urlConfirmResetPassWord + ">Xác thực tài khoản</a>";
            //mess.Body = "Click vào link bên dưới để xác nhận đổi mật khẩu cho tài khoản " + "<br/>" + "Email: " + emailRecipient.Email + "<br/>" + "Tên: " + emailRecipient.Name + "<br/>" + urlConfirmResetPassWord;


            //content = System.IO.File.ReadAllText(HTMLpath);
            //content = content.Replace("{{Email}}", emailRecipient.Email);
            //content = content.Replace("{{Name}}", emailRecipient.Name);
            //content = content.Replace("{{body}}", body);
            //content = content.Replace("{{SDT}}", emailRecipient.PhoneNumber);
            //content = content.Replace("{{CONTENT}}", emailRecipient.Content);

            mess.Body = body;

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = sendEmailOptions.Host;
            client.Port = sendEmailOptions.Port;
            client.EnableSsl = sendEmailOptions.EnableSsl;
            client.Credentials = new NetworkCredential(sendEmailOptions.MailFrom, sendEmailOptions.PassWordMailFrom);
            client.Send(mess);
            return "TRUE";
        }
        public JsonResult CheckCodeValidChangePassword(ForgotPasswordViewModel model)
        {
            var user = _unitOfWork.RepositoryR<User>().GetSingle(x => x.Id == model.UserId);
            if (user != null)
            {
                if (user.VerificationDateTime == null || user.VerificationCode == null)
                {
                    return JsonUtil.Error(null, "Bạn chưa có yêu cầu đổi mật khẩu nào !");
                }
                DateTime currentDate = DateTime.Now;
                dynamic resetPassWordSentat = user.VerificationDateTime;
                TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);

                if (timeConfirmResetPassWord.TotalMinutes > 3)
                {
                    return JsonUtil.Error("Mã đã hết hạn sử dụng.");
                }

                if (!user.VerificationCode.Equals(model.Code))
                {
                    return JsonUtil.Error("Mã không khớp");
                }
                else
                {
                    return JsonUtil.Success(null, "Xác nhận có thể đổi password");
                }
            }
            else
            {
                return JsonUtil.Error("Người dùng không tồn tại");
            }
        }

        public JsonResult CheckCodeValidChangePasswordMobile(ForgotPasswordMobileViewModel model)
        {
            if (model.Language != null)
            {
                if (model.Language.Equals("en"))
                {
                    var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == model.CustomerId);
                    if (customer != null)
                    {
                        if (customer.VerificationDateTime == null || customer.VerificationCode == null)
                        {
                            return JsonUtil.Error(null, "You have not requested a password change yet!");
                        }
                        DateTime currentDate = DateTime.Now;
                        dynamic resetPassWordSentat = customer.VerificationDateTime;
                        TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);

                        if (timeConfirmResetPassWord.TotalMinutes > 1.5)
                        {
                            return JsonUtil.Error("The code has expired.");
                        }

                        if (!customer.VerificationCode.Equals(model.Code))
                        {
                            return JsonUtil.Error("Code does not match");
                        }
                        else
                        {
                            return JsonUtil.Success(null, "Confirm you can change your password");
                        }
                    }
                    else
                    {
                        return JsonUtil.Error("User does not exist");
                    }
                }
                else
                {
                    var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == model.CustomerId);
                    if (customer != null)
                    {
                        if (customer.VerificationDateTime == null || customer.VerificationCode == null)
                        {
                            return JsonUtil.Error(null, "Bạn chưa có yêu cầu đổi mật khẩu nào !");
                        }
                        DateTime currentDate = DateTime.Now;
                        dynamic resetPassWordSentat = customer.VerificationDateTime;
                        TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);

                        if (timeConfirmResetPassWord.TotalMinutes > 1.5)
                        {
                            return JsonUtil.Error("Mã đã hết hạn sử dụng.");
                        }

                        if (!customer.VerificationCode.Equals(model.Code))
                        {
                            return JsonUtil.Error("Mã không khớp");
                        }
                        else
                        {
                            return JsonUtil.Success(null, "Xác nhận có thể đổi password");
                        }
                    }
                    else
                    {
                        return JsonUtil.Error("Người dùng không tồn tại");
                    }
                }
            }
            else
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == model.CustomerId);
                if (customer != null)
                {
                    if (customer.VerificationDateTime == null || customer.VerificationCode == null)
                    {
                        return JsonUtil.Error(null, "Bạn chưa có yêu cầu đổi mật khẩu nào !");
                    }
                    DateTime currentDate = DateTime.Now;
                    dynamic resetPassWordSentat = customer.VerificationDateTime;
                    TimeSpan timeConfirmResetPassWord = currentDate.Subtract(resetPassWordSentat);

                    if (timeConfirmResetPassWord.TotalMinutes > 1.5)
                    {
                        return JsonUtil.Error("Mã đã hết hạn sử dụng.");
                    }

                    if (!customer.VerificationCode.Equals(model.Code))
                    {
                        return JsonUtil.Error("Mã không khớp");
                    }
                    else
                    {
                        return JsonUtil.Success(null, "Xác nhận có thể đổi password");
                    }
                }
                else
                {
                    return JsonUtil.Error("Người dùng không tồn tại");
                }
            }
            
        }

        public async Task<JsonResult> ForgotPasswordMobile(ChangePassWordViewModel model)
        {
            try
            {
                Customer cus = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x =>
                    x.Id == model.Id);

                cus.PasswordHash = _iEncryptionService.EncryptPassword(model.NewPassWord, cus.SecurityStamp);
                cus.VerificationCode = null;
                cus.VerificationDateTime = null;
                _unitOfWork.RepositoryCRUD<Customer>().Update(cus);
                _unitOfWork.Commit();
                if (model.Language != null)
                {
                    if (model.Language.Equals("en"))
                    {
                        return JsonUtil.Success(true, ValidatorMessage.Account.ChangePasswordEnglish);
                    }
                    else
                    {
                        return JsonUtil.Success(true, ValidatorMessage.Account.ChangePassword);
                    }
                }
                else
                {
                    return JsonUtil.Success(true, ValidatorMessage.Account.ChangePassword);
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> ChangePasswordMobile(ChangePassWordViewModel model, int userId)
        {
            try
            {
                
                Customer cus = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == userId);

                if (cus.Language.Equals("en"))
                {
                    if (model.CurrentPassWord == model.NewPassWord)
                        return JsonUtil.Error(ValidatorMessage.Account.NewPassWordNotDuplicatedEnglish);
                }
                else
                {
                    if (model.CurrentPassWord == model.NewPassWord)
                        return JsonUtil.Error(ValidatorMessage.Account.NewPassWordNotDuplicated);
                }

                var passwordHashOld = _iEncryptionService.EncryptPassword(model.CurrentPassWord, cus.SecurityStamp);
                if (cus.PasswordHash != passwordHashOld)
                {
                    if (cus.Language.Equals("en"))
                    {
                        return JsonUtil.Error(ValidatorMessage.Account.CurrentPassWordInValidEnglish);
                    }
                    else
                    {
                        return JsonUtil.Error(ValidatorMessage.Account.CurrentPassWordInValid);
                    }
                    
                }
                cus.PasswordHash = _iEncryptionService.EncryptPassword(model.NewPassWord, cus.SecurityStamp);
                _unitOfWork.RepositoryCRUD<Customer>().Update(cus);
                _unitOfWork.Commit();
                if (cus.Language.Equals("en"))
                {
                    return JsonUtil.Success(true, ValidatorMessage.Account.ChangePasswordEnglish);
                }
                else
                {
                    return JsonUtil.Success(true, ValidatorMessage.Account.ChangePassword);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> ForgotPasswordWeb(ChangePassWordViewModel model)
        {
            try
            {
                User user = await _unitOfWork.RepositoryR<User>().GetSingleAsync(x =>
                    x.Email == model.EmailOrPhone);
                if (user == null)
                {
                    return JsonUtil.Error(ValidatorMessage.Account.InvalidUserName);
                }
                user.PasswordHash = _iEncryptionService.EncryptPassword(model.NewPassWord, user.SecurityStamp);
                user.VerificationCode = null;
                user.VerificationDateTime = null;
                _unitOfWork.RepositoryCRUD<User>().Update(user);
                _unitOfWork.Commit();
                return JsonUtil.Success(true, ValidatorMessage.Account.ChangePassword);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> ChangePasswordWeb(ChangePassWordViewModel model)
        {
            try
            {
                if (model.CurrentPassWord == model.NewPassWord)
                    return JsonUtil.Error(ValidatorMessage.Account.NewPassWordNotDuplicated);
                User user = await _unitOfWork.RepositoryR<User>().GetSingleAsync(x =>
                    x.Id == model.Id.GetValueOrDefault(0));
                if (user == null)
                {
                    return JsonUtil.Error(ValidatorMessage.Account.NotExist);
                }

                var passwordHashOld = _iEncryptionService.EncryptPassword(model.CurrentPassWord, user.SecurityStamp);
                if (user.PasswordHash != passwordHashOld) return JsonUtil.Error(ValidatorMessage.Account.CurrentPassWordInValid);
                user.PasswordHash = _iEncryptionService.EncryptPassword(model.NewPassWord, user.SecurityStamp);
                _unitOfWork.RepositoryCRUD<User>().Update(user);
                _unitOfWork.Commit();
                return JsonUtil.Success(true, ValidatorMessage.Account.ChangePassword);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckUniqueEmailAndPhone(string email, string phoneNumber, string language)
        {
            try
            {
                if (language != null)
                {
                    if (language.Equals("en"))
                    {
                        if (!string.IsNullOrEmpty(phoneNumber) || !string.IsNullOrWhiteSpace(phoneNumber))
                            if (_unitOfWork.RepositoryR<Customer>().Any(x => x.PhoneNumber.ToLower().Equals(phoneNumber.ToLower())))
                                return JsonUtil.Error(ValidatorMessage.Customer.UniquePhoneEnglish);
                        if (!string.IsNullOrEmpty(email) || !string.IsNullOrWhiteSpace(email))
                            if (_unitOfWork.RepositoryR<Customer>().Any(x => x.Email.ToLower().Equals(email.ToLower())))
                                return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmailEnglish);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(phoneNumber) || !string.IsNullOrWhiteSpace(phoneNumber))
                            if (_unitOfWork.RepositoryR<Customer>().Any(x => x.PhoneNumber.ToLower().Equals(phoneNumber.ToLower())))
                                return JsonUtil.Error(ValidatorMessage.Customer.UniquePhone);
                        if (!string.IsNullOrEmpty(email) || !string.IsNullOrWhiteSpace(email))
                            if (_unitOfWork.RepositoryR<Customer>().Any(x => x.Email.ToLower().Equals(email.ToLower())))
                                return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmail);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(phoneNumber) || !string.IsNullOrWhiteSpace(phoneNumber))
                        if (_unitOfWork.RepositoryR<Customer>().Any(x => x.PhoneNumber.ToLower().Equals(phoneNumber.ToLower())))
                            return JsonUtil.Error(ValidatorMessage.Customer.UniquePhone);
                    if (!string.IsNullOrEmpty(email) || !string.IsNullOrWhiteSpace(email))
                        if (_unitOfWork.RepositoryR<Customer>().Any(x => x.Email.ToLower().Equals(email.ToLower())))
                            return JsonUtil.Error(ValidatorMessage.Customer.UniqueEmail);
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
