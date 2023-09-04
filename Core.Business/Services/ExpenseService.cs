using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Core.Business.ViewModels.Expense;

namespace Core.Business.Services
{
    public class ExpenseService : BaseService, IExpenseService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ExpenseService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> GetListExpenseAsync(string keySearch, int pageNum, int pageSize)
        {
            try
            {
                var listExpense = await _unitOfWork.RepositoryR<Expense>()
                    .FindBy(x => x.IsEnabled == true &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Name.ToLower().Contains(keySearch.Trim().ToLower()) ||
                                  x.Code.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                var total = listExpense.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var result = listExpense.Skip((pageNum - 1) * pageSize).Take(pageSize).OrderBy(x => x.Id).ToList();
                return JsonUtil.Success(result, "Success", total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailExpenseAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == id);
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateExpenseAsync(ExpenseViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Expense.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Expense.NameNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                if (_unitOfWork.RepositoryR<Expense>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Expense.UniqueCode);
                if (_unitOfWork.RepositoryR<Expense>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Expense.UniqueName);

                if (model.IsActive == null) model.IsActive = true;
                return JsonUtil.Success(await _iGeneralRawService.Create<Expense, ExpenseViewModel>(model));
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateExpenseAsync(ExpenseViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Expense.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Expense.NameNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                var faqOld = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == model.Id);
                if (!faqOld.Code.ToLower().Equals(model.Code.ToLower()))
                    if (_unitOfWork.RepositoryR<Expense>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Expense.UniqueCode);
                if (!faqOld.Name.ToLower().Equals(model.Name.ToLower()))
                    if (_unitOfWork.RepositoryR<Expense>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Expense.UniqueName);

                if (model.IsActive == null) model.IsActive = faqOld.IsActive;
                model.IsEnabled = true;
                return JsonUtil.Success(await _iGeneralRawService.Update<Expense, ExpenseViewModel>(model));
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledExpenseAsync(int expenseId)
        {
            try
            {
                //var membership = await _unitOfWork.RepositoryR<MembershipAction>()
                //    .AnyAsync(z => z.ExpenseId == expenseId);
                //if (membership) return JsonUtil.Error(ValidatorMessage.Expense.NotDestroy);
                var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == expenseId);
                if (expense == null) return JsonUtil.Error(ValidatorMessage.Expense.NotExist);
                expense.IsEnabled = !expense.IsEnabled;
                _unitOfWork.RepositoryCRUD<Expense>().Update(expense);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveExpenseAsync(int expenseId)
        {
            try
            {
                
                var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == expenseId);
                if (expense == null) return JsonUtil.Error(ValidatorMessage.Expense.NotExist);
                //if (expense.IsActive)
                //{
                //    var membership = await _unitOfWork.RepositoryR<MembershipAction>()
                //        .AnyAsync(z => z.ExpenseId == expenseId);
                //    if (membership) return JsonUtil.Error(ValidatorMessage.Expense.NotDeActive);
                //}
                expense.IsActive = !expense.IsActive;
                _unitOfWork.RepositoryCRUD<Expense>().Update(expense);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllExpense()
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<Expense>().FindBy(x => x.IsActive == true && x.IsEnabled == true).ToListAsync();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllExpenseNotEnabled()
        {
            try
            {
                var result =  _unitOfWork.RepositoryR<Expense>().GetAllNotEnabled();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
