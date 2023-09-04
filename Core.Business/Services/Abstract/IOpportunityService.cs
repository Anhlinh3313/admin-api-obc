using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.Opportunity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IOpportunityService
    {
        JsonResult GetListOpportunity(string keySearch, DateTime fromDate, DateTime toDate, string type, int pageNum, int pageSize);
        Task<JsonResult> CreateOpportunity(OpportunityViewModelCreate model, int customerId);
        JsonResult ChangeStatusOpportunity(int id, int statusId, string note, int customerId);
        JsonResult GetOpportunityReceiver(int customerId ,int opportunityId);

        JsonResult GetOpportunityGive(int customerId, int opportunityId);
        JsonResult GetStatusOpportunity(int customerId);
    }
}
