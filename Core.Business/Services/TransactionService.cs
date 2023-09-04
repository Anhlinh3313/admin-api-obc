using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.General;
using Core.Business.ViewModels.Transaction;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EnumData = Core.Business.ViewModels.EnumData;

namespace Core.Business.Services
{
    public class TransactionService : BaseService, ITransactionService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IFileService _fileService;
        public TransactionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService,
            IFileService fileService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _fileService = fileService;
        }

        public JsonResult GetListHistoryTransaction(string keySearch, int customerId, int actionId, int month, int year, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListHistoryTransaction>()
                    .ExecProcedure(Proc_GetListHistoryTransaction.GetEntityProc(keySearch ,customerId, actionId, month, year, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateTransaction(TransactionViewModelCreate model, int customerId)
        {
            try
            {
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (language.Equals("en"))
                {
                    if (model.File == null || model.File.Length == 0)
                        return JsonUtil.Error("File not selected");
                    else
                    {
                        if (model.File.ContentType.Split('/')[0] != "image")
                        {
                            return JsonUtil.Error("Only pictures allowed, please choose again!");
                        }

                        if (model.File.Length > 10000000)
                        {
                            return JsonUtil.Error("File size exceeds the allowed limit, please choose again!");
                        }
                    }
                }
                else
                {
                    if (model.File == null || model.File.Length == 0)
                        return JsonUtil.Error("Vui lòng chọn tệp");
                    else
                    {
                        if (model.File.ContentType.Split('/')[0] != "image")
                        {
                            return JsonUtil.Error("Chỉ cho phép hình ảnh, vui lòng chọn lại!");
                        }

                        if (model.File.Length > 10000000)
                        {
                            return JsonUtil.Error("Dung lượng tệp quá giới hạn cho phép, vui lòng chọn lại!");
                        }
                    }
                }

                var expense = await _unitOfWork.RepositoryR<Expense>().GetSingleAsync(x => x.Id == model.ExpenseId && x.IsActive == true);
                if (expense == null) return JsonUtil.Error(ValidatorMessage.Expense.NotActiveOrEnable);
                var customer = await _unitOfWork.RepositoryR<Customer>().GetSingleAsync(x => x.Id == customerId);
                if (customer.StatusId != (int) EnumData.CustomerStatusEnum.AcceptedChapter)
                    return JsonUtil.Error(ValidatorMessage.Customer.NotAcceptChapter);
                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId;
                var name = customer.FullName + " mua gói " + expense.Name;
                Transaction transaction = new Transaction()
                {
                    ExpenseId = model.ExpenseId,
                    CustomerId = customerId,
                    Name = name,
                    StatusTransactionId = (int) EnumData.TransactionStatusEnum.PendingActive,
                    ChapterId = chapterId
                };

                _unitOfWork.RepositoryCRUD<Transaction>().Insert(transaction);
                await _unitOfWork.CommitAsync();
                var tmp = "";
                for (int i = 0; i < (6 - transaction.Id.ToString().Length); i++)
                {
                    tmp += "0";
                }
                
                var code = "PG" + String.Format("{0:MM}", transaction.CreatedWhen.GetValueOrDefault()) +
                           transaction.CreatedWhen.GetValueOrDefault().Year + tmp + transaction.Id;
                transaction.Code = code;

                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "Transaction",
                        code);
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;
                transaction.ImagePath = link;
                _unitOfWork.RepositoryCRUD<Transaction>().Update(transaction);
                await _unitOfWork.CommitAsync();

                customer.StatusId = (int) EnumData.CustomerStatusEnum.PendingActive;
                _unitOfWork.RepositoryCRUD<Customer>().Update(customer);
                await _unitOfWork.CommitAsync();

                
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllTransactionAction(int customerId)
        {
            try
            {

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    var result = new List<TransactionActionViewModel>()
                    {
                        new TransactionActionViewModel(){ActionName = "Tất cả", ActionId = 0},
                        new TransactionActionViewModel(){ActionName = "Cơ hội kinh doanh trao đi", ActionId = 1},
                        new TransactionActionViewModel(){ActionName = "Cơ hội kinh doanh nhận được", ActionId = 2},
                        new TransactionActionViewModel(){ActionName = "Face - to - Face (Hẹn gặp)", ActionId = 3},
                        new TransactionActionViewModel(){ActionName = "Face - to - Face (được mời)", ActionId = 4},
                        new TransactionActionViewModel(){ActionName = "Lời cảm ơn - Giá trị trao đi", ActionId = 5},
                        new TransactionActionViewModel(){ActionName = "Lời cảm ơn - Giá trị nhận được", ActionId = 6},
                        new TransactionActionViewModel(){ActionName = "Khách mời", ActionId = 7},
                        new TransactionActionViewModel(){ActionName = "Khoá học", ActionId = 8},
                        new TransactionActionViewModel(){ActionName = "Video", ActionId = 9},
                        new TransactionActionViewModel(){ActionName = "Đi thay", ActionId = 10},
                        new TransactionActionViewModel(){ActionName = "Vắng theo yêu cầu y tế", ActionId = 11}
                    };

                    return JsonUtil.Success(result);
                }
                else
                {
                    var result = new List<TransactionActionViewModel>()
                    {
                        new TransactionActionViewModel(){ActionName = "All", ActionId = 0},
                        new TransactionActionViewModel(){ActionName = "Referral - Transfer", ActionId = 1},
                        new TransactionActionViewModel(){ActionName = "Referral - Receive", ActionId = 2},
                        new TransactionActionViewModel(){ActionName = "Face to Face - Transfer", ActionId = 3},
                        new TransactionActionViewModel(){ActionName = "Face  to  Face - Receive", ActionId = 4},
                        new TransactionActionViewModel(){ActionName = "Thank you For the closed Business - Transfer", ActionId = 5},
                        new TransactionActionViewModel(){ActionName = "Thank you For the closed Business - Receive", ActionId = 6},
                        new TransactionActionViewModel(){ActionName = "Visitors", ActionId = 7},
                        new TransactionActionViewModel(){ActionName = "Courses", ActionId = 8},
                        new TransactionActionViewModel(){ActionName = "Video", ActionId = 9},
                        new TransactionActionViewModel(){ActionName = "Go instead", ActionId = 10},
                        new TransactionActionViewModel(){ActionName = "Absence on medical request", ActionId = 11}
                    };

                    return JsonUtil.Success(result);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetTransactionPending(int customerId)
        {
            try
            {
                var transaction = _unitOfWork.RepositoryR<Transaction>().FindBy(x => x.CustomerId == customerId)
                    .OrderByDescending(x => x.Id).FirstOrDefault();
                return JsonUtil.Success(new
                {
                    TransactionCode = transaction.Code,
                    TransactionCreatedWhen = transaction.CreatedWhen
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
