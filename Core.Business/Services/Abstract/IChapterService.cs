using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IChapterService
    {
        JsonResult GetListChapterAsync(string keySearch, string province, string region, int pageNum, int pageSize);
        Task<JsonResult> GetDetailChapterAsync(int id);
        JsonResult GetChapterInformation(int id);
        Task<JsonResult> CreateChapterAsync(ChapterViewModelCreate model);
        Task<JsonResult> UpdateChapterAsync(ChapterViewModelCreate model);
        Task<JsonResult> DeEnabledChapterAsync(int chapterId);
        Task<JsonResult> DeActiveChapterAsync(int chapterId);
        Task<JsonResult> GetChapterWithRegionIdAsync(int regionId, string keySearch);
        Task<JsonResult> GetChapterMobile(string keySearch);
        Task<JsonResult> CheckFieldOperationUnique(int fieldOperationId, int chapterId);
        JsonResult GetDetailMemberChapter(int businessId);
        JsonResult GenerateQrCodeCustomer(FileStreamResult streamResult, int chapterId);
        JsonResult GetInformationChapterInGuests(int customerId);
    }
}
