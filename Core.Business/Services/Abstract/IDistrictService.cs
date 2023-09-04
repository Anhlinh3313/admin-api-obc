using System.Threading.Tasks;
using Core.Business.ViewModels.Region;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IDistrictService
    {
        Task<JsonResult> GetAllDistrictAsync(string keySearch, int provinceId);
    }
}
