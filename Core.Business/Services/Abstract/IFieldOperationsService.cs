using System.Threading.Tasks;
using Core.Business.ViewModels.Region;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IFieldOperationsService
    {
        Task<JsonResult> GetAllFieldOperationsAsync(string keySearch, int? professionId, int customerId);
        Task<JsonResult> GetAllFieldOperationsInWebAsync(string keySearch, int? professionId);
    }
}
