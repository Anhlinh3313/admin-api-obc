using System.Threading.Tasks;
using Core.Business.ViewModels.Region;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IRegionService
    {
        JsonResult GetListRegionAsync(string keySearch, string province, int pageNum, int pageSize);
        Task<JsonResult> GetDetailRegionAsync(int id);
        Task<JsonResult> CreateRegionAsync(RegionViewModelCreate model);
        Task<JsonResult> UpdateRegionAsync(RegionViewModelCreate model);
        Task<JsonResult> DeEnabledRegionAsync(int regionId);
        Task<JsonResult> DeActiveRegionAsync(int regionId);
        Task<JsonResult> GetAllRegionAsync(string keySearch, string province);
        JsonResult GetAllRegion(string keySearch);
    }
}
