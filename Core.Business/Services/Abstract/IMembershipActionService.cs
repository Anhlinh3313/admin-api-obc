using System.Threading.Tasks;
using Core.Business.ViewModels.MembershipAction;
using Core.Business.ViewModels.ParticipatingProvince;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IMembershipActionService
    {
        Task<JsonResult> GetMembershipActionWithCustomerId(int id);
        Task<JsonResult> GetMembershipWithExpenseId(int id);
        Task<JsonResult> CreateMembershipAction(MembershipActionViewModel model);
        JsonResult GetAllCustomerExpired();
        JsonResult CheckMembershipActionExpired();
    }
}
