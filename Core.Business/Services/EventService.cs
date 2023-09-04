using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Course;
using Core.Business.ViewModels.Event;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QRCoder;

namespace Core.Business.Services
{
    public class EventService : BaseService, IEventService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IFileService _fileService;
        public INotifyService _notifyService;
        public IAccountService _accountService;
        public EventService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IFileService fileService,
            INotifyService notifyService,
            IAccountService accountService,
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _fileService = fileService;
            _notifyService = notifyService;
            _accountService = accountService;
        }

        public async Task<JsonResult> GetListEvent(string keySearch, string fromDateStart, string toDateStart, string fromDateEnd, string toDateEnd,
            string objects, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch))
                    keySearch = keySearch.Trim();
                var listEvent = new List<Event>();
                var listEventId = new List<int>();
                if (string.IsNullOrEmpty(fromDateStart) && string.IsNullOrEmpty(toDateStart) && 
                    string.IsNullOrEmpty(fromDateEnd) && string.IsNullOrEmpty(toDateEnd))
                {
                    listEvent = await _unitOfWork.RepositoryR<Event>().FindBy(x =>
                                                                (keySearch == null ||
                                                                (x.Code.ToLower().Contains(keySearch.ToLower()) ||
                                                                 x.Name.ToLower().Contains(keySearch.ToLower()))) &&
                                                                (objects == null || x.Objects.ToLower().Equals(objects.ToLower()))).ToListAsync();
                } else
                {
                    if (!string.IsNullOrEmpty(fromDateStart) && !string.IsNullOrEmpty(toDateStart))
                    {
                        listEventId = _unitOfWork.RepositoryR<TimeEvent>()
                        .FindBy(y => (y.DateStart >= DateTime.Parse(fromDateStart) && y.DateStart <= DateTime.Parse(toDateStart).AddDays(1)))
                        .Select(x => x.EventId).ToList();
                    }
                    if (!string.IsNullOrEmpty(fromDateEnd) && !string.IsNullOrEmpty(toDateEnd))
                    {
                        listEventId = _unitOfWork.RepositoryR<TimeEvent>()
                        .FindBy(y => (y.DateEnd >= DateTime.Parse(fromDateEnd) && y.DateEnd <= DateTime.Parse(toDateEnd).AddDays(1)))
                        .Select(x => x.EventId).ToList();
                    }

                    if (!string.IsNullOrEmpty(fromDateStart) && !string.IsNullOrEmpty(toDateStart) &&
                        !string.IsNullOrEmpty(fromDateEnd) && !string.IsNullOrEmpty(toDateEnd))
                    {
                        listEventId = _unitOfWork.RepositoryR<TimeEvent>()
                        .FindBy(y => (y.DateStart >= DateTime.Parse(fromDateStart) && y.DateStart <= DateTime.Parse(toDateStart).AddDays(1))
                            || (y.DateEnd >= DateTime.Parse(fromDateEnd) && y.DateEnd <= DateTime.Parse(toDateEnd).AddDays(1)))
                        .Select(x => x.EventId).ToList();
                    }

                    listEvent = await _unitOfWork.RepositoryR<Event>().FindBy(x =>
                                                                (keySearch == null ||
                                                                (x.Code.ToLower().Contains(keySearch.ToLower()) ||
                                                                 x.Name.ToLower().Contains(keySearch.ToLower()))) &&
                                                                 listEventId.Contains(x.Id) &&
                                                                (objects == null || x.Objects.ToLower().Equals(objects.ToLower()))).ToListAsync();
                }
                var total = listEvent.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var tmp = listEvent.Skip((pageNum - 1) * pageSize).Take(pageSize).OrderByDescending(x => x.Id).ToList();
                var result = new List<EventViewModel>();
                foreach (var item in tmp)
                {
                    List<TimeEventMobile> time = new List<TimeEventMobile>();
                    var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == item.Id).ToArray();
                    foreach (var timeEventItem in timeEvent)
                    {
                        var timeItem = new TimeEventMobile();

                        var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                        var hourStart = timeEventItem.DateStart.Hour;
                        var minuteStart = timeEventItem.DateStart.Minute;

                        var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                        var hourEnd = timeEventItem.DateEnd.Hour;
                        var minuteEnd = timeEventItem.DateEnd.Minute;
                        if (dateStartEvent == dateEndEvent)
                        {
                            var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                            var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);

                            timeItem.Date =  timeEventItem.DateStart.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                            var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);

                            var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                            var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);

                            timeItem.Date =  timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                             timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                        }

                        if (hourStart == hourEnd)
                        {
                            if (minuteStart == minuteEnd)
                            {
                                timeItem.Time = timeEventItem.DateStart.ToString("HH:mm");
                            }
                        }
                        else
                        {
                            timeItem.Time = timeEventItem.DateStart.ToString("HH:mm") + " - " +
                                            timeEventItem.DateEnd.ToString("HH:mm");
                        }
                        time.Add(timeItem);
                    }

                    var itemResult = new EventViewModel()
                    {
                        Id = item.Id,
                        IsEnabled = item.IsEnabled,
                        ImagePath = item.ImagePath,
                        IsEnd = item.IsEnd,
                        Code = item.Code,
                        Name = item.Name,
                        IsActive = item.IsActive,
                        Fee = item.Fee,
                        Objects = item.Objects,
                        CreatedWhen = item.CreatedWhen.GetValueOrDefault(),
                        NumberOfAttendees = item.NumberOfAttendees,
                        TimeEvents = time.ToArray()
                    };
                    result.Add(itemResult);
                }
                
                return JsonUtil.Success(result, "Success", total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveEvent(int eventId)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);

                if (detailEvent.IsActive == true)
                {
                    if (_unitOfWork.RepositoryR<TimeEvent>().Any(x => x.EventId == eventId && x.DateEnd >= DateTime.Now))
                    {
                        if (_unitOfWork.RepositoryR<CustomerEvent>().Any(x => x.EventId == eventId))
                        {
                            return JsonUtil.Error(ValidatorMessage.Event.NotDeActive);
                        }
                    }
                }
                else
                {
                    if (detailEvent.IsEnd == true)
                    {
                        return JsonUtil.Error(ValidatorMessage.Event.IsEnd);
                    }

                    
                    if (!_unitOfWork.RepositoryR<TimeEvent>().Any(x => x.DateEnd > DateTime.Now && x.EventId == eventId))
                    {
                        return JsonUtil.Error(ValidatorMessage.Event.NotActive);
                    }
                    
                }

                detailEvent.IsActive = !detailEvent.IsActive;
                _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledEvent(int eventId)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);
                
                if (_unitOfWork.RepositoryR<TimeEvent>().Any(x => x.EventId == eventId && x.DateEnd >= DateTime.Now))
                {
                    if (_unitOfWork.RepositoryR<CustomerEvent>().Any(x => x.EventId == eventId))
                    {
                        return JsonUtil.Error(ValidatorMessage.Event.NotDeEnabled);
                    }
                }
                detailEvent.IsEnabled = !detailEvent.IsEnabled;
                _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEndEvent(int eventId)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);
                if (detailEvent.IsEnd == true) return JsonUtil.Error(ValidatorMessage.Event.EndEvent);
                if (_unitOfWork.RepositoryR<TimeEvent>().Any(x => x.EventId == eventId && x.DateEnd >= DateTime.Now))
                {
                    if (_unitOfWork.RepositoryR<CustomerEvent>().Any(x => x.EventId == eventId))
                    {
                        return JsonUtil.Error(ValidatorMessage.Event.NotDeEnd);
                    }
                }
                detailEvent.IsEnd = true;
                detailEvent.IsActive = false;
                _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailEvent(int eventId)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);
                var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == eventId).ToList();
                if (detailEvent == null) return JsonUtil.Error(ValidatorMessage.Event.NotExist);
                var arrayTimeEvent = new List<TimeEventModel>();
                foreach (var timeEventItem in timeEvent)
                {
                    var timeItem = new TimeEventModel()
                    {
                        DateStart = timeEventItem.DateStart.ToString("yyyy-MM-dd"),
                        TimeStart = timeEventItem.DateStart.ToString("HH:mm"),
                        DateEnd = timeEventItem.DateEnd.ToString("yyyy-MM-dd"),
                        TimeEnd = timeEventItem.DateEnd.ToString("HH:mm")
                    };
                    arrayTimeEvent.Add(timeItem);
                }
                EventViewModelUpdate result = new EventViewModelUpdate()
                {
                    Id = detailEvent.Id,
                    Code = detailEvent.Code,
                    Name = detailEvent.Name,
                    TimeEvents = arrayTimeEvent.ToArray(),
                    Fee = detailEvent.Fee,
                    LinkCheckIn = detailEvent.LinkCheckIn,
                    LinkCheckInQrCodePath = detailEvent.LinkCheckInQrCodePath,
                    LinkInformation = detailEvent.LinkInformation,
                    LinkInformationQrCodePath = detailEvent.LinkInformationQrCodePath,
                    LongDescription = detailEvent.LongDescription,
                    ShortDescription = detailEvent.ShortDescription,
                    Objects = detailEvent.Objects
                };
                if (!string.IsNullOrEmpty(detailEvent.ImagePath))
                {
                    result.ImagePath = detailEvent.ImagePath.Split(",");
                }

                bool isSave = true;
                if (detailEvent.IsEnd == true)
                {
                    isSave = false;
                }
                else
                {
                    if (!_unitOfWork.RepositoryR<TimeEvent>().Any(x => x.DateEnd > DateTime.Now && x.EventId == eventId)) isSave = false;
                }
                return JsonUtil.Success(new
                {
                    Id = result.Id,
                    Code = result.Code,
                    Name = result.Name,
                    TimeEvents = result.TimeEvents,
                    Fee = result.Fee,
                    LinkCheckIn = result.LinkCheckIn,
                    LinkCheckInQrCodePath = result.LinkCheckInQrCodePath,
                    LinkInformation = result.LinkInformation,
                    LinkInformationQrCodePath = result.LinkInformationQrCodePath,
                    LongDescription = result.LongDescription,
                    ShortDescription = result.ShortDescription,
                    Objects = result.Objects,
                    ImagePath = result.ImagePath,
                    Save = isSave
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateEvent(EventViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Event.NameNotEmpty);
                if (!model.TimeEvents.Any())
                    return JsonUtil.Error(ValidatorMessage.Event.NotDateTimeEvent);

                foreach (var modelTimeEvent in model.TimeEvents)
                {
                    if (string.IsNullOrEmpty(modelTimeEvent.DateStart) || string.IsNullOrWhiteSpace(modelTimeEvent.DateStart))
                        return JsonUtil.Error(ValidatorMessage.Event.NotDateStartEvent);
                    if (string.IsNullOrEmpty(modelTimeEvent.DateEnd) || string.IsNullOrWhiteSpace(modelTimeEvent.DateEnd))
                        return JsonUtil.Error(ValidatorMessage.Event.NotDateEndEvent);
                    if (string.IsNullOrEmpty(modelTimeEvent.TimeStart) || string.IsNullOrWhiteSpace(modelTimeEvent.TimeStart))
                        return JsonUtil.Error(ValidatorMessage.Event.NotTimeStartEvent);
                    if (string.IsNullOrEmpty(modelTimeEvent.TimeEnd) || string.IsNullOrWhiteSpace(modelTimeEvent.TimeEnd))
                        return JsonUtil.Error(ValidatorMessage.Event.NotTimeEndEvent);
                }

                if (model.Objects.ToLower().Equals("tất cả"))
                {
                    if (string.IsNullOrEmpty(model.LinkCheckIn) || string.IsNullOrWhiteSpace(model.LinkCheckIn))
                        return JsonUtil.Error(ValidatorMessage.Event.LinkCheckInNotEmpty);
                    if (string.IsNullOrEmpty(model.LinkInformation) || string.IsNullOrWhiteSpace(model.LinkInformation))
                        return JsonUtil.Error(ValidatorMessage.Event.LinkInformationNotEmpty);
                }

                Event detail = new Event()
                {
                    Fee = model.Fee,
                    IsActive = true,
                    IsEnd = false,
                    LinkCheckIn = model.LinkCheckIn,
                    LinkInformation = model.LinkInformation,
                    LongDescription = model.LongDescription,
                    Name = model.Name,
                    Objects = model.Objects,
                    ShortDescription = model.ShortDescription
                };
                _unitOfWork.RepositoryCRUD<Event>().Insert(detail);
                await _unitOfWork.CommitAsync();

                var tmp = "";
                for (int i = 0; i < (6 - detail.Id.ToString().Length); i++)
                {
                    tmp += "0";
                }

                var code = "SK" + String.Format("{0:MM}", detail.CreatedWhen.GetValueOrDefault()) +
                           detail.CreatedWhen.GetValueOrDefault().Year + tmp + detail.Id;
                detail.Code = code;

                _unitOfWork.RepositoryCRUD<Event>().Update(detail);
                await _unitOfWork.CommitAsync();

                foreach (var modelTimeEvent in model.TimeEvents)
                {
                    var dateStart = DateTime.ParseExact(modelTimeEvent.DateStart + " " + modelTimeEvent.TimeStart,
                        "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    var dateEnd = DateTime.ParseExact(modelTimeEvent.DateEnd + " " + modelTimeEvent.TimeEnd,
                        "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    TimeEvent timeEvent = new TimeEvent()
                    {
                        Id = 0,
                        IsEnabled = true,
                        EventId = detail.Id,
                        DateStart = dateStart,
                        DateEnd = dateEnd
                    };

                    _unitOfWork.RepositoryCRUD<TimeEvent>().Insert(timeEvent);
                    await _unitOfWork.CommitAsync();
                }

                return JsonUtil.Success(detail.Id);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateEvent(EventViewModelUpdate model)
        {
            try
            {
                if (model.Objects.ToLower().Equals("tất cả"))
                {
                    if (string.IsNullOrEmpty(model.LinkCheckIn) || string.IsNullOrWhiteSpace(model.LinkCheckIn))
                        return JsonUtil.Error(ValidatorMessage.Event.LinkCheckInNotEmpty);
                    if (string.IsNullOrEmpty(model.LinkInformation) || string.IsNullOrWhiteSpace(model.LinkInformation))
                        return JsonUtil.Error(ValidatorMessage.Event.LinkInformationNotEmpty);
                }
                if (!model.TimeEvents.Any())
                    return JsonUtil.Error(ValidatorMessage.Event.NotDateTimeEvent);

                foreach (var modelTimeEvent in model.TimeEvents)
                {
                    if (string.IsNullOrEmpty(modelTimeEvent.DateStart) || string.IsNullOrWhiteSpace(modelTimeEvent.DateStart))
                        return JsonUtil.Error(ValidatorMessage.Event.NotDateStartEvent);
                    if (string.IsNullOrEmpty(modelTimeEvent.DateEnd) || string.IsNullOrWhiteSpace(modelTimeEvent.DateEnd))
                        return JsonUtil.Error(ValidatorMessage.Event.NotDateEndEvent);
                    if (string.IsNullOrEmpty(modelTimeEvent.TimeStart) || string.IsNullOrWhiteSpace(modelTimeEvent.TimeStart))
                        return JsonUtil.Error(ValidatorMessage.Event.NotTimeStartEvent);
                    if (string.IsNullOrEmpty(modelTimeEvent.TimeEnd) || string.IsNullOrWhiteSpace(modelTimeEvent.TimeEnd))
                        return JsonUtil.Error(ValidatorMessage.Event.NotTimeEndEvent);
                }
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == model.Id);

                detailEvent.Fee = model.Fee;
                detailEvent.LinkCheckIn = model.LinkCheckIn;
                detailEvent.LinkInformation = model.LinkInformation;
                detailEvent.LongDescription = model.LongDescription;
                detailEvent.Objects = model.Objects;
                detailEvent.ShortDescription = model.ShortDescription;
                detailEvent.Name = model.Name;
                if (model.ImagePath != null)
                {
                    detailEvent.ImagePath = string.Join(",", model.ImagePath);
                }
                else
                {
                    detailEvent.ImagePath = null;
                }

                _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                await _unitOfWork.CommitAsync();
                var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == model.Id).ToArray();
                if (model.TimeEvents.Length == timeEvent.Length)
                {
                    for (int i = 0; i < timeEvent.Length; i++)
                    {
                        var dateStart = DateTime.ParseExact(model.TimeEvents[i].DateStart + " " + model.TimeEvents[i].TimeStart,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        var dateEnd = DateTime.ParseExact(model.TimeEvents[i].DateEnd + " " + model.TimeEvents[i].TimeEnd,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                        timeEvent[i].DateStart = dateStart;
                        timeEvent[i].DateEnd = dateEnd;

                        _unitOfWork.RepositoryCRUD<TimeEvent>().Update(timeEvent[i]);
                        await _unitOfWork.CommitAsync();
                    }
                }

                if (model.TimeEvents.Length < timeEvent.Length)
                {
                    for (int i = 0; i < model.TimeEvents.Length; i++)
                    {
                        var dateStart = DateTime.ParseExact(model.TimeEvents[i].DateStart + " " + model.TimeEvents[i].TimeStart,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        var dateEnd = DateTime.ParseExact(model.TimeEvents[i].DateEnd + " " + model.TimeEvents[i].TimeEnd,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                        timeEvent[i].DateStart = dateStart;
                        timeEvent[i].DateEnd = dateEnd;

                        _unitOfWork.RepositoryCRUD<TimeEvent>().Update(timeEvent[i]);
                        await _unitOfWork.CommitAsync();
                    }

                    for (int i = model.TimeEvents.Length; i < timeEvent.Length; i++)
                    {
                        _unitOfWork.RepositoryCRUD<TimeEvent>().Delete(timeEvent[i]);
                        await _unitOfWork.CommitAsync();
                    }
                }

                if (model.TimeEvents.Length > timeEvent.Length)
                {
                    for (int i = 0; i < timeEvent.Length; i++)
                    {
                        var dateStart = DateTime.ParseExact(model.TimeEvents[i].DateStart + " " + model.TimeEvents[i].TimeStart,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        var dateEnd = DateTime.ParseExact(model.TimeEvents[i].DateEnd + " " + model.TimeEvents[i].TimeEnd,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                        timeEvent[i].DateStart = dateStart;
                        timeEvent[i].DateEnd = dateEnd;

                        _unitOfWork.RepositoryCRUD<TimeEvent>().Update(timeEvent[i]);
                        _unitOfWork.Commit();
                    }

                    for (int i = timeEvent.Length; i < model.TimeEvents.Length; i++)
                    {
                        var dateStart = DateTime.ParseExact(model.TimeEvents[i].DateStart + " " + model.TimeEvents[i].TimeStart,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        var dateEnd = DateTime.ParseExact(model.TimeEvents[i].DateEnd + " " + model.TimeEvents[i].TimeEnd,
                            "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        TimeEvent time2 = new TimeEvent()
                        {
                            Id = 0,
                            IsEnabled = true,
                            EventId = model.Id,
                            DateStart = dateStart,
                            DateEnd = dateEnd
                        };
                        _unitOfWork.RepositoryCRUD<TimeEvent>().Insert(time2);
                        _unitOfWork.Commit();
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadImageEvent(EventViewModelUploadImage model)
        {
            try
            {
                if (!model.Files.Any())
                    return JsonUtil.Error("File not selected");
               
                List<string> path = new List<string>();
                if (model.Files.Any())
                {
                    var code = _unitOfWork.RepositoryR<Event>().GetSingle(x => x.Id == model.EventId).Code;
                    
                    foreach (var file in model.Files)
                    {
                        if (file.ContentType.Split('/')[0] != "image")
                        {
                            return JsonUtil.Error("Chỉ cho phép hình ảnh, vui lòng chọn lại!");
                        }

                        if (file.Length > 10000000)
                        {
                            return JsonUtil.Error("Dung lượng tệp quá giới hạn cho phép, vui lòng chọn lại!");
                        }

                        var uploadFile =
                            await _fileService.UploadImageOptional(file, "Event",
                                code + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                        var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return uploadFile;
                        }

                        var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                        var link = (dynamic)value;
                        path.Add(link);
                    }
                }

                return JsonUtil.Success(path.ToArray());
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult UpdateImageEvent(EventViewModelUpdate model)
        {
            try
            {
                var detailEvent = _unitOfWork.RepositoryR<Event>().GetSingle(x => x.Id == model.Id);
                detailEvent.ImagePath = string.Join(",", model.ImagePath);
                detailEvent.LinkCheckInQrCodePath = model.LinkCheckInQrCodePath;
                detailEvent.LinkInformationQrCodePath = model.LinkInformationQrCodePath;
                _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                _unitOfWork.Commit();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListCustomerEvent(int eventId, string keySearch, int typeId, int status, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                bool? statusEvent = null;
                if (status == 0) statusEvent = null;
                if (status == 1) statusEvent = false;
                if (status == 2) statusEvent = true;
                var data = _unitOfWork.Repository<Proc_GetListCustomerEvent>()
                    .ExecProcedure(Proc_GetListCustomerEvent.GetEntityProc(keySearch, eventId, typeId, statusEvent, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult UpdateNoteCustomerEvent(int customerEventId, string note)
        {
            try
            {
                var customerEvent = _unitOfWork.RepositoryR<CustomerEvent>().GetSingle(x => x.Id == customerEventId);
                customerEvent.Note = note;
                _unitOfWork.RepositoryCRUD<CustomerEvent>().Update(customerEvent);
                _unitOfWork.Commit();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllStatusCustomerEvent()
        {
            try
            {
                var result = new List<EventViewModelStatus>()
                {
                    new EventViewModelStatus(){StatusId = 0, StatusName = "Tất cả"},
                    new EventViewModelStatus(){StatusId = 1, StatusName = "Đã đăng ký"},
                    new EventViewModelStatus(){StatusId = 2, StatusName = "Đã thanh toán"}
                };

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllEvent(int type)
        {
            try
            {
                List<Event> result;
                if (type == 0) //event chưa kết thúc
                {
                    result = _unitOfWork.RepositoryR<Event>().FindBy(x => x.IsActive == true && x.IsEnd == false && x.Fee == true)
                        .ToList();
                }
                else // event đã kết thúc
                {
                    result = _unitOfWork.RepositoryR<Event>().FindBy(x => x.IsEnd == true && x.Fee == true)
                        .ToList();
                }

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public Event GetEvent(int id)
        {
            return _unitOfWork.RepositoryR<Event>().GetSingle(x => x.Id == id);
        }

        public string GetTimeEvent(int id)
        {
            var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == id).ToList();
            string time = "";

            foreach (var timeEventItem in timeEvent)
            {
                var timeItem = new TimeEventMobile();

                var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                var hourStart = timeEventItem.DateStart.Hour;
                var minuteStart = timeEventItem.DateStart.Minute;

                var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                var hourEnd = timeEventItem.DateEnd.Hour;
                var minuteEnd = timeEventItem.DateEnd.Minute;
                if (dateStartEvent == dateEndEvent)
                {
                    var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                    var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);

                    timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                }
                else
                {
                    var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                    var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);

                    var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                    var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);

                    timeItem.Date = descriptionDateStart + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                    descriptionDateEnd + ", " + timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                }

                if (hourStart == hourEnd)
                {
                    if (minuteStart == minuteEnd)
                    {
                        timeItem.Time = timeEventItem.DateStart.ToString("HH:mm");
                    }
                }
                else
                {
                    timeItem.Time = timeEventItem.DateStart.ToString("HH:mm") + " - " +
                                    timeEventItem.DateEnd.ToString("HH:mm");
                }

                time = time + timeItem.Time + " | " + timeItem.Date + "\n";
            }

            return time;
        }

        public JsonResult GetListTransactionEvent(int eventId, string keySearch, DateTime fromDate, DateTime toDate, int chapterId, int statusId, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                if (eventId == 0) return JsonUtil.Success(new List<Proc_GetListTransactionEvent>(), "Success");
                var data = _unitOfWork.Repository<Proc_GetListTransactionEvent>()
                    .ExecProcedure(Proc_GetListTransactionEvent.GetEntityProc(eventId, keySearch, fromDate, toDate.AddDays(1), chapterId, statusId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> ActiveTransactionEvent(int transactionEventId, int active, string note, int customerId)
        {
            try
            {
                var transaction = await _unitOfWork.RepositoryR<TransactionEvent>()
                    .GetSingleAsync(x => x.Id == transactionEventId);

                var customerEvent = await _unitOfWork.RepositoryR<CustomerEvent>().GetSingleAsync(x =>
                    x.CustomerId == transaction.CustomerId
                    && x.EventId == transaction.EventId);

                var detailEvent = _unitOfWork.RepositoryR<Event>().GetSingle(x => x.Id == transaction.EventId);
                if (active != 0) // kích hoạt
                {
                    transaction.StatusId = (int) EnumData.TransactionStatusEnum.Accepted;
                    transaction.Note = note;
                    transaction.DateActive = DateTime.Now;

                    customerEvent.Status = true; // đã thanh toán

                    detailEvent.NumberOfAttendees += 1;


                    string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                    if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                    {
                        var notify = _notifyService.CreateNotify(transaction.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.RegisterEvent, detailEvent.Name),
                            (int)EnumData.NotifyType.Event, transaction.EventId, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }
                    else
                    {
                        var notify = _notifyService.CreateNotify(transaction.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.RegisterEventEnglish, detailEvent.Name),
                            (int)EnumData.NotifyType.Event, transaction.EventId, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }

                    


                    string formatPhoneNumber = customerEvent.PhoneNumber.Substring(0, 1);
                    if (formatPhoneNumber == "0")
                    {
                        formatPhoneNumber = customerEvent.PhoneNumber.Substring(1, customerEvent.PhoneNumber.Length - 1);
                    }
                    else
                    {
                        formatPhoneNumber = customerEvent.PhoneNumber;
                    }

                    var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                    string content = $"Chuc mung Anh/Chi da dang ky thanh cong event cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline}";
                    

                    _accountService.SendMailCustomer(customerId, "Đăng ký event thành công", content);
                    var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);

                    _unitOfWork.RepositoryCRUD<TransactionEvent>().Update(transaction);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<CustomerEvent>().Update(customerEvent);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                    await _unitOfWork.CommitAsync();
                    if (sendSms == false)
                    {
                        return JsonUtil.Success(true, "Đã có lỗi xảy ra khi gửi sms, vui lòng kiểm tra lại!");
                    }
                    else
                    {
                        return JsonUtil.Success(true);
                    }
                }
                else // Từ chối
                {
                    transaction.StatusId = (int) EnumData.TransactionStatusEnum.Cancel;
                    transaction.Note = note;

                    customerEvent.IsEnabled = false;

                    _unitOfWork.RepositoryCRUD<TransactionEvent>().Update(transaction);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<CustomerEvent>().Update(customerEvent);
                    await _unitOfWork.CommitAsync();

                    string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                    if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                    {
                        var notify = _notifyService.CreateNotify(transaction.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.CancelRegisterEvent, detailEvent.Name),
                            (int)EnumData.NotifyType.Event, transaction.EventId, customerId, note);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }
                    else
                    {
                        var notify = _notifyService.CreateNotify(transaction.CustomerId,
                            string.Format(ValidatorMessage.ContentNotify.CancelRegisterEventEnglish, detailEvent.Name),
                            (int)EnumData.NotifyType.Event, transaction.EventId, customerId, note);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }

                    
                    return JsonUtil.Success(true);
                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GenerateQrCode(FileStreamResult streamResult, string fileName, string folderName)
        {
            try
            {
                string path = $@"{ApplicationEnvironment.ApplicationBasePath}{folderName}";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fullPath = Path.Combine(path, fileName + ".png");

                // string[] listFile = Directory.GetFiles(path);

                // foreach (var item in listFile)
                // {
                //     if (item == fullPath)
                //     {
                //         File.Delete(item);
                //     }
                // }

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    streamResult.FileStream.CopyTo(fileStream);
                }

                string result = folderName + '/' + fileName + ".png";

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GenerateLinkInformationQrCodePath(FileStreamResult streamResult, int eventId)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);
                return await GenerateQrCode(streamResult, "LinkInformationQrCode_" + detailEvent.Code,
                    "LinkInformationQrCode");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GenerateLinkCheckInQrCodePath(FileStreamResult streamResult, int eventId)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);
                return await GenerateQrCode(streamResult, "LinkCheckInQrCode_" + detailEvent.Code,
                    "LinkCheckInQrCode");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListEventMobile(string keySearch, int customerId, int eventType, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListEventMobile>()
                    .ExecProcedure(Proc_GetListEventMobile.GetEntityProc(keySearch ,customerId, eventType, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<EventViewModelMobile> result = new List<EventViewModelMobile>();

                foreach (var item in data)
                {
                    List<TimeEventMobile> time = new List<TimeEventMobile>();
                    var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == item.EventId).ToArray();
                    if (eventType == 0)
                    {
                        for (int i = 0; i < timeEvent.Length; i++)
                        {
                            if (timeEvent[i].DateEnd > DateTime.Now)
                            {
                                foreach (var timeEventItem in timeEvent)
                                {
                                    var timeItem = new TimeEventMobile();

                                    var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                                    var hourStart = timeEventItem.DateStart.Hour;
                                    var minuteStart = timeEventItem.DateStart.Minute;

                                    var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                                    var hourEnd = timeEventItem.DateEnd.Hour;
                                    var minuteEnd = timeEventItem.DateEnd.Minute;
                                    if (dateStartEvent == dateEndEvent)
                                    {
                                        var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                        string descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                                        string descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                                        if (language.Equals("en"))
                                        {
                                            timeItem.Date = descriptionDateEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy");
                                        }
                                        else
                                        {
                                            timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                                        }
                                    }
                                    else
                                    {
                                        var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                        var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                                        var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                                        var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                                        var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                                        var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);

                                        if (language.Equals("en"))
                                        {
                                            timeItem.Date = descriptionDateStartEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                                            descriptionDateEndEnglish + ", " + timeEventItem.DateEnd.ToString("MM/dd/yyyy");
                                        }
                                        else
                                        {
                                            timeItem.Date = descriptionDateStart + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                                            descriptionDateEnd + ", " + timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                                        }
                                    }

                                    if (hourStart == hourEnd)
                                    {
                                        if (minuteStart == minuteEnd)
                                        {
                                            timeItem.Time = timeEventItem.DateStart.ToString("HH:mm");
                                        }
                                    }
                                    else
                                    {
                                        timeItem.Time = timeEventItem.DateStart.ToString("HH:mm") + " - " +
                                                        timeEventItem.DateEnd.ToString("HH:mm");
                                    }
                                    time.Add(timeItem);
                                }

                                EventViewModelMobile eventViewModelMobile = new EventViewModelMobile()
                                {
                                    TimeEvents = time.ToArray(),
                                    EventId = item.EventId,
                                    EventType = item.EventType,
                                    EventName = item.EventName,
                                    EventCode = item.EventCode,
                                    Liked = item.Liked,
                                    LongDescription = item.LongDescription,
                                    QrInformation = item.LinkInformationQrCodePath,
                                    LinkCheckInQrCodePath = item.LinkCheckInQrCodePath,
                                    Objects = item.Objects,
                                    IsFee = item.IsFee,
                                    RowNum = item.RowNum,
                                    Shared = item.Shared,
                                    ShortDescription = item.ShortDescription,
                                    SumLike = item.SumLike,
                                    SumShare = item.SumShare,
                                    Total = item.Total
                                };
                                if (!string.IsNullOrEmpty(item.ImagePath))
                                {
                                    eventViewModelMobile.ImagePath = item.ImagePath.Split(",");
                                }
                                else
                                {
                                    eventViewModelMobile.ImagePath = new string[] { };
                                }
                                result.Add(eventViewModelMobile);
                                i = timeEvent.Length;
                            }

                        }
                    }
                    else
                    {
                        foreach (var timeEventItem in timeEvent)
                        {
                            var timeItem = new TimeEventMobile();

                            var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                            var hourStart = timeEventItem.DateStart.Hour;
                            var minuteStart = timeEventItem.DateStart.Minute;

                            var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                            var hourEnd = timeEventItem.DateEnd.Hour;
                            var minuteEnd = timeEventItem.DateEnd.Minute;
                            if (dateStartEvent == dateEndEvent)
                            {
                                var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                                var descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                                }
                            }
                            else
                            {
                                var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                                var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                                var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                                var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                                var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);
                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateStartEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                                    descriptionDateEndEnglish + ", " + timeEventItem.DateEnd.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDateStart + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                                    descriptionDateEnd + ", " + timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                                }
                            }

                            if (hourStart == hourEnd)
                            {
                                if (minuteStart == minuteEnd)
                                {
                                    timeItem.Time = timeEventItem.DateStart.ToString("HH:mm");
                                }
                            }
                            else
                            {
                                timeItem.Time = timeEventItem.DateStart.ToString("HH:mm") + " - " +
                                                timeEventItem.DateEnd.ToString("HH:mm");
                            }
                            time.Add(timeItem);
                        }

                        EventViewModelMobile eventViewModelMobile = new EventViewModelMobile()
                        {
                            TimeEvents = time.ToArray(),
                            EventId = item.EventId,
                            EventType = item.EventType,
                            EventName = item.EventName,
                            EventCode = item.EventCode,
                            Liked = item.Liked,
                            LongDescription = item.LongDescription,
                            QrInformation = item.LinkInformationQrCodePath,
                            LinkCheckInQrCodePath = item.LinkCheckInQrCodePath,
                            Objects = item.Objects,
                            IsFee = item.IsFee,
                            RowNum = item.RowNum,
                            Shared = item.Shared,
                            ShortDescription = item.ShortDescription,
                            SumLike = item.SumLike,
                            SumShare = item.SumShare,
                            Total = item.Total
                        };
                        if (!string.IsNullOrEmpty(item.ImagePath))
                        {
                            eventViewModelMobile.ImagePath = item.ImagePath.Split(",");
                        }
                        else
                        {
                            eventViewModelMobile.ImagePath = new string[] { };
                        }
                        result.Add(eventViewModelMobile);
                    }
                    

                }
                return JsonUtil.Success(result, "Success", result.Count);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult LikedEvent(int customerId, int eventId)
        {
            try
            {
                var customerLikeEvent = _unitOfWork.RepositoryR<CustomerLikeEvent>()
                    .GetSingle(x => x.CustomerId == customerId && x.EventId == eventId);
                if (customerLikeEvent == null)
                {
                    CustomerLikeEvent likedEvent = new CustomerLikeEvent()
                    {
                        Id = 0,
                        CustomerId = customerId,
                        IsEnabled = true,
                        EventId = eventId,
                        IsLiked = true
                    };

                    _unitOfWork.RepositoryCRUD<CustomerLikeEvent>().Insert(likedEvent);
                    _unitOfWork.Commit();

                    var sumLike = _unitOfWork.RepositoryR<CustomerLikeEvent>().FindBy(x => x.EventId == eventId && x.IsLiked == true).ToList().Count;

                    return JsonUtil.Success(new
                    {
                        Liked = true,
                        SumLike = sumLike
                    });
                }
                else
                {
                    customerLikeEvent.IsLiked = !customerLikeEvent.IsLiked;
                    _unitOfWork.RepositoryCRUD<CustomerLikeEvent>().Update(customerLikeEvent);
                    _unitOfWork.Commit();

                    var sumLike = _unitOfWork.RepositoryR<CustomerLikeEvent>().FindBy(x => x.EventId == eventId && x.IsLiked == true).ToList().Count;

                    return JsonUtil.Success(new
                    {
                        Liked = customerLikeEvent.IsLiked,
                        SumLike = sumLike
                    });
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SharedEvent(int customerId, int eventId)
        {
            try
            {
                CustomerShareEvent shareEvent = new CustomerShareEvent()
                {
                    Id = 0,
                    CustomerId = customerId,
                    IsEnabled = true,
                    EventId = eventId,
                    IsShared = true
                };

                _unitOfWork.RepositoryCRUD<CustomerShareEvent>().Update(shareEvent);
                _unitOfWork.Commit();


                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailEventMobile(int customerId, string eventCode)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Code.ToLower().Equals(eventCode.ToLower()));
                if (detailEvent == null) return JsonUtil.Error(ValidatorMessage.Event.NotExist);
                if (detailEvent.IsEnd == true) return JsonUtil.Error(ValidatorMessage.Event.End);
                if (detailEvent.IsActive == false) return JsonUtil.Error(ValidatorMessage.Event.IsActive);
                var customerLikeEvent = await _unitOfWork.RepositoryR<CustomerLikeEvent>()
                    .AnyAsync(x => x.CustomerId == customerId && x.EventId == detailEvent.Id && x.IsLiked == true);
                var customerShareEvent = await _unitOfWork.RepositoryR<CustomerShareEvent>()
                    .AnyAsync(x => x.CustomerId == customerId && x.EventId == detailEvent.Id && x.IsShared == true);
                var sumLike = await _unitOfWork.RepositoryR<CustomerLikeEvent>().CountAsync(x => x.EventId == detailEvent.Id && x.IsLiked == true);
                var sumShare = await _unitOfWork.RepositoryR<CustomerShareEvent>().CountAsync(x => x.EventId == detailEvent.Id && x.IsShared == true);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<TimeEventMobile> time = new List<TimeEventMobile>();
                var timeEvent = await _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == detailEvent.Id).ToArrayAsync();
                foreach (var timeEventItem in timeEvent)
                {
                    var timeItem = new TimeEventMobile();

                    var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                    var hourStart = timeEventItem.DateStart.Hour;
                    var minuteStart = timeEventItem.DateStart.Minute;

                    var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                    var hourEnd = timeEventItem.DateEnd.Hour;
                    var minuteEnd = timeEventItem.DateEnd.Minute;
                    if (dateStartEvent == dateEndEvent)
                    {
                        var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                        var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                        var descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                        if (language.Equals("en"))
                        {
                            timeItem.Date = descriptionDateEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                        }
                    }
                    else
                    {
                        var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                        var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                        var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                        var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                        var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                        var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);
                        if (language.Equals("en"))
                        {
                            timeItem.Date = descriptionDateStartEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                            descriptionDateEndEnglish + ", " + timeEventItem.DateEnd.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            timeItem.Date = descriptionDateStart + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                            descriptionDateEnd + ", " + timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                        }
                        
                    }

                    if (hourStart == hourEnd)
                    {
                        if (minuteStart == minuteEnd)
                        {
                            timeItem.Time = timeEventItem.DateStart.ToString("hh:mm");
                        }
                    }
                    else
                    {
                        timeItem.Time = timeEventItem.DateStart.ToString("hh:mm") + " - " +
                                        timeEventItem.DateEnd.ToString("hh:mm");
                    }
                    time.Add(timeItem);
                }
                string[] imagePath = new string[] { };
                if (!string.IsNullOrEmpty(detailEvent.ImagePath))
                {
                    imagePath = detailEvent.ImagePath.Split(",");
                }

                int eventType = 0;
                if (detailEvent.Fee == false)
                {
                    if (_unitOfWork.RepositoryR<CustomerEvent>().Any(x =>
                        x.EventId == detailEvent.Id && x.CustomerId == customerId && x.Status == false))
                    {
                        eventType = 1;
                    }
                }
                else
                {
                    if (_unitOfWork.RepositoryR<CustomerEvent>().Any(x =>
                        x.EventId == detailEvent.Id && x.CustomerId == customerId && x.Status == true))
                    {
                        eventType = 1;
                    }
                }

                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    return JsonUtil.Success(new
                    {
                        EventId = detailEvent.Id,
                        EventName = detailEvent.Name,
                        EventCode = detailEvent.Code,
                        EventType = eventType,
                        TimeEvents = time.ToArray(),
                        ShortDescription = detailEvent.ShortDescription,
                        LongDescription = detailEvent.LongDescription,
                        QrInformation = detailEvent.LinkInformationQrCodePath,
                        LinkCheckInQrCodePath = detailEvent.LinkCheckInQrCodePath,
                        IsFee = detailEvent.Fee,
                        Objects = detailEvent.Objects,
                        ImagePath = imagePath,
                        Liked = customerLikeEvent,
                        SumLike = sumLike,
                        Shared = customerShareEvent,
                        sumShare = sumShare
                    });
                }
                else
                {
                    string objectEvent = "";
                    if (detailEvent.Objects.Equals("Tất cả"))
                    {
                        objectEvent = "All User";
                    }
                    else
                    {
                        objectEvent = "Member OBC";
                    }
                    return JsonUtil.Success(new
                    {
                        EventId = detailEvent.Id,
                        EventName = detailEvent.Name,
                        EventCode = detailEvent.Code,
                        EventType = eventType,
                        TimeEvents = time.ToArray(),
                        ShortDescription = detailEvent.ShortDescription,
                        LongDescription = detailEvent.LongDescription,
                        QrInformation = detailEvent.LinkInformationQrCodePath,
                        LinkCheckInQrCodePath = detailEvent.LinkCheckInQrCodePath,
                        IsFee = detailEvent.Fee,
                        Objects = objectEvent,
                        ImagePath = imagePath,
                        Liked = customerLikeEvent,
                        SumLike = sumLike,
                        Shared = customerShareEvent,
                        sumShare = sumShare
                    });
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckCustomerRegisterEvent(int customerId, int eventId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var detailEvent = _unitOfWork.RepositoryR<Event>().GetSingle(x => x.Id == eventId);
                var transactionEvent = _unitOfWork.RepositoryR<TransactionEvent>()
                    .Any(x => x.CustomerId == customerId && x.EventId == eventId &&
                              x.StatusId == (int) EnumData.TransactionStatusEnum.PendingActive);

                if (detailEvent.Objects.ToLower().Equals("thành viên obc"))
                {
                    if (customer.CustomerRoleId == (int) EnumData.CustomerRoleEnum.PremiumMember && customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                    {
                        if (transactionEvent)
                        {
                            if (customer.Language.Equals("en"))
                            {
                                return JsonUtil.Error(ValidatorMessage.Event.NotRegisterEnglish);
                            }
                            else
                            {
                                return JsonUtil.Error(ValidatorMessage.Event.NotRegister);
                            }
                        }
                        else
                        {
                            return JsonUtil.Success(true);
                        }
                    }
                    else
                    {
                        if (customer.Language.Equals("en"))
                        {
                            return JsonUtil.Error(ValidatorMessage.Event.FreeMemberNotRegisterEnglish);
                        }
                        else
                        {
                            return JsonUtil.Error(ValidatorMessage.Event.FreeMemberNotRegister);
                        }
                       
                    }
                }
                else
                {
                    if (transactionEvent)
                    {
                        if (customer.Language.Equals("en"))
                        {
                            return JsonUtil.Error(ValidatorMessage.Event.NotRegisterEnglish);
                        }
                        else
                        {
                            return JsonUtil.Error(ValidatorMessage.Event.NotRegister);
                        }
                    }
                    else
                    {
                        return JsonUtil.Success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> RegisterEvent(int customerId, int eventId, string phoneNumber, string email)
        {
            try
            {
                var detailEvent = await _unitOfWork.RepositoryR<Event>().GetSingleAsync(x => x.Id == eventId);
                CustomerEvent customerEvent = new CustomerEvent()
                {
                    Id = 0,
                    EventId = eventId,
                    CheckIn = false,
                    CustomerId = customerId,
                    Status = false, // Đã đăng ký
                    PhoneNumber = phoneNumber,
                    Email = email
                };

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    var notify = _notifyService.CreateNotify(customerId,
                        string.Format(ValidatorMessage.ContentNotify.RegisterEvent, detailEvent.Name),
                        (int)EnumData.NotifyType.Event, eventId, null, null);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }
                else
                {
                    var notify = _notifyService.CreateNotify(customerId,
                        string.Format(ValidatorMessage.ContentNotify.RegisterEventEnglish, detailEvent.Name),
                        (int)EnumData.NotifyType.Event, eventId, null, null);
                    var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                    var isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return notify;
                    }
                }

                

                string formatPhoneNumber = customerEvent.PhoneNumber.Substring(0, 1);
                if (formatPhoneNumber == "0")
                {
                    formatPhoneNumber = customerEvent.PhoneNumber.Substring(1, customerEvent.PhoneNumber.Length - 1);
                }
                else
                {
                    formatPhoneNumber = customerEvent.PhoneNumber;
                }

                var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                string content = $"Chuc mung Anh/Chi da dang ky thanh cong event cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline}";
                string contentEmail = $"<p style=\"color: black\">Chuc mung Anh/Chi da dang ky thanh cong event cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline} </p>";
                

                _accountService.SendMailCustomer(customerId, "Đăng ký event thành công", contentEmail);
                var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);

                _unitOfWork.RepositoryCRUD<CustomerEvent>().Insert(customerEvent);
                await _unitOfWork.CommitAsync();

                detailEvent.NumberOfAttendees = _unitOfWork.RepositoryR<CustomerEvent>()
                    .Count(x => x.EventId == eventId && x.Status == false);

                _unitOfWork.RepositoryCRUD<Event>().Update(detailEvent);
                await _unitOfWork.CommitAsync();

                if (sendSms == false)
                {
                    return JsonUtil.Success(new
                    {
                        CustomerEventId = customerEvent.Id,
                        TransactionEventId = 0
                    }, "Đã có lỗi xảy ra khi gửi sms, vui lòng kiểm tra lại!");
                }
                else
                {
                    return JsonUtil.Success(new
                    {
                        CustomerEventId = customerEvent.Id,
                        TransactionEventId = 0
                    });
                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadImageTransactionEvent(EventViewModelUploadImageTransaction model, int customerId)
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

                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId;
                CustomerEvent customerEvent = new CustomerEvent()
                {
                    Id = 0,
                    EventId = model.EventId,
                    CheckIn = false,
                    CustomerId = customerId,
                    Status = false, // Đã dăng ký
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email
                };
                _unitOfWork.RepositoryCRUD<CustomerEvent>().Insert(customerEvent);
                await _unitOfWork.CommitAsync();

                TransactionEvent transactionEvent = new TransactionEvent()
                {
                    Id = 0,
                    ChapterId = chapterId,
                    CustomerId = customerId,
                    EventId = model.EventId,
                    StatusId = (int)EnumData.TransactionStatusEnum.PendingActive
                };
                _unitOfWork.RepositoryCRUD<TransactionEvent>().Insert(transactionEvent);
                await _unitOfWork.CommitAsync();

                var tmp = "";
                for (int i = 0; i < (6 - transactionEvent.Id.ToString().Length); i++)
                {
                    tmp += "0";
                }

                var code = "SK" + String.Format("{0:MM}", transactionEvent.CreatedWhen.GetValueOrDefault()) +
                           transactionEvent.CreatedWhen.GetValueOrDefault().Year + tmp + transactionEvent.Id;
                

                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "TransactionEvent",
                        code + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;

                transactionEvent.Code = code;
                transactionEvent.ImagePath = link;

                _unitOfWork.RepositoryCRUD<TransactionEvent>().Update(transactionEvent);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListLikedEventAndCourse(string keySearch, int customerId, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListLikedEventAndCourse>()
                    .ExecProcedure(Proc_GetListLikedEventAndCourse.GetEntityProc(keySearch ,customerId, pageNum, pageSize)).ToList();

                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);

                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<EventAndCourseViewModelLiked> result = new List<EventAndCourseViewModelLiked>();

                foreach (var dataItem in data)
                {
                    if (dataItem.FormType.ToLower().Equals("event"))
                    {
                        List<TimeEventMobile> time = new List<TimeEventMobile>();
                        var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == dataItem.EventId).ToArray();
                        foreach (var timeEventItem in timeEvent)
                        {
                            var timeItem = new TimeEventMobile();

                            var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                            var hourStart = timeEventItem.DateStart.Hour;
                            var minuteStart = timeEventItem.DateStart.Minute;

                            var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                            var hourEnd = timeEventItem.DateEnd.Hour;
                            var minuteEnd = timeEventItem.DateEnd.Minute;
                            if (dateStartEvent == dateEndEvent)
                            {
                                var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                                string descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                                }
                            }
                            else
                            {
                                var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                                var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                                var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                                var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                                var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);

                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateStartEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                                    descriptionDateEndEnglish + ", " + timeEventItem.DateEnd.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDateStart + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                                    descriptionDateEnd + ", " + timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                                }
                            }

                            if (hourStart == hourEnd)
                            {
                                if (minuteStart == minuteEnd)
                                {
                                    timeItem.Time = timeEventItem.DateStart.ToString("HH:mm");
                                }
                            }
                            else
                            {
                                timeItem.Time = timeEventItem.DateStart.ToString("HH:mm") + " - " +
                                                timeEventItem.DateEnd.ToString("HH:mm");
                            }
                            time.Add(timeItem);
                        }

                        var resultItem = new EventAndCourseViewModelLiked()
                        {
                            RowNum = dataItem.RowNum,
                            FormType = dataItem.FormType,
                            EventId = dataItem.EventId,
                            EventCode = dataItem.EventCode,
                            EventName = dataItem.EventName,
                            EventType = dataItem.EventType,
                            ShortDescription = dataItem.ShortDescription,
                            CourseId = dataItem.CourseId,
                            CourseCode = dataItem.CourseCode,
                            CourseName = dataItem.CourseName,
                            CourseType = dataItem.CourseType,
                            Assess = dataItem.Assess,
                            Assessed = dataItem.Assessed,
                            VideoId = dataItem.VideoId,
                            VideoCode = dataItem.VideoCode,
                            VideoName = dataItem.VideoName,
                            VideoType = dataItem.VideoType,
                            ImagePath = dataItem.ImagePath,
                            Scores = dataItem.Scores,
                            DateLiked = dataItem.DateLiked,
                            Total = dataItem.Total
                        };
                        resultItem.TimeEvents = time.ToArray();

                        result.Add(resultItem);
                    }

                    if (dataItem.FormType.ToLower().Equals("course"))
                    {
                        List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                        var timeCourse = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == dataItem.CourseId).ToArray();
                        foreach (var timeCourseItem in timeCourse)
                        {
                            var timeItem = new TimeCourseMobile();

                            var dateStartEvent = new DateTime(timeCourseItem.DateStart.Year, timeCourseItem.DateStart.Month, timeCourseItem.DateStart.Day);
                            var hourStart = timeCourseItem.DateStart.Hour;
                            var minuteStart = timeCourseItem.DateStart.Minute;

                            var dateEndEvent = new DateTime(timeCourseItem.DateEnd.Year, timeCourseItem.DateEnd.Month, timeCourseItem.DateEnd.Day);
                            var hourEnd = timeCourseItem.DateEnd.Hour;
                            var minuteEnd = timeCourseItem.DateEnd.Minute;
                            if (dateStartEvent == dateEndEvent)
                            {
                                var date = timeCourseItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                                string descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateEnglish + ", " + timeCourseItem.DateStart.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDate + ", " + timeCourseItem.DateStart.ToString("dd/MM/yyyy");
                                }
                            }
                            else
                            {
                                var dateStart = timeCourseItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                                var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                                var dateEnd = timeCourseItem.DateEnd.DayOfWeek.GetHashCode();
                                var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                                var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);

                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateStartEnglish + ", " + timeCourseItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                                    descriptionDateEndEnglish + ", " + timeCourseItem.DateEnd.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDateStart + ", " + timeCourseItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                                    descriptionDateEnd + ", " + timeCourseItem.DateEnd.ToString("dd/MM/yyyy");
                                }
                            }

                            if (hourStart == hourEnd)
                            {
                                if (minuteStart == minuteEnd)
                                {
                                    timeItem.Time = timeCourseItem.DateStart.ToString("HH:mm");
                                }
                            }
                            else
                            {
                                timeItem.Time = timeCourseItem.DateStart.ToString("HH:mm") + " - " +
                                                timeCourseItem.DateEnd.ToString("HH:mm");
                            }
                            time.Add(timeItem);
                        }

                        var resultItem = new EventAndCourseViewModelLiked()
                        {
                            RowNum = dataItem.RowNum,
                            FormType = dataItem.FormType,
                            EventId = dataItem.EventId,
                            EventCode = dataItem.EventCode,
                            EventName = dataItem.EventName,
                            EventType = dataItem.EventType,
                            ShortDescription = dataItem.ShortDescription,
                            CourseId = dataItem.CourseId,
                            CourseCode = dataItem.CourseCode,
                            CourseName = dataItem.CourseName,
                            CourseType = dataItem.CourseType,
                            Assess = dataItem.Assess,
                            Assessed = dataItem.Assessed,
                            VideoId = dataItem.VideoId,
                            VideoCode = dataItem.VideoCode,
                            VideoName = dataItem.VideoName,
                            VideoType = dataItem.VideoType,
                            ImagePath = dataItem.ImagePath,
                            Scores = dataItem.Scores,
                            DateLiked = dataItem.DateLiked,
                            Total = dataItem.Total
                        };
                        resultItem.TimeCourses = time.ToArray();

                        result.Add(resultItem);
                    }

                    if (dataItem.FormType.ToLower().Equals("video"))
                    {
                        List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                        var timeVideo = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == dataItem.VideoId).ToArray();
                        foreach (var timeVideoItem in timeVideo)
                        {
                            var timeItem = new TimeCourseMobile();

                            var dateStartEvent = new DateTime(timeVideoItem.DateStart.Year, timeVideoItem.DateStart.Month, timeVideoItem.DateStart.Day);
                            var hourStart = timeVideoItem.DateStart.Hour;
                            var minuteStart = timeVideoItem.DateStart.Minute;

                            var dateEndEvent = new DateTime(timeVideoItem.DateEnd.Year, timeVideoItem.DateEnd.Month, timeVideoItem.DateEnd.Day);
                            var hourEnd = timeVideoItem.DateEnd.Hour;
                            var minuteEnd = timeVideoItem.DateEnd.Minute;
                            if (dateStartEvent == dateEndEvent)
                            {
                                var date = timeVideoItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                                string descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateEnglish + ", " + timeVideoItem.DateStart.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDate + ", " + timeVideoItem.DateStart.ToString("dd/MM/yyyy");
                                }
                            }
                            else
                            {
                                var dateStart = timeVideoItem.DateStart.DayOfWeek.GetHashCode();
                                var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                                var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                                var dateEnd = timeVideoItem.DateEnd.DayOfWeek.GetHashCode();
                                var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                                var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);

                                if (language.Equals("en"))
                                {
                                    timeItem.Date = descriptionDateStartEnglish + ", " + timeVideoItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                                    descriptionDateEndEnglish + ", " + timeVideoItem.DateEnd.ToString("MM/dd/yyyy");
                                }
                                else
                                {
                                    timeItem.Date = descriptionDateStart + ", " + timeVideoItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                                    descriptionDateEnd + ", " + timeVideoItem.DateEnd.ToString("dd/MM/yyyy");
                                }
                            }

                            if (hourStart == hourEnd)
                            {
                                if (minuteStart == minuteEnd)
                                {
                                    timeItem.Time = timeVideoItem.DateStart.ToString("HH:mm");
                                }
                            }
                            else
                            {
                                timeItem.Time = timeVideoItem.DateStart.ToString("HH:mm") + " - " +
                                                timeVideoItem.DateEnd.ToString("HH:mm");
                            }
                            time.Add(timeItem);
                        }

                        var resultItem = new EventAndCourseViewModelLiked()
                        {
                            RowNum = dataItem.RowNum,
                            FormType = dataItem.FormType,
                            EventId = dataItem.EventId,
                            EventCode = dataItem.EventCode,
                            EventName = dataItem.EventName,
                            EventType = dataItem.EventType,
                            ShortDescription = dataItem.ShortDescription,
                            CourseId = dataItem.CourseId,
                            CourseCode = dataItem.CourseCode,
                            CourseName = dataItem.CourseName,
                            CourseType = dataItem.CourseType,
                            Assess = dataItem.Assess,
                            Assessed = dataItem.Assessed,
                            VideoId = dataItem.VideoId,
                            VideoCode = dataItem.VideoCode,
                            VideoName = dataItem.VideoName,
                            VideoType = dataItem.VideoType,
                            ImagePath = dataItem.ImagePath,
                            Scores = dataItem.Scores,
                            DateLiked = dataItem.DateLiked,
                            Total = dataItem.Total
                        };
                        resultItem.TimeVideos = time.ToArray();

                        result.Add(resultItem);
                    }
                }
                return JsonUtil.Success(result, "Success", result.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CustomerCheckInEvent(string eventCode, int customerId)
        {
            try
            {
                var detailEvent = _unitOfWork.RepositoryR<Event>().GetSingle(x => x.Code.Equals(eventCode));
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                var customerEvent = _unitOfWork.RepositoryR<CustomerEvent>()
                    .GetSingle(x => x.EventId == detailEvent.Id && x.CustomerId == customerId);
                if (customerEvent == null)
                {
                    if (language.Equals("en"))
                    {
                        return JsonUtil.Error("You are not on the participating list");
                    }
                    else
                    {
                        return JsonUtil.Error("Bạn không có trong danh sách tham gia");
                    }
                }
                customerEvent.CheckIn = true;

                _unitOfWork.RepositoryCRUD<CustomerEvent>().Update(customerEvent);
                _unitOfWork.Commit();

                if (language.Equals("en"))
                {
                    return JsonUtil.Success("You have successfully checked in");
                }
                else
                {
                    return JsonUtil.Success("Bạn đã check in thành công");
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListRecentEvent(int customerId)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListRecentEvent>()
                    .ExecProcedure(Proc_GetListRecentEvent.GetEntityProc(customerId)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);

                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<EventViewModelRecent> result = new List<EventViewModelRecent>();

                foreach (var item in data)
                {
                    List<TimeEventMobile> time = new List<TimeEventMobile>();
                    var timeEvent = _unitOfWork.RepositoryR<TimeEvent>().FindBy(x => x.EventId == item.EventId).ToArray();
                    for (int i = 0; i < timeEvent.Length; i++)
                    {
                        if (timeEvent[i].DateEnd > DateTime.Now)
                        {
                            foreach (var timeEventItem in timeEvent)
                            {
                                var timeItem = new TimeEventMobile();

                                var dateStartEvent = new DateTime(timeEventItem.DateStart.Year, timeEventItem.DateStart.Month, timeEventItem.DateStart.Day);
                                var hourStart = timeEventItem.DateStart.Hour;
                                var minuteStart = timeEventItem.DateStart.Minute;

                                var dateEndEvent = new DateTime(timeEventItem.DateEnd.Year, timeEventItem.DateEnd.Month, timeEventItem.DateEnd.Day);
                                var hourEnd = timeEventItem.DateEnd.Hour;
                                var minuteEnd = timeEventItem.DateEnd.Minute;
                                if (dateStartEvent == dateEndEvent)
                                {
                                    var date = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                    var descriptionDate = Extensions.GetDescription((EnumData.DayOfWeek)date);
                                    string descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
                                    if (language.Equals("en"))
                                    {
                                        timeItem.Date = descriptionDateEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy");
                                    }
                                    else
                                    {
                                        timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                                    }

                                    timeItem.Date = descriptionDate + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    var dateStart = timeEventItem.DateStart.DayOfWeek.GetHashCode();
                                    var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);
                                    var descriptionDateStartEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateStart);

                                    var dateEnd = timeEventItem.DateEnd.DayOfWeek.GetHashCode();
                                    var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);
                                    var descriptionDateEndEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)dateEnd);

                                    if (language.Equals("en"))
                                    {
                                        timeItem.Date = descriptionDateStartEnglish + ", " + timeEventItem.DateStart.ToString("MM/dd/yyyy") + " - " +
                                                        descriptionDateEndEnglish + ", " + timeEventItem.DateEnd.ToString("MM/dd/yyyy");
                                    }
                                    else
                                    {
                                        timeItem.Date = descriptionDateStart + ", " + timeEventItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                                        descriptionDateEnd + ", " + timeEventItem.DateEnd.ToString("dd/MM/yyyy");
                                    }
                                }

                                if (hourStart == hourEnd)
                                {
                                    if (minuteStart == minuteEnd)
                                    {
                                        timeItem.Time = timeEventItem.DateStart.ToString("HH:mm");
                                    }
                                }
                                else
                                {
                                    timeItem.Time = timeEventItem.DateStart.ToString("HH:mm") + " - " +
                                                    timeEventItem.DateEnd.ToString("HH:mm");
                                }
                                time.Add(timeItem);
                            }

                            EventViewModelRecent eventViewModelMobile = new EventViewModelRecent()
                            {
                                TimeEvents = time.ToArray(),
                                EventId = item.EventId,
                                EventName = item.EventName,
                                EventCode = item.EventCode,
                                LongDescription = item.LongDescription,
                                ShortDescription = item.ShortDescription,
                                ImagePath = item.ImagePath
                            };
                            result.Add(eventViewModelMobile);
                            i = timeEvent.Length;
                        }
                    }
                }
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }


        public byte[] GenerateByteArray(string url)
        {
            var image = GenerateImage(url);
            return ImageToByte(image);
        }

        public static Bitmap GenerateImage(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            string path = $@"{ApplicationEnvironment.ApplicationBasePath}{"Logo"}";
            var fullPath = Path.Combine(path, "logoOBC.jpg");
            Bitmap qrCodeImage = qrCode.GetGraphic(15, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(fullPath),35);
            return qrCodeImage;
        }

        private static byte[] ImageToByte(Image img)
        {
            var stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
