 using System;
 using System.Collections.Generic;
 using System.Threading.Tasks;
 using Core.Business.ViewModels.Event;
 using Core.Business.ViewModels.ParticipatingProvince;
 using Core.Business.ViewModels.Thanks;
 using Core.Entity.Entities;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IEventService
    {
        Task<JsonResult> GetListEvent(string keySearch, string fromDateStart, string toDateStart, string fromDateEnd, string toDateEnd,
            string objects, int pageNum, int pageSize);
        Task<JsonResult> DeActiveEvent(int eventId);
        Task<JsonResult> DeEnabledEvent(int eventId);
        Task<JsonResult> DeEndEvent(int eventId);
        Task<JsonResult> GetDetailEvent(int eventId);
        Task<JsonResult> CreateEvent(EventViewModelCreate model);
        Task<JsonResult> UpdateEvent(EventViewModelUpdate model);
        Task<JsonResult> UploadImageEvent(EventViewModelUploadImage model);
        JsonResult UpdateImageEvent(EventViewModelUpdate model);

        JsonResult GetListCustomerEvent(int eventId, string keySearch, int typeId, int status, int pageNum, int pageSize);
        JsonResult UpdateNoteCustomerEvent(int customerEventId, string note);
        JsonResult GetAllStatusCustomerEvent();
        JsonResult GetAllEvent(int type);
        Event GetEvent(int id);
        string GetTimeEvent(int id);

        JsonResult GetListTransactionEvent(int eventId, string keySearch, DateTime fromDate, DateTime toDate,
            int chapterId, int statusId, int pageNum, int pageSize);

        Task<JsonResult> ActiveTransactionEvent(int transactionEventId, int active, string note, int customerId);
        byte[] GenerateByteArray(string url);
        Task<JsonResult> GenerateQrCode(FileStreamResult streamResult, string fileName, string folderName);

        Task<JsonResult> GenerateLinkInformationQrCodePath(FileStreamResult streamResult, int eventId);
        Task<JsonResult> GenerateLinkCheckInQrCodePath(FileStreamResult streamResult, int eventId);

        JsonResult GetListEventMobile(string keySearch, int customerId, int eventType, int pageNum, int pageSize);
        JsonResult LikedEvent(int customerId, int eventId);
        JsonResult SharedEvent(int customerId, int eventId);
        Task<JsonResult> GetDetailEventMobile(int customerId, string eventCode);

        JsonResult CheckCustomerRegisterEvent(int customerId, int eventId);
        Task<JsonResult> RegisterEvent(int customerId, int eventId, string phoneNumber, string email);
        Task<JsonResult> UploadImageTransactionEvent(EventViewModelUploadImageTransaction model, int customerId);

        JsonResult GetListLikedEventAndCourse(string keySearch, int customerId, int pageNum, int pageSize);

        JsonResult CustomerCheckInEvent(string eventCode, int customerId);
        JsonResult GetListRecentEvent(int customerId);
    }
}
