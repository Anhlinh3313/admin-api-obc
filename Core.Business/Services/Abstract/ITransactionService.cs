using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
using Core.Business.ViewModels.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface ITransactionService
    {
        JsonResult GetListHistoryTransaction(string keySearch, int customerId, int actionId, int month, int year, int pageNum, int pageSize);
        Task<JsonResult> CreateTransaction(TransactionViewModelCreate model, int customerId);

        JsonResult GetAllTransactionAction(int customerId);
        JsonResult GetTransactionPending(int customerId);
    }
}
