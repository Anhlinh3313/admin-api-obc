 using System;
 using System.Collections.Generic;
 using System.Threading.Tasks;
 using Core.Business.ViewModels.Course;
 using Core.Business.ViewModels.ParticipatingProvince;
 using Core.Business.ViewModels.Thanks;
 using Core.Entity.Entities;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface ICourseService
    {
        Task<JsonResult> GetListCourse(string keySearch, string fromDateStart, string toDateStart, string fromDateEnd, string toDateEnd,
            string objects, int pageNum, int pageSize);
        Task<JsonResult> DeActiveCourse(int courseId);
        Task<JsonResult> DeEnabledCourse(int courseId);
        //Task<JsonResult> DeEndCourse(int CourseId);
        Task<JsonResult> GetDetailCourse(int courseId);
        Course DetailCourse(int id);
        Task<JsonResult> CreateCourse(CourseViewModelCreate model);
        Task<JsonResult> UpdateCourse(CourseViewModelUpdate model);
        Task<JsonResult> UploadImageCourse(CourseViewModelUploadImage model);

        Task<JsonResult> UploadVideoCourse(CourseViewModelUploadImage model);
        JsonResult UpdateImageCourse(CourseViewModelUpdateFile model);

        JsonResult GetListCustomerCourse(int courseId, string keySearch, int typeId, int status, int pageNum, int pageSize);
        JsonResult UpdateNoteCustomerCourse(int customerCourseId, string note);
        //JsonResult GetAllStatusCustomerCourse();
        JsonResult GetAllCourseFee(int type);

        JsonResult GetListTransactionCourse(int courseId, string keySearch, DateTime fromDate, DateTime toDate,
            int chapterId, int statusId, int pageNum, int pageSize);

        Task<JsonResult> ActiveTransactionCourse(int transactionCourseId, int active, string note, int customerId);

        JsonResult GetListAssess(int courseId);

        JsonResult GetListCourseMobile(string keySearch, int customerId, int courseType, int pageNum, int pageSize);
        JsonResult GetDetailCourseMobile(int customerId, string courseCode);
        JsonResult LikedCourse(int customerId, int courseId);
        JsonResult SharedCourse(int customerId, int courseId);
        JsonResult CheckCustomerRegisterCourse(int customerId, int courseId);
        Task<JsonResult> RegisterCourse(int customerId, int courseId, string phoneNumber, string email);
        Task<JsonResult> UploadImageTransactionCourse(CourseViewModelUploadImageTransaction model, int customerId);
        JsonResult AssessCourse(int customerId, int courseId, int value, string comment);
        Task<JsonResult> UploadCertificate(CourseViewModelUploadCertificate model, int customerId);
        JsonResult GetCertificateWithCustomerId(int customerId, int courseId);

        JsonResult GetListVideoMobile(string keySearch, int customerId, int videoType, int pageNum, int pageSize);
        Task<JsonResult> UploadImageTransactionVideo(VideoViewModelUploadImageTransaction model, int customerId);
        JsonResult GetListAssessMobile(int courseId, int pageNum, int pageSize);
        JsonResult UpdateTimeVideo(int customerId, int videoId, int timeVideo);
        JsonResult GetDetailVideoMobile(int customerId, string videoCode);
        Task<JsonResult> GenerateInformationQrCodePath(FileStreamResult streamResult, int courseId);

        JsonResult GetDetailAssessCourse(int courseId, int customerId);
    }
}
