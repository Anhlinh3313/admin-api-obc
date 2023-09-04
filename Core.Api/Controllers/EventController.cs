using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Event;
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
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;

        public EventController(
            IEventService eventService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _eventService = eventService;
        }

        [HttpGet("GetListEvent")]
        public async Task<IActionResult> GetListEvent(string keySearch, string fromDateStart, string toDateStart, string fromDateEnd, string toDateEnd, 
            string objects, int? pageNum, int? pageSize)
        {
            return await _eventService.GetListEvent(keySearch, fromDateStart, toDateStart, fromDateEnd, toDateEnd, objects, 
                pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("DeActiveEvent")]
        public async Task<IActionResult> DeActiveEvent(int eventId)
        {
            return await _eventService.DeActiveEvent(eventId);
        }

        [HttpGet("DeEnabledEvent")]
        public async Task<IActionResult> DeEnabledEvent(int eventId)
        {
            return await _eventService.DeEnabledEvent(eventId);
        }

        [HttpGet("DeEndEvent")]
        public async Task<IActionResult> DeEndEvent(int eventId)
        {
            return await _eventService.DeEndEvent(eventId);
        }

        [HttpGet("GetDetailEvent")]
        public async Task<IActionResult> GetDetailEvent(int eventId)
        {
            return await _eventService.GetDetailEvent(eventId);
        }

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent([FromBody] EventViewModelCreate model)
        {
           
            return await _eventService.CreateEvent(model);
            
        }

        [HttpPost("UpdateEvent")]
        public async Task<IActionResult> UpdateEvent([FromBody] EventViewModelUpdate model)
        {
            return await _eventService.UpdateEvent(model);
        }

        [HttpPost("UploadImageEvent")]
        public async Task<IActionResult> UploadImageEvent([FromForm] EventViewModelUploadImage model)
        {
            return await _eventService.UploadImageEvent(model);
        }

        [HttpPost("UpdateImageEvent")]
        public IActionResult UpdateImageEvent([FromBody] EventViewModelUpdate model)
        {
            return _eventService.UpdateImageEvent(model);
        }

        [HttpGet("GetListCustomerEvent")]
        public IActionResult GetListCustomerEvent(int eventId, string keySearch, int? typeId, int? status, int? pageNum, int? pageSize)
        {
            return _eventService.GetListCustomerEvent(eventId, keySearch, typeId.GetValueOrDefault(0), status.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("UpdateNoteCustomerEvent")]
        public IActionResult UpdateNoteCustomerEvent(int customerEventId, string note)
        {
            return _eventService.UpdateNoteCustomerEvent(customerEventId, note);
        }

        [HttpGet("GetAllStatusCustomerEvent")]
        public IActionResult GetAllStatusCustomerEvent()
        {
            return _eventService.GetAllStatusCustomerEvent();
        }

        [HttpGet("GetAllEvent")]
        public IActionResult GetAllEvent(int? type)
        {
            return _eventService.GetAllEvent(type.GetValueOrDefault(0));
        }

        [HttpGet("GetListTransactionEvent")]
        public IActionResult GetListTransactionEvent(int? eventId, string keySearch, DateTime fromDate, DateTime toDate, int? chapterId, int? statusId, int? pageNum, int? pageSize)
        {
            return _eventService.GetListTransactionEvent(eventId.GetValueOrDefault(0), keySearch, 
                fromDate, toDate, chapterId.GetValueOrDefault(0), statusId.GetValueOrDefault(0),
                pageNum.GetValueOrDefault(1),pageSize.GetValueOrDefault(10));
        }

        [HttpGet("ActiveTransactionEvent")]
        public async Task<IActionResult> ActiveTransactionEvent(int transactionEventId, int? active, string note)
        {
            var currentUserId = GetCurrentUserId();
            return await _eventService.ActiveTransactionEvent(transactionEventId, active.GetValueOrDefault(0), note, currentUserId);
        }

        //[HttpGet("GenerateQrCode")]
        //public async Task<IActionResult> GenerateQrCode(string text, string fileName, string folderName)
        //{
        //    var image = _eventService.GenerateByteArray(text);
        //    var files = File(image, "image/png");
        //    var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
        //        files.ContentType);
        //    streamResult.FileDownloadName = files.FileDownloadName;
        //    return await _eventService.GenerateQrCode(streamResult, fileName, folderName);
        //}

        [HttpGet("GenerateLinkInformationQrCodePath")]
        public async Task<IActionResult> GenerateLinkInformationQrCodePath(int eventId)
        {
            var detailEvent = _eventService.GetEvent(eventId);
            var detailTimeEvent = _eventService.GetTimeEvent(eventId);
            if (detailEvent.Objects.ToLower().Equals("Thành viên OBC".ToLower()))
            {
                var text = "Tên sự kiện: " + detailEvent.Name +
                           "\nMã sự kiện: " + detailEvent.Code +
                           "\nMô tả ngắn: " + detailEvent.ShortDescription +
                           "\nThời gian: " + detailTimeEvent;
                var image = _eventService.GenerateByteArray(text);
                var files = File(image, "image/png");
                var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                    files.ContentType);
                streamResult.FileDownloadName = files.FileDownloadName;
                return await _eventService.GenerateLinkInformationQrCodePath(streamResult, eventId);
            }
            else
            {
                var text = "Tên sự kiện: " + detailEvent.Name +
                           "\nMã sự kiện: " + detailEvent.Code +
                           "\nMô tả ngắn: " + detailEvent.ShortDescription +
                           "\nLink biểu mẫu đăng ký: " + detailEvent.LinkInformation +
                           "\nThời gian: " + detailTimeEvent;
                var image = _eventService.GenerateByteArray(text);
                var files = File(image, "image/png");
                var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                    files.ContentType);
                streamResult.FileDownloadName = files.FileDownloadName;
                return await _eventService.GenerateLinkInformationQrCodePath(streamResult, eventId);
            }
        }

        [HttpGet("GenerateLinkCheckInQrCodePath")]
        public async Task<IActionResult> GenerateLinkCheckInQrCodePath(int eventId)
        {
            var detailEvent = _eventService.GetEvent(eventId);
            var detailTimeEvent = _eventService.GetTimeEvent(eventId);
            if (detailEvent.Objects.ToLower().Equals("Thành viên OBC".ToLower()))
            {
                var text = "Thông tin checkin" +
                           "\nTên sự kiện: " + detailEvent.Name +
                           "\nMã sự kiện: " + detailEvent.Code +
                           "\nMô tả ngắn: " + detailEvent.ShortDescription +
                           "\nThời gian: " + detailTimeEvent;
                var image = _eventService.GenerateByteArray(text);
                var files = File(image, "image/png");
                var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                    files.ContentType);
                streamResult.FileDownloadName = files.FileDownloadName;
                return await _eventService.GenerateLinkCheckInQrCodePath(streamResult, eventId);
            }
            else
            {
                var text = "Thông tin checkin" +
                           "\nTên sự kiện: " + detailEvent.Name +
                           "\nMã sự kiện: " + detailEvent.Code +
                           "\nMô tả ngắn: " + detailEvent.ShortDescription +
                           "\nLink biểu mẫu checkin: " + detailEvent.LinkCheckIn +
                           "\nThời gian: " + detailTimeEvent;
                var image = _eventService.GenerateByteArray(text);
                var files = File(image, "image/png");
                var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                    files.ContentType);
                streamResult.FileDownloadName = files.FileDownloadName;
                return await _eventService.GenerateLinkCheckInQrCodePath(streamResult, eventId);
            }
            
        }

        [HttpGet("GetListEventMobile")]
        public IActionResult GetListEventMobile(string keySearch,int? eventType, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.GetListEventMobile(keySearch,currentUserId, eventType.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetDetailEventMobile")]
        public async Task<IActionResult> GetDetailEventMobile(string eventCode)
        {
            var currentUserId = GetCurrentUserId();
            return await _eventService.GetDetailEventMobile(currentUserId,eventCode);
        }

        [HttpGet("LikedEvent")]
        public IActionResult LikedEvent(int eventId)
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.LikedEvent(currentUserId, eventId);
        }

        [HttpGet("SharedEvent")]
        public IActionResult SharedEvent(int eventId)
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.SharedEvent(currentUserId, eventId);
        }

        [HttpGet("CheckCustomerRegisterEvent")]
        public IActionResult CheckCustomerRegisterEvent(int eventId)
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.CheckCustomerRegisterEvent(currentUserId, eventId);
        }

        [HttpGet("RegisterEvent")]
        public async Task<IActionResult> RegisterEvent(int eventId, string phoneNumber, string email)
        {
            var currentUserId = GetCurrentUserId();
            return await _eventService.RegisterEvent(currentUserId, eventId, phoneNumber, email);
        }

        [HttpPost("PaymentEvent")]
        public async Task<IActionResult> PaymentEvent([FromForm] EventViewModelUploadImageTransaction model)
        {
            var currentUserId = GetCurrentUserId();
            return await _eventService.UploadImageTransactionEvent(model, currentUserId);
        }

        [HttpGet("CustomerCheckInEvent")]
        public IActionResult CustomerCheckInEvent(string eventCode)
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.CustomerCheckInEvent(eventCode, currentUserId);
        }

        [HttpGet("GetListRecentEvent")]
        public IActionResult GetListRecentEvent()
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.GetListRecentEvent(currentUserId);
        }
    }
}
