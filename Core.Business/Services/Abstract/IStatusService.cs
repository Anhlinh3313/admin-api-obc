using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IStatusService
    {
        Task<JsonResult> GetAllStatusTransactionAsync();
        Task<JsonResult> GetStatusCustomerInformationAsync();
    }
}
