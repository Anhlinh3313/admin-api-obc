using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Core.Business.ViewModels.Notify;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface INotifyService
    {
        JsonResult CreateNotify(int customerId, string content, int notifyTypeId, int actionTypeId, int? customerCancelId, string reasonCancel);
        Task<JsonResult> SeenNotify(int? notifyId, int customerId);
        JsonResult GetListNotify(int customerId, int pageNum, int pageSize);
        JsonResult SumUnSeenNotify(int customerId);

        JsonResult CreateNotifyWhenDeActiveCustomer(int customerId);
        JsonResult CreateNotifyWhenCustomerLogInDeviceDifferent(int customerId);
    }
}
