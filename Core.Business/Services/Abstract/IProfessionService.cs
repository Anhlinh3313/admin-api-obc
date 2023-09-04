using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IProfessionService
    {
        //Task<JsonResult> GetListProvinceAsync(string keySearch, int pageNum, int pageSize);
        //Task<JsonResult> GetDetailProvinceAsync(int id);
        //Task<JsonResult> CreateProvinceAsync(ParticipatingProvinceViewModel model);
        //Task<JsonResult> UpdateProvinceAsync(ParticipatingProvinceViewModel model);
        //Task<JsonResult> DeEnabledProvinceAsync(int provinceId);
        //Task<JsonResult> DeActiveProvinceAsync(int provinceId);
        Task<JsonResult> DropdownProfessionAsync(string keySearch, string language);
        Task<JsonResult> DropdownProfessionFieldOperationsAsync(string keySearch);
    }
}
