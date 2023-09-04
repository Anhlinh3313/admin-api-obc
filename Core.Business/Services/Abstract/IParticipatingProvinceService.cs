using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IParticipatingProvinceService
    {
        Task<JsonResult> GetListProvinceAsync(string keySearch, int pageNum, int pageSize);
        Task<JsonResult> GetDetailProvinceAsync(int id);
        Task<JsonResult> CreateProvinceAsync(ParticipatingProvinceViewModel model);
        Task<JsonResult> UpdateProvinceAsync(ParticipatingProvinceViewModel model);
        Task<JsonResult> DeEnabledProvinceAsync(int provinceId);
        Task<JsonResult> DeActiveProvinceAsync(int provinceId);
        Task<JsonResult> DropdownProvinceAsync(string keySearch);
        Task<JsonResult> GetProvinceAndRegionWithChapterId(int chapterId);
        Task<JsonResult> GetProvinceTreeViewAsync(string keySearch);
    }
}
