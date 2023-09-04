using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Core.Business.ViewModels.MeetingChapter;
using Core.Entity.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IMeetingChapterService
    {
        Task<JsonResult> CreateMeetingChapter(MeetingChapterViewModel model, int customerId);
        Task<JsonResult> UpdateEnabledMeetingChapter(MeetingChapter meetingChapter);
        Task<JsonResult> GenerateQrCodePath(FileStreamResult streamResult, int meetingChapterId);
        Task<JsonResult> GetListMeetingChapter(int customerId);
        Task<JsonResult> GetAllMeetingChapterExpired();
        JsonResult GetDetailMeetingChapter(int meetingChapterId, int currentUserId, int isEdit);
        JsonResult GetMeetingChapterWithChapterId(int chapterId);
        JsonResult MeetingChapterCheckInCustomer(int meetingChapterId, int customerId);
        JsonResult GetLoopMeetingChapter(int currentUserId);
        JsonResult GetListMeetingChapterInGuests(int customerId);
        JsonResult EditMeetingChapter(int meetingChapterId, string form, string link, string address);
        JsonResult EndMeetingChapter(int meetingChapterId);
    }
}
