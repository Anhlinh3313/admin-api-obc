using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Course;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CourseController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly IEventService _eventService;

        public CourseController(
            ICourseService courseService,
            IEventService eventService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _courseService = courseService;
            _eventService = eventService;
        }

        [HttpGet("GetListCourse")]
        public async Task<IActionResult> GetListCourse(string keySearch, string fromDateStart, string toDateStart, string fromDateEnd, string toDateEnd, 
            string objects, int? pageNum, int? pageSize)
        {
            return await _courseService.GetListCourse(keySearch, fromDateStart, toDateStart, fromDateEnd, toDateEnd, objects, 
                pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("DeActiveCourse")]
        public async Task<IActionResult> DeActiveCourse(int courseId)
        {
            return await _courseService.DeActiveCourse(courseId);
        }

        [HttpGet("DeEnabledCourse")]
        public async Task<IActionResult> DeEnabledCourse(int courseId)
        {
            return await _courseService.DeEnabledCourse(courseId);
        }

        //[HttpGet("DeEndCourse")]
        //public async Task<IActionResult> DeEndCourse(int CourseId)
        //{
        //    return await _courseService.DeEndCourse(CourseId);
        //}

        [HttpGet("GetDetailCourse")]
        public async Task<IActionResult> GetDetailCourse(int courseId)
        {
            return await _courseService.GetDetailCourse(courseId);
        }

        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseViewModelCreate model)
        {
            var courseId = await _courseService.CreateCourse(model);
            var success = courseId.Value.GetType().GetProperty("isSuccess")?.GetValue(courseId.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return courseId;
            }
            var value = courseId.Value.GetType().GetProperty("data")?.GetValue(courseId.Value, null);
            var link = (int)value;
            Course detailCourse = _courseService.DetailCourse(link);
            var text = "";
            if (detailCourse.Form.ToLower().Equals("khoá học"))
            {
                text = "Mã khoá học: " + detailCourse.Code;
            }
            else
            {
                text = "Mã video: " + detailCourse.Code;
            }
            
            var image = _eventService.GenerateByteArray(text);
            var files = File(image, "image/png");
            var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                files.ContentType);
            streamResult.FileDownloadName = files.FileDownloadName;

            return await _courseService.GenerateInformationQrCodePath(streamResult, link);
        }

        [HttpPost("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseViewModelUpdate model)
        {
            return await _courseService.UpdateCourse(model);
        }

        [HttpPost("UploadImageCourse")]
        public async Task<IActionResult> UploadImageCourse([FromForm] CourseViewModelUploadImage model)
        {
            return await _courseService.UploadImageCourse(model);
        }

        //[HttpPost("UploadVideoCourse")]
        //[DisableFormValueModelBinding]
        //[RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
        //[RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadVideoCourse([FromForm] CourseViewModelUploadImage model)
        //{
        //    return await _courseService.UploadVideoCourse(model);
        //}

        [HttpPost("UpdateImageCourse")]
        public IActionResult UpdateImageCourse([FromBody] CourseViewModelUpdateFile model)
        {
            return _courseService.UpdateImageCourse(model);
        }

        [HttpGet("GetListCustomerCourse")]
        public IActionResult GetListCustomerCourse(int courseId, string keySearch, int? typeId, int? status, int? pageNum, int? pageSize)
        {
            return _courseService.GetListCustomerCourse(courseId, keySearch, typeId.GetValueOrDefault(0), status.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("UpdateNoteCustomerCourse")]
        public IActionResult UpdateNoteCustomerCourse(int customerCourseId, string note)
        {
            return _courseService.UpdateNoteCustomerCourse(customerCourseId, note);
        }

        //[HttpGet("GetAllStatusCustomerCourse")]
        //public IActionResult GetAllStatusCustomerCourse()
        //{
        //    return _courseService.GetAllStatusCustomerCourse();
        //}

        [HttpGet("GetAllCourseFee")]
        public IActionResult GetAllCourseFee(int? type)
        {
            return _courseService.GetAllCourseFee(type.GetValueOrDefault(0));
        }

        [HttpGet("GetListTransactionCourse")]
        public IActionResult GetListTransactionCourse(int? courseId, string keySearch, DateTime fromDate, DateTime toDate, int? chapterId, int? statusId, int? pageNum, int? pageSize)
        {
            return _courseService.GetListTransactionCourse(courseId.GetValueOrDefault(0), keySearch,
                fromDate, toDate, chapterId.GetValueOrDefault(0), statusId.GetValueOrDefault(0),
                pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("ActiveTransactionCourse")]
        public async Task<IActionResult> ActiveTransactionCourse(int transactionCourseId, int? active, string note)
        {
            var currentUserId = GetCurrentUserId();
            return await _courseService.ActiveTransactionCourse(transactionCourseId, active.GetValueOrDefault(0), note, currentUserId);
        }

        [HttpGet("GetListAssess")]
        public IActionResult GetListAssess(int courseId)
        {
            return _courseService.GetListAssess(courseId);
        }

        [HttpGet("GetListCourseMobile")]
        public IActionResult GetListCourseMobile(string keySearch ,int? courseType, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.GetListCourseMobile(keySearch , currentUserId, courseType.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetDetailCourseMobile")]
        public IActionResult GetDetailCourseMobile(string courseCode)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.GetDetailCourseMobile(currentUserId, courseCode);
        }

        [HttpGet("LikedCourse")]
        public IActionResult LikedCourse(int courseId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.LikedCourse(currentUserId, courseId);
        }

        [HttpGet("SharedCourse")]
        public IActionResult SharedCourse(int courseId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.SharedCourse(currentUserId, courseId);
        }

        [HttpGet("CheckCustomerRegisterCourse")]
        public IActionResult CheckCustomerRegisterCourse(int courseId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.CheckCustomerRegisterCourse(currentUserId, courseId);
        }

        [HttpGet("RegisterCourse")]
        public async Task<IActionResult> RegisterCourse(int courseId, string phoneNumber, string email)
        {
            var currentUserId = GetCurrentUserId();
            return await _courseService.RegisterCourse(currentUserId, courseId, phoneNumber, email);
        }

        [HttpPost("PaymentCourse")]
        public async Task<IActionResult> PaymentCourse([FromForm] CourseViewModelUploadImageTransaction model)
        {
            var currentUserId = GetCurrentUserId();
            return await _courseService.UploadImageTransactionCourse(model, currentUserId);
        }

        [HttpGet("AssessCourse")]
        public IActionResult AssessCourse(int courseId, int value, string comment)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.AssessCourse(currentUserId, courseId, value, comment);
        }

        [HttpPost("UploadCertificate")]
        public async Task<IActionResult> UploadCertificate([FromForm] CourseViewModelUploadCertificate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _courseService.UploadCertificate(model, currentUserId);
        }

        [HttpGet("GetCertificateWithCourseId")]
        public IActionResult GetCertificateWithCourseId(int courseId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.GetCertificateWithCustomerId(currentUserId, courseId);
        }

        [HttpGet("GetListVideoMobile")]
        public IActionResult GetListVideoMobile(string keySearch, int? videoType, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.GetListVideoMobile(keySearch,currentUserId, videoType.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetDetailVideoMobile")]
        public IActionResult GetDetailVideoMobile(string videoCode)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.GetDetailVideoMobile(currentUserId, videoCode);
        }

        [HttpGet("LikedVideo")]
        public IActionResult LikedVideo(int videoId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.LikedCourse(currentUserId, videoId);
        }

        [HttpGet("CheckCustomerRegisterVideo")]
        public IActionResult CheckCustomerRegisterVideo(int videoId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.CheckCustomerRegisterCourse(currentUserId, videoId);
        }

        [HttpGet("RegisterVideo")]
        public async Task<IActionResult> RegisterVideo(int videoId, string phoneNumber, string email)
        {
            var currentUserId = GetCurrentUserId();
            return await _courseService.RegisterCourse(currentUserId, videoId, phoneNumber, email);
        }

        [HttpPost("PaymentVideo")]
        public async Task<IActionResult> PaymentVideo([FromForm] VideoViewModelUploadImageTransaction model)
        {
            var currentUserId = GetCurrentUserId();
            return await _courseService.UploadImageTransactionVideo(model, currentUserId);
        }

        [HttpGet("GetListAssessCourseMobile")]
        public IActionResult GetListAssessCourseMobile(int courseId, int? pageNum, int? pageSize)
        {
            return _courseService.GetListAssessMobile(courseId, pageNum.GetValueOrDefault(0), pageSize.GetValueOrDefault(0));
        }

        [HttpGet("GetListAssessVideoMobile")]
        public IActionResult GetListAssessVideoMobile(int videoId, int? pageNum, int? pageSize)
        {
            return _courseService.GetListAssessMobile(videoId, pageNum.GetValueOrDefault(0), pageSize.GetValueOrDefault(0));
        }

        [HttpGet("AssessVideo")]
        public IActionResult AssessVideo(int videoId, int value, string comment)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.AssessCourse(currentUserId, videoId, value, comment);
        }

        [HttpGet("UpdateTimeVideo")]
        public IActionResult UpdateTimeVideo(int videoId, int videoTime)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.UpdateTimeVideo(currentUserId, videoId, videoTime);
        }

        [HttpGet("GetDetailAssessCourse")]
        public IActionResult GetDetailAssessCourse(int courseId)
        {
            var currentUserId = GetCurrentUserId();
            return _courseService.GetDetailAssessCourse(courseId, currentUserId);
        }
    }
}
