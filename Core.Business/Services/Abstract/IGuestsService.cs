using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.Guests;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IGuestsService
    {
        JsonResult GetListGuests(string keySearch, DateTime fromDate, DateTime toDate, int statusId,int chapterId, int pageNum, int pageSize);
        JsonResult GetAllStatusGuests();
        Task<JsonResult> CreateGuests(GuestsViewModelCreate model, int customerId);
        JsonResult CheckInGuests(int guestsId, int checkIn);
        JsonResult GetDetailGuests(int guestsId);
        JsonResult GetListGuestsWithMeetingChapterId(string keySearch ,int meetingChapterId, int pageNum, int pageSize, int currentId);
        Task<JsonResult> CreateGoInstead(GuestsViewModelCreate model, int customerId);
        JsonResult GetDetailGoInstead(int goInsteadId);
    }
}
