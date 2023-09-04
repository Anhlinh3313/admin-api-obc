using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface ICustomerService
    {
        Task<JsonResult> GetDetailProfileCustomerAsync(int id);
        Task<JsonResult> UpdateCustomerProfileAsync(CustomerViewModel model);
        Task<JsonResult> AcceptPremium(int id, string note, int? active);
        Task<JsonResult> AcceptChapter(int id, string note, int? active, int customerId);
        Task<JsonResult> CancelMember(int id, string note);

        JsonResult GetListCustomerWaitingActive(int id, string keySearch, DateTime fromDate, DateTime toDate,
            int chapterId, int statusId, int pageNum, int pageSize);

        JsonResult GetListCustomerMember(int chapterId, string keySearch, string fieldOperations, string status, int pageNum,
            int pageSize);
        Task<JsonResult> ChangeCustomerRole(int customerId, int? customerRoleId);
        Task<JsonResult> GetListRoleMemberChapter();
        Task<JsonResult> GetInformationCustomer(int id);
        JsonResult GetListCustomerOutOfChapter(int customerId, string fullName, string businessName, string fieldOperationsName, string provinceName, string keySearch, int type);
        JsonResult GetContractCustomer(int id);
        Task<JsonResult> GetCustomerProfile(int id, int currentUserId, string phoneNumber);
        Task<JsonResult> UploadAvatarCustomer(CustomerViewModelUploadAvatar model, int customerId);
        JsonResult GetCustomerOutOfChapterWithQrCode(int customerId);
        //JsonResult GetQrCodeCustomer(int customerId);
        JsonResult UpdateExpoPushTokenCustomer(int customerId, string expoPushToken, string language);
        JsonResult DeleteExpoPushTokenCustomer(int customerId);

        JsonResult GetIndicators(int customerId);
        JsonResult AssessCustomer(int customerId, int value, string comment);

        JsonResult GetListTopCustomer(int customerId,int pageNum);
        JsonResult UpdateLanguageCustomer(int customerId, string language);
        JsonResult CheckCustomer(int customerId, string expoPushToken);
        JsonResult GetAllAssessCustomer(int customerId, int pageNum, int pageSize);
        JsonResult GetAvgAssessCustomer(int customerId);
        JsonResult SendMailOTPChangeEmailCustomer(int customerId, string newEmail, int sendAgain);
        Task<JsonResult> SendOTPByPhoneChangePhoneCustomer(int customerId, string newPhoneNumber, int sendAgain);
        JsonResult CheckCodeValidChangEmailOrPhoneCustomer(int customerId, string code, string type);
    }
}
