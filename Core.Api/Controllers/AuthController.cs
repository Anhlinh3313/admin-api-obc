using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Accounts;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Infrastructure.ViewModels;
using Core.Infrastructure.Utils;
using System.IO;
using Core.Entity.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly SendMail _iSendMail;
        private readonly IEventService _eventService;
        private readonly IBusinessService _businessService;

        public AuthController(
            IAccountService accountService, 
            IEventService eventService,
            IBusinessService businessService,
            ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IOptions<SendMail> sendMail,
            IUnitOfWork unitOfWork
        ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _accountService = accountService;
            _iSendMail = sendMail.Value;
            _eventService = eventService;
            _businessService = businessService;

        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInViewModel model)
        {
            return await _accountService.SignIn(model);
        }
        [HttpPost("SignInMobile")]
        public async Task<IActionResult> SignInMobile([FromBody] SignInMobileViewModel model)
        {
            return await _accountService.SignInMobile(model);
        }
        [HttpPost("SignUpMobile")]
        public  IActionResult SignUpMobile([FromBody] SignUpMobileViewModel model)
        {
            //return _accountService.SignUpMobile(model);
            var customer = _accountService.SignUpMobile(model);
            var success = customer.Value.GetType().GetProperty("isSuccess")?.GetValue(customer.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return customer;
            }
            var value = customer.Value.GetType().GetProperty("data")?.GetValue(customer.Value, null);
            var link = (Customer)value;
            var getTextQrCode = _businessService.GetCustomer(link.Id);
            var text = "Họ và tên: " + getTextQrCode.FullName +
                       "\nĐiện thoại: " + getTextQrCode.PhoneNumber +
                       "\nEmail: " + getTextQrCode.Email +
                       "\nCông ty: " + getTextQrCode.BusinessName +
                       "\nNgành nghề: " + getTextQrCode.ProfessionName +
                       "\nLĩnh vực: " + getTextQrCode.FieldOperationName +
                       "\nTỉnh thành: " + getTextQrCode.ProvinceName;
            var image = _eventService.GenerateByteArray(text);
            var files = File(image, "image/png");
            var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                files.ContentType);
            streamResult.FileDownloadName = files.FileDownloadName;

            return _businessService.GenerateQrCodeCustomer(streamResult, link.Id);
        }
        [HttpPost("ForgotPasswordMobile")]
        public async Task<IActionResult> ForgotPasswordMobile([FromBody] ChangePassWordViewModel model)
        {
            return await _accountService.ForgotPasswordMobile(model);
        }
        
        [HttpPost("ChangePasswordMobile")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordMobile([FromBody] ChangePassWordViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            return await _accountService.ChangePasswordMobile(model, currentUserId);
        }

        [HttpPost("ForgotPasswordWeb")]
        public async Task<IActionResult> ForgotPasswordWeb([FromBody] ChangePassWordViewModel model)
        {
            return await _accountService.ForgotPasswordWeb(model);
        }

        [HttpPost("ChangePasswordWeb")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordWeb([FromBody] ChangePassWordViewModel model)
        {
            return await _accountService.ChangePasswordWeb(model);
        }

        [HttpPost("SendMailForgotPassword")]
        public JsonResult SendMailForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            return _accountService.SendMailForgotPassword(model);
        }

        [HttpPost("CheckCodeValidChangePassword")]
        public JsonResult CheckCodeValidChangePassword([FromBody] ForgotPasswordViewModel model)
        {
            return _accountService.CheckCodeValidChangePassword(model);
        }

        [HttpGet("SendOTPByPhoneNumberForgotPass")]
        public async Task<JsonResult> SendOTPByPhoneNumberForgotPass(string phoneNumber,string language)
        {
            return await _accountService.SendOTPByPhoneNumberForgotPass(phoneNumber,language);
        }

        [HttpGet("SendOTPByPhoneNumberRegister")]
        public JsonResult SendOTPByPhoneNumberRegister(string phoneNumber, string language)
        {
            return _accountService.SendOTPByPhoneNumberRegister(phoneNumber, language);
        }

        [HttpPost("SendMailForgotPasswordMobile")]
        public JsonResult SendMailForgotPasswordMobile([FromBody] ForgotPasswordMobileViewModel model)
        {
            return _accountService.SendMailForgotPasswordMobile(model);
        }

        [HttpPost("CheckCodeValidChangePasswordMobile")]
        public JsonResult CheckCodeValidChangePasswordMobile([FromBody] ForgotPasswordMobileViewModel model)
        {
            return _accountService.CheckCodeValidChangePasswordMobile(model);
        }

        [AllowAnonymous]
        [HttpPost("SendMailOTP")]
        public JsonResult SendMailOTP([FromBody] ForgotPasswordViewModel model)
        {
            return _accountService.SendMailOTP(model);
        }

        [HttpGet("CheckUniqueEmailAndPhone")]
        public IActionResult CheckUniqueEmailAndPhone(string email, string phoneNumber, string language)
        {
            return _accountService.CheckUniqueEmailAndPhone(email, phoneNumber, language);
        }
    }
}
