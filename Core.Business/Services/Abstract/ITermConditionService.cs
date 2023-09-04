using System.Threading.Tasks;
using Core.Business.ViewModels.TermCondition;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface ITermConditionService
    {
        Task<JsonResult> GetListTermConditionAsync();
        Task<JsonResult> CreateTermConditionAsync(TermConditionViewModel model);
    }
}
