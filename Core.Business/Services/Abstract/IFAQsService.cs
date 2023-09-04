using System.Threading.Tasks;
using Core.Business.ViewModels.FAQs;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IFAQsService
    {
        Task<JsonResult> GetListFAQsAsync(string keySearch, int pageNum, int pageSize);
        Task<JsonResult> GetDetailFAQsAsync(int id);
        Task<JsonResult> CreateFAQsAsync(FAQsViewModel model);
        Task<JsonResult> UpdateFAQsAsync(FAQsViewModel model);
        Task<JsonResult> DeEnabledFAQsAsync(int faqId);
        Task<JsonResult> DeActiveFAQsAsync(int faqId);
        Task<JsonResult> GetListFAQsMobile(string keySearch, int pageNum, int pageSize);
        Task<JsonResult> GetAllFAQsMobile(string keySearch);
    }
}
