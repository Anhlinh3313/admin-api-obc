using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Business;
using Core.Business.ViewModels.User;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusinessController : BaseController
    {
        private readonly IBusinessService _businessService;
        private readonly IEventService _eventService;

        public BusinessController(
            IBusinessService businessService,
            IEventService eventService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _businessService = businessService;
            _eventService = eventService;
        }
        // GET: api/<UserController>
        [HttpPost("GetListBusiness")]
        public IActionResult GetListBusiness([FromBody] BusinessModel model)
        {
            return _businessService.GetListBusiness(model.KeySearch, model.Province,model.Profession, model.FieldOperation,
                model.CustomerRole.GetValueOrDefault(0), model.PageNum.GetValueOrDefault(1), model.PageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailBusiness")]
        public IActionResult GetDetailBusiness(int customerId)
        {
            return _businessService.GetDetailBusinessAsync(customerId);
        }

        [HttpGet("GetDetailBusinessByCustomerId")]
        public async Task<IActionResult> GetDetailBusinessByCustomerId(int customerId)
        {
            return await _businessService.GetDetailBusinessByCustomerIdAsync(customerId);
        }

        [HttpGet("GetDetailProfileBusiness")]
        public async Task<IActionResult> GetDetailProfileBusiness(int businessId)
        {
            return await _businessService.GetDetailProfileBusinessAsync(businessId);
        }

        //// POST api/<UserController>
        [HttpPost("CreateBusiness")]
        public async Task<IActionResult> CreateBusiness([FromBody] BusinessViewModelCreateAdmin model)
        {
            var currentUserId = GetCurrentUserId();
            var customerId = await _businessService.CreateBusinessAsync(model, currentUserId);
            var success = customerId.Value.GetType().GetProperty("isSuccess")?.GetValue(customerId.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return customerId;
            }
            var value = customerId.Value.GetType().GetProperty("data")?.GetValue(customerId.Value, null);
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

            return _businessService.GenerateQrCodeCustomer(streamResult, link);
        }

        //// PUT api/<UserController>/5
        [HttpPost("UpdateBusiness")]
        public async Task<IActionResult> UpdateBusiness([FromBody] BusinessViewModelCreateAdmin model)
        {
            var currentUserId = GetCurrentUserId();
            var customerId = await _businessService.UpdateBusinessAsync(model, currentUserId);
            var success = customerId.Value.GetType().GetProperty("isSuccess")?.GetValue(customerId.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return customerId;
            }
            var value = customerId.Value.GetType().GetProperty("data")?.GetValue(customerId.Value, null);
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

            return _businessService.GenerateQrCodeCustomer(streamResult, link);
        }

        [HttpPost("UpdateProfileBusiness")]
        public async Task<IActionResult> UpdateProfileBusiness([FromBody] BusinessViewModel model)
        {
            var business = await _businessService.UpdateProfileBusinessAsync(model);
            var success = business.Value.GetType().GetProperty("isSuccess")?.GetValue(business.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return business;
            }
            var value = business.Value.GetType().GetProperty("data")?.GetValue(business.Value, null);
            var link = (Entity.Entities.Business)value;
            var getTextQrCode = _businessService.GetCustomer(link.CustomerId.GetValueOrDefault());
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

            return _businessService.GenerateQrCodeCustomer(streamResult, link.CustomerId.GetValueOrDefault());

        }

        [HttpGet("DeActiveBusiness")]
        public async Task<IActionResult> DeActiveBusiness(int businessId)
        {
            return await _businessService.DeActiveBusinessAsync(businessId);
        }

        [HttpGet("DeEnableBusiness")]
        public async Task<IActionResult> DeEnableBusiness(int businessId)
        {
            return await _businessService.DeEnableBusinessAsync(businessId);
        }

        [HttpGet("DropdownCustomerRole")]
        public async Task<IActionResult> DropdownCustomerRole()
        {
            return await _businessService.DropdownCustomerRole();
        }

        [HttpGet("GetListBusinessPending")]
        public IActionResult GetListBusinessPending(string keySearch, int? status, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _businessService.GetListBusinessPendingAsync(keySearch, status.GetValueOrDefault(0), currentUserId, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpPost("SubscribeChapter")]
        public async Task<IActionResult> SubscribeChapter([FromBody] BusinessViewModelCreateMobile model)
        {
            var business = await _businessService.SubscribeChapter(model);
            var success = business.Value.GetType().GetProperty("isSuccess")?.GetValue(business.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return business;
            }
            var value = business.Value.GetType().GetProperty("data")?.GetValue(business.Value, null);
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

            return _businessService.GenerateQrCodeCustomer(streamResult, link);
        }

        [HttpPost("UploadAvatarBusiness")]
        public async Task<IActionResult> UploadAvatarBusiness([FromForm] BusinessViewModelUploadAvatar model)
        {
            var currentUserId = GetCurrentUserId();
            return await _businessService.UploadAvatarBusiness(model, currentUserId);
        }
        [HttpGet("ChangeProfessionBusiness")]
        public IActionResult ChangeProfessionBusiness(int professionId, int fieldOperationsId)
        {
            var currentUserId = GetCurrentUserId();
            return _businessService.ChangeProfessionBusiness(currentUserId, professionId, fieldOperationsId);
        }
        [HttpGet("ChangeFieldOperationsBusiness")]
        public IActionResult ChangeFieldOperationsBusiness(int fieldOperationsId)
        {
            var currentUserId = GetCurrentUserId();
            return _businessService.ChangeFieldOperationsBusiness(currentUserId, fieldOperationsId);
        }
        [HttpGet("AcceptChangeProfessionAndFieldOperationsBusiness")]
        public IActionResult AcceptChangeProfessionAndFieldOperationsBusiness(int customerId, int active, string note)
        {
            var currentUserId = GetCurrentUserId();
            var business =  _businessService.ChangeProfessionAndFieldOperationsBusiness(customerId, active, note, currentUserId);
            var success = business.Value.GetType().GetProperty("isSuccess")?.GetValue(business.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return business;
            }
            var value = business.Value.GetType().GetProperty("data")?.GetValue(business.Value, null);
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

            return _businessService.GenerateQrCodeCustomer(streamResult, link);
        }
    }
}
