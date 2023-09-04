using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Customer;
using Core.Business.ViewModels.User;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IBusinessService _businessService;
        private readonly IEventService _eventService;

        public CustomerController(
            ICustomerService customerService,
            IBusinessService businessService,
            IEventService eventService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _customerService = customerService;
            _businessService = businessService;
            _eventService = eventService;
        }

        [HttpGet("GetDetailCustomer")]
        public async Task<IActionResult> GetDetailCustomer(int customerId)
        {
            return await _customerService.GetDetailProfileCustomerAsync(customerId);
        }

        [HttpPost("UpdateCustomerProfileAsync")]
        public async Task<IActionResult> UpdateCustomerProfileAsync([FromBody] CustomerViewModel model)
        {
            var cus =  await _customerService.UpdateCustomerProfileAsync(model);
            var success = cus.Value.GetType().GetProperty("isSuccess")?.GetValue(cus.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return cus;
            }
            var value = cus.Value.GetType().GetProperty("data")?.GetValue(cus.Value, null);
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

        [HttpGet("AcceptPremium")]
        public async Task<IActionResult> AcceptPremium(int transactionId, string note, int? active)
        {
            return await _customerService.AcceptPremium(transactionId, note, active);
        }

        [HttpGet("AcceptChapter")]
        public async Task<IActionResult> AcceptChapter(int customerId, string note, int? active)
        {
            var currentUserId = GetCurrentUserId();
            return await _customerService.AcceptChapter(customerId, note, active, currentUserId);
        }

        [HttpGet("CancelMember")]
        public async Task<IActionResult> CancelMember(int customerId, string note)
        {
            return await _customerService.CancelMember(customerId, note);
        }

        [HttpGet("GetListCustomerWaitingActive")]
        public IActionResult GetListCustomerWaitingActive(int? transactionId, string keySearch, DateTime? fromDate, DateTime? toDate,
            int? chapterId, int? statusId, int? pageNum, int? pageSize)
        {
            return _customerService.GetListCustomerWaitingActive(transactionId.GetValueOrDefault(0), keySearch, fromDate.GetValueOrDefault(DateTime.Now), toDate.GetValueOrDefault(DateTime.Now), chapterId.GetValueOrDefault(0), statusId.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetListCustomerMember")]
        public IActionResult GetListCustomerMember(int chapterId, string keySearch, string fieldOperations, string status, int? pageNum, int? pageSize)
        {
            return _customerService.GetListCustomerMember(chapterId, keySearch, fieldOperations, status, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("ChangeCustomerRole")]
        public async Task<IActionResult> ChangeCustomerRole(int customerId, int? customerRoleId)
        {
            return await _customerService.ChangeCustomerRole(customerId, customerRoleId);
        }
        [HttpGet("GetListRoleMemberChapter")]
        public async Task<IActionResult> GetListRoleMemberChapter()
        {
            return await _customerService.GetListRoleMemberChapter();
        }

        [HttpGet("GetInformationCustomer")]
        public async Task<IActionResult> GetInformationCustomer()
        {
            var currentUserId = GetCurrentUserId();
            return await _customerService.GetInformationCustomer(currentUserId);
        }

        [HttpGet("GetListCustomerOutOfChapter")]
        public IActionResult GetListCustomerOutOfChapter(string fullName, string businessName, string fieldOperationsName, string provinceName, string keySearch, int? type)
        {
            var currentUserId = GetCurrentUserId();
            return _customerService.GetListCustomerOutOfChapter(currentUserId, fullName, businessName, fieldOperationsName, provinceName,keySearch, type.GetValueOrDefault(0));
        }

        [HttpGet("GetContractCustomer")]
        public IActionResult GetContractCustomer()
        {
            var currentUserId = GetCurrentUserId();
            return _customerService.GetContractCustomer(currentUserId);
        }

        [HttpGet("GetCustomerProfile")]
        public async Task<IActionResult> GetCustomerProfile(int customerId)
        {
            var currentUserId = GetCurrentUserId();
            return await _customerService.GetCustomerProfile(customerId, currentUserId, "");
        }

        [HttpGet("GetCustomerProfileByPhone")]
        public async Task<IActionResult> GetCustomerProfileByPhone(string phone)
        {
            var currentUserId = GetCurrentUserId();
            return await _customerService.GetCustomerProfile(0, currentUserId, phone);
        }

        [HttpPost("UploadAvatarCustomer")]
        public async Task<IActionResult> UploadAvatarCustomer([FromForm] CustomerViewModelUploadAvatar model)
        {
            var currentUserId = GetCurrentUserId();
            return await _customerService.UploadAvatarCustomer(model, currentUserId);
        }

        [HttpGet("GetCustomerOutOfChapterWithQrCode")]
        public IActionResult GetCustomerOutOfChapterWithQrCode()
        {
            var currentUserId = GetCurrentUserId();
            return  _customerService.GetCustomerOutOfChapterWithQrCode(currentUserId);
        }

        [HttpGet("UpdateExpoPushTokenCustomer")]
        public IActionResult UpdateExpoPushTokenCustomer(int customerId, string expoPushToken, string language)
        {
            return _customerService.UpdateExpoPushTokenCustomer(customerId, expoPushToken, language);
        }

        [HttpGet("DeleteExpoPushTokenCustomer")]
        public IActionResult DeleteExpoPushTokenCustomer(int customerId)
        {
            return _customerService.DeleteExpoPushTokenCustomer(customerId);
        }

        [HttpGet("GetIndicators")]
        public IActionResult GetIndicators(int customerId)
        {
            return _customerService.GetIndicators(customerId);
        }

        [HttpGet("AssessCustomer")]
        public IActionResult AssessCustomer(int customerId, int value, string comment)
        {
            return _customerService.AssessCustomer(customerId, value, comment);
        }

        [HttpGet("GetListTopCustomer")]
        public IActionResult GetListTopCustomer(int? pageNum)
        {
            var currentUserId = GetCurrentUserId();
            return _customerService.GetListTopCustomer(currentUserId,pageNum.GetValueOrDefault(1));
        }

        [HttpGet("UpdateLanguageCustomer")]
        public IActionResult UpdateLanguageCustomer(int customerId, string language)
        {
            return _customerService.UpdateLanguageCustomer(customerId, language);
        }

        [HttpGet("CheckCustomer")]
        public IActionResult CheckCustomer(string expoPushToken)
        {
            var currentUserId = GetCurrentUserId();
            return _customerService.CheckCustomer(currentUserId, expoPushToken);
        }
        [HttpGet("GetAllAssessCustomer")]
        public IActionResult GetAllAssessCustomer(int customerId, int? pageNum, int? pageSize)
        {
            return _customerService.GetAllAssessCustomer(customerId, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }
        [HttpGet("GetAvgAssessCustomer")]
        public IActionResult GetAvgAssessCustomer(int customerId)
        {
            return _customerService.GetAvgAssessCustomer(customerId);
        }
        [HttpGet("SendMailOTPChangeEmailCustomer")]
        public IActionResult SendMailOTPChangeEmailCustomer(string newEmail, int? sendAgain)
        {
            var currentUserId = GetCurrentUserId();
            return _customerService.SendMailOTPChangeEmailCustomer(currentUserId, newEmail, sendAgain.GetValueOrDefault(0));
        }
        [HttpGet("SendOTPByPhoneChangePhoneCustomer")]
        public async Task<IActionResult> SendOTPByPhoneChangePhoneCustomer(string newPhoneNumber, int? sendAgain)
        {
            var currentUserId = GetCurrentUserId();
            return await _customerService.SendOTPByPhoneChangePhoneCustomer(currentUserId, newPhoneNumber, sendAgain.GetValueOrDefault(0));
        }
        [HttpGet("CheckCodeValidChangEmailOrPhoneCustomer")]
        public IActionResult CheckCodeValidChangEmailOrPhoneCustomer(string code, string type)
        {
            var currentUserId = GetCurrentUserId();
            var cus =  _customerService.CheckCodeValidChangEmailOrPhoneCustomer(currentUserId, code, type);
            var success = cus.Value.GetType().GetProperty("isSuccess")?.GetValue(cus.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return cus;
            }
            var value = cus.Value.GetType().GetProperty("data")?.GetValue(cus.Value, null);
            var link = (int)value;
            var getTextQrCode = _businessService.GetCustomer(link);
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

            var geneQr = _businessService.GenerateQrCodeCustomer(streamResult, link);
            var successGeneQr = geneQr.Value.GetType().GetProperty("isSuccess")?.GetValue(geneQr.Value, null);
            var isSuccessGeneQr = (int)success;
            if (isSuccessGeneQr == 0)
            {
                return cus;
            }
            return cus;
        }
    }
}
