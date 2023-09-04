 using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
 using Core.Business.ViewModels.Thanks;
 using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IThanksService
    {
        JsonResult GetListThanks(string keySearch, DateTime fromDate, DateTime toDate, string type, int pageNum, int pageSize);
        Task<JsonResult> CreateThanks(ThanksViewModelCreate model, int customerId);
        JsonResult GetThanksReceiver(int thanksId, int customerId);
        JsonResult GetThanksGive(int thanksId, int customerId);
    }
}
