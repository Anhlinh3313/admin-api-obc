using System.Threading.Tasks;
using Core.Business.ViewModels.Introduce;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IIntroduceService
    {
        Task<JsonResult> GetListIntroduceAsync();
        Task<JsonResult> CreateIntroduceAsync(IntroduceViewModel model);
    }
}
