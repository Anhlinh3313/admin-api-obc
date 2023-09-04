using System.Threading.Tasks;
using Core.Business.ViewModels.Business;
using Core.Business.ViewModels.Customer;
using Core.Business.ViewModels.User;
using Core.Entity.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IBusinessService
    {
        JsonResult GetListBusiness(string keySearch, string province, string profession, string fieldOperation, int customerRole, int pageNum, int pageSize);
        JsonResult GetDetailBusinessAsync(int id);
        Task<JsonResult> GetDetailProfileBusinessAsync(int id);
        Task<JsonResult> GetDetailBusinessByCustomerIdAsync(int id);
        Task<JsonResult> CreateBusinessAsync(BusinessViewModelCreateAdmin model, int userId);
        Task<JsonResult> UpdateBusinessAsync(BusinessViewModelCreateAdmin model, int userId);
        Task<JsonResult> UpdateProfileBusinessAsync(BusinessViewModel model);
        Task<JsonResult> DeActiveBusinessAsync(int businessId);
        Task<JsonResult> DeEnableBusinessAsync(int businessId);
        Task<JsonResult> DropdownCustomerRole();
        JsonResult GetListBusinessPendingAsync(string keySearch, int status, int customerId, int pageNum, int pageSize);
        Task<JsonResult> SubscribeChapter(BusinessViewModelCreateMobile model);
        Task<JsonResult> UploadAvatarBusiness(BusinessViewModelUploadAvatar model, int customerId);
        JsonResult GenerateQrCodeCustomer(FileStreamResult streamResult, int customerId);
        BusinessViewModelTextQrCode GetCustomer(int customerId);
        JsonResult ChangeProfessionBusiness(int customerId, int professionId, int fieldOperationsId);
        JsonResult ChangeFieldOperationsBusiness(int customerId, int fieldOperationsId);
        JsonResult ChangeProfessionAndFieldOperationsBusiness(int customerId, int active, string note, int currentId);
    }
}
