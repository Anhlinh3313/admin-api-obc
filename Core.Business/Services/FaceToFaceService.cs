using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.FaceToFace;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EnumData = Core.Business.ViewModels.EnumData;

namespace Core.Business.Services
{
    public class FaceToFaceService : BaseService, IFaceToFaceService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IFileService _fileService;
        public INotifyService _notifyService;
        public FaceToFaceService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            INotifyService notifyService,
            IGeneralService iGeneralRawService,
            IFileService fileService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _fileService = fileService;
            _notifyService = notifyService;
        }

        public JsonResult GetListFaceToFace(string keySearch, DateTime fromDate, DateTime toDate, string type, int statusId, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListFaceToFace>()
                    .ExecProcedure(Proc_GetListFaceToFace.GetEntityProc(keySearch, fromDate, toDate, type, statusId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllStatusFaceToFace()
        {
            try
            {
                var result = _unitOfWork.RepositoryR<StatusFaceToFaceAndGuests>().FindBy(x => x.IsEnabled == true).ToList();
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateFaceToFace(FaceToFaceViewModelCreate model, int customerId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    if (string.IsNullOrEmpty(model.Location) || string.IsNullOrWhiteSpace(model.Location))
                        return JsonUtil.Error(ValidatorMessage.FaceToFace.LocationNotEmpty);
                    if (model.ExchangeTime <= DateTime.Now)
                        return JsonUtil.Error(ValidatorMessage.FaceToFace.ExchangeTime);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Location) || string.IsNullOrWhiteSpace(model.Location))
                        return JsonUtil.Error(ValidatorMessage.FaceToFace.LocationNotEmptyEnglish);
                    if (model.ExchangeTime <= DateTime.Now)
                        return JsonUtil.Error(ValidatorMessage.FaceToFace.ExchangeTimeEnglish);
                }
                

                model.Location = model.Location.Trim();

                FaceToFace faceToFace = new FaceToFace()
                {
                    CustomerId = customerId,
                    ExchangeTime = model.ExchangeTime,
                    Location = model.Location,
                    Description = model.Description,
                    ReceiverId = model.ReceiverId,
                    Type = model.Type,
                    StatusFaceToFaceId = (int) EnumData.StatusFaceToFaceAndGuests.Pending,
                };

                _unitOfWork.RepositoryCRUD<FaceToFace>().Insert(faceToFace);
                await _unitOfWork.CommitAsync();


                string languageReceiverId = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == model.ReceiverId).Language;
                if (languageReceiverId != null)
                {
                    if (languageReceiverId.Equals("vi"))
                    {
                        var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                        var notify = _notifyService.CreateNotify(model.ReceiverId,
                            string.Format(ValidatorMessage.ContentNotify.FaceToFaceFor, customerName),
                            (int)EnumData.NotifyType.FaceToFace, faceToFace.Id, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }

                        
                    }
                    else
                    {
                        var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                        var notify = _notifyService.CreateNotify(model.ReceiverId,
                            string.Format(ValidatorMessage.ContentNotify.FaceToFaceForEnglish, customerName),
                            (int)EnumData.NotifyType.FaceToFace, faceToFace.Id, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }

                        
                    }
                }
                else
                {
                    var customerName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).FullName;
                    var notify = _notifyService.CreateNotify(model.ReceiverId,
                        string.Format(ValidatorMessage.ContentNotify.FaceToFaceFor, customerName),
                        (int)EnumData.NotifyType.FaceToFace, faceToFace.Id, null, null);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }

                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    var statusName = _unitOfWork.RepositoryR<StatusFaceToFaceAndGuests>()
                        .GetSingle(x => x.Id == faceToFace.StatusFaceToFaceId);

                    return JsonUtil.Success(new
                    {
                        ReceiverName = model.ReceiverName,
                        Location = model.Location,
                        ExchangeTime = model.ExchangeTime,
                        Description = model.Description,
                        StatusName = statusName.Name
                    });
                }
                else
                {
                    var statusName = _unitOfWork.RepositoryR<StatusFaceToFaceAndGuests>()
                        .GetSingle(x => x.Id == faceToFace.StatusFaceToFaceId);

                    return JsonUtil.Success(new
                    {
                        ReceiverName = model.ReceiverName,
                        Location = model.Location,
                        ExchangeTime = model.ExchangeTime,
                        Description = model.Description,
                        StatusName = statusName.Code
                    });
                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetFaceToFaceReceiver(int customerId, int faceToFaceId)
        {
            try
            {
                var faceToFace = _unitOfWork.RepositoryR<FaceToFace>().GetSingle(x => x.Id == faceToFaceId);
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == faceToFace.CustomerId);

                var statusName = _unitOfWork.RepositoryR<StatusFaceToFaceAndGuests>()
                    .GetSingle(x => x.Id == faceToFace.StatusFaceToFaceId);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    return JsonUtil.Success(new
                    {
                        FaceToFaceId = faceToFaceId,
                        GiverId = customer.Id,
                        GiverName = customer.FullName,
                        Location = faceToFace.Location,
                        ExchangeTime = faceToFace.ExchangeTime,
                        Description = faceToFace.Description,
                        StatusId = faceToFace.StatusFaceToFaceId,
                        StatusName = statusName.Name,
                        ImagePathReceive = faceToFace.ImagePathReceive,
                        ReasonCancel = faceToFace.ReasonCancel,
                        ActionTypeId = 4
                    });
                }
                else
                {
                    return JsonUtil.Success(new
                    {
                        FaceToFaceId = faceToFaceId,
                        GiverId = customer.Id,
                        GiverName = customer.FullName,
                        Location = faceToFace.Location,
                        ExchangeTime = faceToFace.ExchangeTime,
                        Description = faceToFace.Description,
                        StatusId = faceToFace.StatusFaceToFaceId,
                        StatusName = statusName.Code,
                        ImagePathReceive = faceToFace.ImagePathReceive,
                        ReasonCancel = faceToFace.ReasonCancel,
                        ActionTypeId = 4
                    });
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetFaceToFaceGive(int customerId, int faceToFaceId)
        {
            try
            {
                var faceToFace = _unitOfWork.RepositoryR<FaceToFace>().GetSingle(x => x.Id == faceToFaceId);
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == faceToFace.ReceiverId);

                var statusName = _unitOfWork.RepositoryR<StatusFaceToFaceAndGuests>()
                    .GetSingle(x => x.Id == faceToFace.StatusFaceToFaceId);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    return JsonUtil.Success(new
                    {
                        ReceiverId = customer.Id,
                        ReceiverName = customer.FullName,
                        Location = faceToFace.Location,
                        ExchangeTime = faceToFace.ExchangeTime,
                        Description = faceToFace.Description,
                        StatusId = faceToFace.StatusFaceToFaceId,
                        StatusName = statusName.Name,
                        ImagePathGive = faceToFace.ImagePathGive,
                        ReasonCancel = faceToFace.ReasonCancel,
                        ActionTypeId = 3
                    });
                }
                else
                {
                    return JsonUtil.Success(new
                    {
                        ReceiverId = customer.Id,
                        ReceiverName = customer.FullName,
                        Location = faceToFace.Location,
                        ExchangeTime = faceToFace.ExchangeTime,
                        Description = faceToFace.Description,
                        StatusId = faceToFace.StatusFaceToFaceId,
                        StatusName = statusName.Code,
                        ImagePathGive = faceToFace.ImagePathGive,
                        ReasonCancel = faceToFace.ReasonCancel,
                        ActionTypeId = 3
                    });
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult AcceptOrCancel(int faceToFaceId, int confirm, string reasonCancel, int customerId)
        {
            try
            {
                var faceToFace = _unitOfWork.RepositoryR<FaceToFace>().GetSingle(x => x.Id == faceToFaceId);
                var receiverName = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == faceToFace.ReceiverId).FullName;

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    if (confirm == 0) // Đồng ý
                    {
                        faceToFace.StatusFaceToFaceId = (int)EnumData.StatusFaceToFaceAndGuests.Accept;

                        var notify = _notifyService.CreateNotify(faceToFace.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.FaceToFaceSuccess, receiverName),
                            (int)EnumData.NotifyType.FaceToFaceSuccess, faceToFaceId, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }
                    else // Từ chối
                    {
                        var notify = _notifyService.CreateNotify(faceToFace.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.FaceToFaceCancel, receiverName),
                            (int)EnumData.NotifyType.FaceToFace, faceToFaceId, faceToFace.CustomerId, reasonCancel);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                        faceToFace.StatusFaceToFaceId = (int)EnumData.StatusFaceToFaceAndGuests.Cancel;
                        faceToFace.ReasonCancel = reasonCancel;
                    }
                }
                else
                {
                    if (confirm == 0) // Đồng ý
                    {
                        faceToFace.StatusFaceToFaceId = (int)EnumData.StatusFaceToFaceAndGuests.Accept;

                        var notify = _notifyService.CreateNotify(faceToFace.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.FaceToFaceSuccessEnglish, receiverName),
                            (int)EnumData.NotifyType.FaceToFaceSuccess, faceToFaceId, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }
                    else // Từ chối
                    {
                        var notify = _notifyService.CreateNotify(faceToFace.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.FaceToFaceCancelEnglish, receiverName),
                            (int)EnumData.NotifyType.FaceToFace, faceToFaceId, faceToFace.CustomerId, reasonCancel);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                        faceToFace.StatusFaceToFaceId = (int)EnumData.StatusFaceToFaceAndGuests.Cancel;
                        faceToFace.ReasonCancel = reasonCancel;
                    }
                }

                
                _unitOfWork.RepositoryCRUD<FaceToFace>().Update(faceToFace);
                _unitOfWork.Commit();

                return GetFaceToFaceReceiver(customerId,faceToFaceId);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadFileAfterMeeting(UploadFileFaceToFaceViewModel model, int customerId)
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

                var faceToFace = await _unitOfWork.RepositoryR<FaceToFace>().GetSingleAsync(x => x.Id == model.FaceToFaceId);
                var tmp = "";
                for (int i = 0; i < (6 - model.FaceToFaceId.ToString().Length); i++)
                {
                    tmp += "0";
                }
                var code = "F2F" + String.Format("{0:MM}", faceToFace.CreatedWhen.GetValueOrDefault()) +
                           faceToFace.CreatedWhen.GetValueOrDefault().Year + tmp + model.FaceToFaceId;
                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "FaceToFace",code);
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data").GetValue(uploadFile.Value, null);
                var link = (dynamic)value;
                if (customerId == faceToFace.CustomerId)
                {
                    faceToFace.ImagePathGive = link;
                }

                if (customerId == faceToFace.ReceiverId)
                {
                    faceToFace.ImagePathReceive = link;
                }
                //faceToFace.ImagePathReceive = link;
                _unitOfWork.RepositoryCRUD<FaceToFace>().Update(faceToFace);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
