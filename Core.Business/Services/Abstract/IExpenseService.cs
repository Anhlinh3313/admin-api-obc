using System.Threading.Tasks;
using Core.Business.ViewModels.Expense;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IExpenseService
    {
        Task<JsonResult> GetListExpenseAsync(string keySearch, int pageNum, int pageSize);
        Task<JsonResult> GetDetailExpenseAsync(int id);
        Task<JsonResult> CreateExpenseAsync(ExpenseViewModel model);
        Task<JsonResult> UpdateExpenseAsync(ExpenseViewModel model);
        Task<JsonResult> DeEnabledExpenseAsync(int expenseId);
        Task<JsonResult> DeActiveExpenseAsync(int expenseId);
        Task<JsonResult> GetAllExpense();
        JsonResult GetAllExpenseNotEnabled();
    }
}
