using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Course;
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

namespace Core.Business.Services
{
    public class CourseService : BaseService, ICourseService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IFileService _fileService;
        public INotifyService _notifyService;
        public IAccountService _accountService;
        public CourseService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
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

        public async Task<JsonResult> GetListCourse(string keySearch, string fromDateStart, string toDateStart, string fromDateEnd, string toDateEnd,
            string objects, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch))
                    keySearch = keySearch.Trim();
                var listCourseId = new List<int>();
                var listCourse = new List<Course>();
                if (string.IsNullOrEmpty(fromDateStart) && string.IsNullOrEmpty(toDateStart) &&
                    string.IsNullOrEmpty(fromDateEnd) && string.IsNullOrEmpty(toDateEnd))
                {
                    listCourse = await _unitOfWork.RepositoryR<Course>().FindBy(x =>
                                                        (keySearch == null ||
                                                            (x.Code.ToLower().Contains(keySearch.ToLower()) ||
                                                             x.Name.ToLower().Contains(keySearch.ToLower()))) &&
                                                        (objects == null || x.Objects.ToLower().Equals(objects.ToLower()))).ToListAsync();
                } 
                else 
                {
                    if (!string.IsNullOrEmpty(fromDateStart) && !string.IsNullOrEmpty(toDateStart))
                    {
                        listCourseId = _unitOfWork.RepositoryR<TimeCourse>()
                                .FindBy(y => (y.DateStart.Date >= DateTime.Parse(fromDateStart).Date && y.DateStart.Date <= DateTime.Parse(toDateStart).Date))
                                .Select(x => x.CourseId).ToList();
                    }
                    if (!string.IsNullOrEmpty(fromDateEnd) && !string.IsNullOrEmpty(toDateEnd))
                    {
                        listCourseId = _unitOfWork.RepositoryR<TimeCourse>()
                                   .FindBy(y => (y.DateEnd.Date >= DateTime.Parse(fromDateEnd).Date && y.DateEnd.Date <= DateTime.Parse(toDateEnd).Date))
                                   .Select(x => x.CourseId).ToList();
                    }
                    if (!string.IsNullOrEmpty(fromDateStart) && !string.IsNullOrEmpty(toDateStart) &&
                        !string.IsNullOrEmpty(fromDateEnd) && !string.IsNullOrEmpty(toDateEnd))
                    {
                        listCourseId = _unitOfWork.RepositoryR<TimeCourse>()
                        .FindBy(y => (y.DateStart.Date >= DateTime.Parse(fromDateStart).Date && y.DateStart.Date <= DateTime.Parse(toDateStart).Date)
                            && (y.DateEnd.Date >= DateTime.Parse(fromDateEnd).Date && y.DateEnd.Date <= DateTime.Parse(toDateEnd).Date))
                        .Select(x => x.CourseId).ToList();
                    }
                    listCourse = await _unitOfWork.RepositoryR<Course>().FindBy(x =>
                                                       (keySearch == null ||
                                                           (x.Code.ToLower().Contains(keySearch.ToLower()) ||
                                                            x.Name.ToLower().Contains(keySearch.ToLower()))) &&
                                                       listCourseId.Contains(x.Id) &&
                                                       (objects == null || x.Objects.ToLower().Equals(objects.ToLower()))).ToListAsync();
                }
                var total = listCourse.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var tmp = listCourse.Skip((pageNum - 1) * pageSize).Take(pageSize).OrderBy(x => x.Id).ToList();
                var result = new List<CourseViewModel>();
                foreach (var item in tmp)
                {
                    List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                    var timeCourse = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == item.Id).ToArray();
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

                            timeItem.Date =  timeCourseItem.DateStart.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            var dateStart = timeCourseItem.DateStart.DayOfWeek.GetHashCode();
                            var descriptionDateStart = Extensions.GetDescription((EnumData.DayOfWeek)dateStart);

                            var dateEnd = timeCourseItem.DateEnd.DayOfWeek.GetHashCode();
                            var descriptionDateEnd = Extensions.GetDescription((EnumData.DayOfWeek)dateEnd);

                            timeItem.Date = timeCourseItem.DateStart.ToString("dd/MM/yyyy") + " - " +
                                             timeCourseItem.DateEnd.ToString("dd/MM/yyyy");
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
                    var itemResult = new CourseViewModel()
                    {
                        Id = item.Id,
                        IsEnabled = item.IsEnabled,
                        Code = item.Code,
                        Name = item.Name,
                        IsActive = item.IsActive,
                        Fee = item.Fee,
                        Objects = item.Objects,
                        CreatedWhen = item.CreatedWhen.GetValueOrDefault(),
                        NumberOfAttendees = item.NumberOfAttendees.GetValueOrDefault(0),
                        TimeCourses = time.ToArray(),
                        Form = item.Form
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

        public async Task<JsonResult> DeActiveCourse(int courseId)
        {
            try
            {
                var detailCourse = await _unitOfWork.RepositoryR<Course>().GetSingleAsync(x => x.Id == courseId);
                if (detailCourse.IsActive == true)
                {

                    if (_unitOfWork.RepositoryR<TimeCourse>().Any(x => x.CourseId == courseId && x.DateEnd >= DateTime.Now))
                    {
                        if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x => x.CourseId == courseId))
                        {
                            return JsonUtil.Error(ValidatorMessage.Course.NotDeActive);
                        }
                    }
                }
                else
                {
                    if (!_unitOfWork.RepositoryR<TimeCourse>().Any(x => x.DateEnd > DateTime.Now && x.CourseId == courseId))
                    {
                        return JsonUtil.Error(ValidatorMessage.Course.NotActive);
                    }
                }
                detailCourse.IsActive = !detailCourse.IsActive;
                _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledCourse(int courseId)
        {
            try
            {
                var detailCourse = await _unitOfWork.RepositoryR<Course>().GetSingleAsync(x => x.Id == courseId);
                if (_unitOfWork.RepositoryR<TimeCourse>().Any(x => x.CourseId == courseId && x.DateEnd >= DateTime.Now))
                {
                    if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x => x.CourseId == courseId))
                    {
                        return JsonUtil.Error(ValidatorMessage.Course.NotDeEnabled);
                    }
                }
                detailCourse.IsEnabled = !detailCourse.IsEnabled;
                _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                await _unitOfWork.CommitAsync();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailCourse(int courseId)
        {
            try
            {
                var detailCourse = await _unitOfWork.RepositoryR<Course>().GetSingleAsync(x => x.Id == courseId);
                if (detailCourse == null) return JsonUtil.Error(ValidatorMessage.Course.NotExist);
                var timeCourse = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == courseId).ToList();
                var arrayTimeCourse = new List<TimeCourseModel>();
                foreach (var timeCourseItem in timeCourse)
                {
                    var timeItem = new TimeCourseModel()
                    {
                        DateStart = timeCourseItem.DateStart.ToString("yyyy-MM-dd"),
                        TimeStart = timeCourseItem.DateStart.ToString("HH:mm"),
                        DateEnd = timeCourseItem.DateEnd.ToString("yyyy-MM-dd"),
                        TimeEnd = timeCourseItem.DateEnd.ToString("HH:mm")
                    };
                    arrayTimeCourse.Add(timeItem);
                }
                CourseViewModelUpdate result = new CourseViewModelUpdate()
                {
                    Id = detailCourse.Id,
                    Code = detailCourse.Code,
                    Name = detailCourse.Name,
                    TimeCourses = arrayTimeCourse.ToArray(),
                    Fee = detailCourse.Fee,
                    Form = detailCourse.Form,
                    Scores = detailCourse.Scores.GetValueOrDefault(),
                    LongDescription = detailCourse.LongDescription,
                    ShortDescription = detailCourse.ShortDescription,
                    Objects = detailCourse.Objects,
                    ImagePath = new string[]{},
                    VideoPath = new string[] {}
                };
                if (!string.IsNullOrEmpty(detailCourse.ImagePath))
                {
                    result.ImagePath = detailCourse.ImagePath.Split(",");
                }
                if (!string.IsNullOrEmpty(detailCourse.VideoPath))
                {
                    result.VideoPath = detailCourse.VideoPath.Split(",");
                }

                bool isSave = _unitOfWork.RepositoryR<TimeCourse>().Any(x => x.DateEnd > DateTime.Now && x.CourseId == courseId);
                return JsonUtil.Success(new
                {
                    Id = result.Id,
                    Code = result.Code,
                    Name = result.Name,
                    TimeCourses = arrayTimeCourse,
                    Fee = result.Fee,
                    Form = result.Form,
                    Scores = result.Scores,
                    LongDescription = result.LongDescription,
                    ShortDescription = result.ShortDescription,
                    Objects = result.Objects,
                    ImagePath = result.ImagePath,
                    VideoPath = result.VideoPath,
                    Save = isSave
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public Course DetailCourse(int id)
        {
            return _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == id);
        }

        public async Task<JsonResult> CreateCourse(CourseViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Course.NameNotEmpty);

                if (!model.TimeCourses.Any())
                    return JsonUtil.Error(ValidatorMessage.Course.NotDateTimeEvent);

                foreach (var modelTimeCourse in model.TimeCourses)
                {
                    if (string.IsNullOrEmpty(modelTimeCourse.DateStart) || string.IsNullOrWhiteSpace(modelTimeCourse.DateStart))
                        return JsonUtil.Error(ValidatorMessage.Course.NotDateStartEvent);
                    if (string.IsNullOrEmpty(modelTimeCourse.DateEnd) || string.IsNullOrWhiteSpace(modelTimeCourse.DateEnd))
                        return JsonUtil.Error(ValidatorMessage.Course.NotDateEndEvent);
                    if (string.IsNullOrEmpty(modelTimeCourse.TimeStart) || string.IsNullOrWhiteSpace(modelTimeCourse.TimeStart))
                        return JsonUtil.Error(ValidatorMessage.Course.NotTimeStartEvent);
                    if (string.IsNullOrEmpty(modelTimeCourse.TimeEnd) || string.IsNullOrWhiteSpace(modelTimeCourse.TimeEnd))
                        return JsonUtil.Error(ValidatorMessage.Course.NotTimeEndEvent);
                }

                Course detail = new Course()
                {
                    Fee = model.Fee,
                    IsActive = true,
                    LongDescription = model.LongDescription,
                    Form = model.Form,
                    Scores = model.Scores,
                    Name = model.Name,
                    Objects = model.Objects,
                    ShortDescription = model.ShortDescription
                };
                _unitOfWork.RepositoryCRUD<Course>().Insert(detail);
                await _unitOfWork.CommitAsync();

                var tmp = "";
                for (int i = 0; i < (6 - detail.Id.ToString().Length); i++)
                {
                    tmp += "0";
                }

                var code = "KH" + String.Format("{0:MM}", detail.CreatedWhen.GetValueOrDefault()) +
                           detail.CreatedWhen.GetValueOrDefault().Year + tmp + detail.Id;
                detail.Code = code;

                _unitOfWork.RepositoryCRUD<Course>().Update(detail);
                await _unitOfWork.CommitAsync();
                
                foreach (var modelTimeCourse in model.TimeCourses)
                {
                    var dateStart = DateTime.ParseExact(modelTimeCourse.DateStart + " " + modelTimeCourse.TimeStart,
                        "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    var dateEnd = DateTime.ParseExact(modelTimeCourse.DateEnd + " " + modelTimeCourse.TimeEnd,
                        "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    TimeCourse timeCourse = new TimeCourse()
                    {
                        Id = 0,
                        IsEnabled = true,
                        CourseId = detail.Id,
                        DateStart = dateStart,
                        DateEnd = dateEnd
                    };

                    _unitOfWork.RepositoryCRUD<TimeCourse>().Insert(timeCourse);
                    await _unitOfWork.CommitAsync();
                }

                return JsonUtil.Success(detail.Id);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateCourse(CourseViewModelUpdate model)
        {
            try
            {
                if (!model.TimeCourses.Any())
                    return JsonUtil.Error(ValidatorMessage.Course.NotDateTimeEvent);

                foreach (var modelTimeCourse in model.TimeCourses)
                {
                    if (string.IsNullOrEmpty(modelTimeCourse.DateStart) || string.IsNullOrWhiteSpace(modelTimeCourse.DateStart))
                        return JsonUtil.Error(ValidatorMessage.Course.NotDateStartEvent);
                    if (string.IsNullOrEmpty(modelTimeCourse.DateEnd) || string.IsNullOrWhiteSpace(modelTimeCourse.DateEnd))
                        return JsonUtil.Error(ValidatorMessage.Course.NotDateEndEvent);
                    if (string.IsNullOrEmpty(modelTimeCourse.TimeStart) || string.IsNullOrWhiteSpace(modelTimeCourse.TimeStart))
                        return JsonUtil.Error(ValidatorMessage.Course.NotTimeStartEvent);
                    if (string.IsNullOrEmpty(modelTimeCourse.TimeEnd) || string.IsNullOrWhiteSpace(modelTimeCourse.TimeEnd))
                        return JsonUtil.Error(ValidatorMessage.Course.NotTimeEndEvent);
                }
                var detailCourse = await _unitOfWork.RepositoryR<Course>().GetSingleAsync(x => x.Id == model.Id);
                //if (!detailCourse.Form.ToLower().Equals(model.Form))
                //{
                //    if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x => x.CourseId == model.Id))
                //    {
                //        return JsonUtil.Error(ValidatorMessage.Course.NotChangeForm);
                //    }
                //}
                detailCourse.Fee = model.Fee;
                detailCourse.Form = model.Form;
                detailCourse.Scores = model.Scores;
                detailCourse.LongDescription = model.LongDescription;
                detailCourse.Objects = model.Objects;
                detailCourse.ShortDescription = model.ShortDescription;
                detailCourse.Name = model.Name;
                if (model.ImagePath != null)
                {
                    detailCourse.ImagePath = string.Join(",", model.ImagePath);
                }
                else
                {
                    detailCourse.ImagePath = null;
                }

                if (model.VideoPath != null)
                {
                    detailCourse.VideoPath = string.Join(",", model.VideoPath);
                }
                else
                {
                    detailCourse.VideoPath = null;
                }

                _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                await _unitOfWork.CommitAsync();
                if (model.TimeCourses.Any())
                {
                    var timeCourse = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == model.Id).ToArray();
                    if (model.TimeCourses.Length == timeCourse.Length)
                    {
                        for (int i = 0; i < timeCourse.Length; i++)
                        {
                            var dateStart = DateTime.ParseExact(model.TimeCourses[i].DateStart + " " + model.TimeCourses[i].TimeStart,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            var dateEnd = DateTime.ParseExact(model.TimeCourses[i].DateEnd + " " + model.TimeCourses[i].TimeEnd,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                            timeCourse[i].DateStart = dateStart;
                            timeCourse[i].DateEnd = dateEnd;
                            _unitOfWork.RepositoryCRUD<TimeCourse>().Update(timeCourse[i]);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    if (model.TimeCourses.Length < timeCourse.Length)
                    {
                        for (int i = 0; i < model.TimeCourses.Length; i++)
                        {

                            var dateStart = DateTime.ParseExact(model.TimeCourses[i].DateStart + " " + model.TimeCourses[i].TimeStart,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            var dateEnd = DateTime.ParseExact(model.TimeCourses[i].DateEnd + " " + model.TimeCourses[i].TimeEnd,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                            timeCourse[i].DateStart = dateStart;
                            timeCourse[i].DateEnd = dateEnd;
                            _unitOfWork.RepositoryCRUD<TimeCourse>().Update(timeCourse[i]);
                            await _unitOfWork.CommitAsync();
                        }

                        for (int i = model.TimeCourses.Length; i < timeCourse.Length; i++)
                        {
                            _unitOfWork.RepositoryCRUD<TimeCourse>().Delete(timeCourse[i]);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    if (model.TimeCourses.Length > timeCourse.Length)
                    {
                        for (int i = 0; i < timeCourse.Length; i++)
                        {
                            var dateStart = DateTime.ParseExact(model.TimeCourses[i].DateStart + " " + model.TimeCourses[i].TimeStart,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            var dateEnd = DateTime.ParseExact(model.TimeCourses[i].DateEnd + " " + model.TimeCourses[i].TimeEnd,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                            timeCourse[i].DateStart = dateStart;
                            timeCourse[i].DateEnd = dateEnd;
                            _unitOfWork.RepositoryCRUD<TimeCourse>().Update(timeCourse[i]);
                            await _unitOfWork.CommitAsync();
                        }

                        for (int i = timeCourse.Length; i < model.TimeCourses.Length; i++)
                        {
                            var dateStart = DateTime.ParseExact(model.TimeCourses[i].DateStart + " " + model.TimeCourses[i].TimeStart,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            var dateEnd = DateTime.ParseExact(model.TimeCourses[i].DateEnd + " " + model.TimeCourses[i].TimeEnd,
                                "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            TimeCourse time = new TimeCourse()
                            {
                                Id = 0,
                                IsEnabled = true,
                                CourseId = model.Id,
                                DateStart = dateStart,
                                DateEnd = dateEnd
                            };
                            _unitOfWork.RepositoryCRUD<TimeCourse>().Insert(time);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadImageCourse(CourseViewModelUploadImage model)
        {
            try
            {
                if (!model.Files.Any())
                    return JsonUtil.Error("File not selected");

                List<string> path = new List<string>();
                if (model.Files.Any())
                {
                    var code = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == model.CourseId).Code;

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
                            await _fileService.UploadImageOptional(file, "Course",
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

        public async Task<JsonResult> UploadVideoCourse(CourseViewModelUploadImage model)
        {
            try
            {
                if (!model.Files.Any())
                    return JsonUtil.Error("File not selected");
                
                List<string> path = new List<string>();
                if (model.Files.Any())
                {
                    var code = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == model.CourseId).Code;

                    foreach (var file in model.Files)
                    {
                        if (file.Length > 2148000000)
                        {
                            return JsonUtil.Error("Dung lượng tệp quá giới hạn cho phép, vui lòng chọn lại!");
                        }
                        var uploadFile =
                            await _fileService.UploadFile(file, "Course",
                                 "Video_"+ code + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
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

        public JsonResult UpdateImageCourse(CourseViewModelUpdateFile model)
        {
            try
            {
                var detailCourse = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == model.Id);
                detailCourse.ImagePath = string.Join(",", model.ImagePath);
                detailCourse.VideoPath = string.Join(",", model.VideoPath);
                _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                _unitOfWork.Commit();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListCustomerCourse(int courseId, string keySearch, int typeId, int status, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                bool? statusCourse = null;
                if (status == 0) statusCourse = null;
                if (status == 1) statusCourse = false;
                if (status == 2) statusCourse = true;
                var data = _unitOfWork.Repository<Proc_GetListCustomerCourse>()
                    .ExecProcedure(Proc_GetListCustomerCourse.GetEntityProc(keySearch, courseId, typeId, statusCourse, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult UpdateNoteCustomerCourse(int customerCourseId, string note)
        {
            try
            {
                var customerCourse = _unitOfWork.RepositoryR<CustomerCourse>().GetSingle(x => x.Id == customerCourseId);
                customerCourse.Note = note;
                _unitOfWork.RepositoryCRUD<CustomerCourse>().Update(customerCourse);
                _unitOfWork.Commit();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllCourseFee(int type)
        {
            try
            {
                List<Course> result;
                if (type == 0) //Khoá học
                {
                    result = _unitOfWork.RepositoryR<Course>().FindBy(x => x.IsActive == true && x.Form.ToLower().Equals("khoá học") && x.Fee == true)
                        .ToList();
                }
                else // Video
                {
                    result = _unitOfWork.RepositoryR<Course>().FindBy(x => x.IsActive == true && x.Form.ToLower().Equals("video") && x.Fee == true)
                        .ToList();
                }

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListTransactionCourse(int courseId, string keySearch, DateTime fromDate, DateTime toDate, int chapterId, int statusId, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                if (courseId == 0) return JsonUtil.Success(new List<Proc_GetListTransactionCourse>(), "Success");
                var data = _unitOfWork.Repository<Proc_GetListTransactionCourse>()
                    .ExecProcedure(Proc_GetListTransactionCourse.GetEntityProc(courseId, keySearch, fromDate, toDate.AddDays(1), chapterId, statusId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> ActiveTransactionCourse(int transactionCourseId, int active, string note, int customerId)
        {
            try
            {
                var transaction = await _unitOfWork.RepositoryR<TransactionCourse>()
                    .GetSingleAsync(x => x.Id == transactionCourseId);

                var customerCourse = await _unitOfWork.RepositoryR<CustomerCourse>().GetSingleAsync(x =>
                    x.CustomerId == transaction.CustomerId
                    && x.CourseId == transaction.CourseId);

                var detailCourse = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == transaction.CourseId);
                if (active != 0) // kích hoạt
                {
                    transaction.StatusId = (int)EnumData.TransactionStatusEnum.Accepted;
                    transaction.Note = note;
                    transaction.DateActive = DateTime.Now;

                    customerCourse.Status = true; // đã thanh toán

                    if (detailCourse.NumberOfAttendees == null) detailCourse.NumberOfAttendees = 0;
                    detailCourse.NumberOfAttendees += 1;


                    if (detailCourse.Form.ToLower().Equals("video"))
                    {
                        string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == transaction.CustomerId).Language;
                        if (language != null)
                        {
                            if (language.Equals("vi"))
                            {
                                var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                    string.Format(ValidatorMessage.ContentNotify.RegisterVideo, detailCourse.Name),
                                    (int)EnumData.NotifyType.Video, transaction.CourseId, null, null);

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
                                    string.Format(ValidatorMessage.ContentNotify.RegisterVideoEnglish, detailCourse.Name),
                                    (int)EnumData.NotifyType.Video, transaction.CourseId, null, null);

                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }
                        }else{
                            var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                string.Format(ValidatorMessage.ContentNotify.RegisterVideo, detailCourse.Name),
                                (int)EnumData.NotifyType.Video, transaction.CourseId, null, null);

                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }

                        

                        string formatPhoneNumber = customerCourse.PhoneNumber.Substring(0, 1);
                        if (formatPhoneNumber == "0")
                        {
                            formatPhoneNumber = customerCourse.PhoneNumber.Substring(1, customerCourse.PhoneNumber.Length - 1);
                        }
                        else
                        {
                            formatPhoneNumber = customerCourse.PhoneNumber;
                        }

                        var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                        string content = $"Chuc mung Anh/Chi da dang ky thanh cong video cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline}";
                       

                        _accountService.SendMailCustomer(customerId, "Mua video thành công", content);
                        var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);


                        _unitOfWork.RepositoryCRUD<TransactionCourse>().Update(transaction);
                        await _unitOfWork.CommitAsync();

                        _unitOfWork.RepositoryCRUD<CustomerCourse>().Update(customerCourse);
                        await _unitOfWork.CommitAsync();

                        _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
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
                    else
                    {
                        string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                        if(language != null)
                        {
                            if (language.Equals("vi"))
                            {
                                var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                    string.Format(ValidatorMessage.ContentNotify.RegisterCourse, detailCourse.Name),
                                    (int)EnumData.NotifyType.Course, transaction.CourseId, null, null);
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
                                    string.Format(ValidatorMessage.ContentNotify.RegisterCourseEnglish, detailCourse.Name),
                                    (int)EnumData.NotifyType.Course, transaction.CourseId, null, null);
                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }
                        }else{
                            var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                string.Format(ValidatorMessage.ContentNotify.RegisterCourse, detailCourse.Name),
                                (int)EnumData.NotifyType.Course, transaction.CourseId, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }

                        

                        string formatPhoneNumber = customerCourse.PhoneNumber.Substring(0, 1);
                        if (formatPhoneNumber == "0")
                        {
                            formatPhoneNumber = customerCourse.PhoneNumber.Substring(1, customerCourse.PhoneNumber.Length - 1);
                        }
                        else
                        {
                            formatPhoneNumber = customerCourse.PhoneNumber;
                        }

                        var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                        string content = $"Chuc mung Anh/Chi da dang ky thanh cong khoa hoc cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline}";
                        

                        _accountService.SendMailCustomer(customerId, "Mua khóa học thành công", content);
                        var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);

                        _unitOfWork.RepositoryCRUD<TransactionCourse>().Update(transaction);
                        await _unitOfWork.CommitAsync();

                        _unitOfWork.RepositoryCRUD<CustomerCourse>().Update(customerCourse);
                        await _unitOfWork.CommitAsync();

                        _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
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

                }
                else // Từ chối
                {
                    transaction.StatusId = (int)EnumData.TransactionStatusEnum.Cancel;
                    transaction.Note = note;

                    customerCourse.IsEnabled = false;

                    _unitOfWork.RepositoryCRUD<TransactionCourse>().Update(transaction);
                    await _unitOfWork.CommitAsync();

                    _unitOfWork.RepositoryCRUD<CustomerCourse>().Update(customerCourse);
                    await _unitOfWork.CommitAsync();

                    string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                    if(language != null)
                    {
                        if (language.Equals("vi"))
                        {
                            if (detailCourse.Form.ToLower().Equals("video"))
                            {
                                var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                    string.Format(ValidatorMessage.ContentNotify.CancelRegisterVideo, detailCourse.Name),
                                    (int)EnumData.NotifyType.Video, transaction.CourseId, customerId, note);
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
                                    string.Format(ValidatorMessage.ContentNotify.CancelRegisterCourse, detailCourse.Name),
                                    (int)EnumData.NotifyType.Course, transaction.CourseId, customerId, note);
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
                            if (detailCourse.Form.ToLower().Equals("video"))
                            {
                                var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                    string.Format(ValidatorMessage.ContentNotify.CancelRegisterVideoEnglish, detailCourse.Name),
                                    (int)EnumData.NotifyType.Video, transaction.CourseId, customerId, note);
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
                                    string.Format(ValidatorMessage.ContentNotify.CancelRegisterCourseEnglish, detailCourse.Name),
                                    (int)EnumData.NotifyType.Course, transaction.CourseId, customerId, note);
                                var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                                var isSuccess = (int)success;
                                if (isSuccess == 0)
                                {
                                    return notify;
                                }
                            }

                        }
                    }else{
                        if (detailCourse.Form.ToLower().Equals("video"))
                        {
                            var notify = _notifyService.CreateNotify(transaction.CustomerId,
                                string.Format(ValidatorMessage.ContentNotify.CancelRegisterVideo, detailCourse.Name),
                                (int)EnumData.NotifyType.Video, transaction.CourseId, customerId, note);
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
                                string.Format(ValidatorMessage.ContentNotify.CancelRegisterCourse, detailCourse.Name),
                                (int)EnumData.NotifyType.Course, transaction.CourseId, customerId, note);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
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

        public JsonResult GetListAssess(int courseId)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListAssess>()
                    .ExecProcedure(Proc_GetListAssess.GetEntityProc(courseId)).ToList();
                if (data.Count <= 0)
                {
                    return JsonUtil.Success(new CourseViewModelAssess()
                    {
                        Assess = data,
                        Sum = 0
                    });
                }
                var sum = (float) data.Select(x => x.Value).Sum() / data.Count;
                
                return JsonUtil.Success(new CourseViewModelAssess()
                {
                    Assess = data,
                    Sum = sum
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListCourseMobile(string keySearch, int customerId, int courseType, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListCourseMobile>()
                    .ExecProcedure(Proc_GetListCourseMobile.GetEntityProc(keySearch,customerId, courseType, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<CourseViewModelMobile> result = new List<CourseViewModelMobile>();
                foreach (var item in data)
                {
                    List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                    var timeCourse = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == item.CourseId).ToArray();
                    if (courseType == 0)
                    {
                        for (int i = 0; i < timeCourse.Length; i++)
                        {
                            if (timeCourse[i].DateEnd > DateTime.Now)
                            {
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

                                CourseViewModelMobile courseViewModelMobile = new CourseViewModelMobile()
                                {
                                    TimeCourses = time.ToArray(),
                                    CourseId = item.CourseId,
                                    CourseType = item.CourseType,
                                    CourseCode = item.CourseCode,
                                    CourseName = item.CourseName,
                                    Liked = item.Liked,
                                    LongDescription = item.LongDescription,
                                    QrInformation = item.QrInformation,
                                    Objects = item.Objects,
                                    IsFee = item.IsFee,
                                    RowNum = item.RowNum,
                                    Shared = item.Shared,
                                    ShortDescription = item.ShortDescription,
                                    SumLike = item.SumLike,
                                    SumShare = item.SumShare,
                                    Assess = item.Assess,
                                    Assessed = item.Assessed,
                                    Scores = item.Scores,
                                    SumComment = item.SumComment,
                                    CertificatePath = item.CertificatePath,
                                    DateCertificate = item.DateCertificate,
                                    StatusCertificate = item.StatusCertificate,
                                    Total = item.Total
                                };
                                if (!string.IsNullOrEmpty(item.ImagePath))
                                {
                                    courseViewModelMobile.ImagePath = item.ImagePath.Split(",");
                                }
                                else
                                {
                                    courseViewModelMobile.ImagePath = new string[] { };
                                }
                                result.Add(courseViewModelMobile);
                                i = timeCourse.Length;
                            }

                        }

                    }
                    else
                    {
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

                        CourseViewModelMobile courseViewModelMobile = new CourseViewModelMobile()
                        {
                            TimeCourses = time.ToArray(),
                            CourseId = item.CourseId,
                            CourseType = item.CourseType,
                            CourseCode = item.CourseCode,
                            CourseName = item.CourseName,
                            Liked = item.Liked,
                            LongDescription = item.LongDescription,
                            QrInformation = item.QrInformation,
                            Objects = item.Objects,
                            IsFee = item.IsFee,
                            RowNum = item.RowNum,
                            Shared = item.Shared,
                            ShortDescription = item.ShortDescription,
                            SumLike = item.SumLike,
                            SumShare = item.SumShare,
                            Assess = item.Assess,
                            Assessed = item.Assessed,
                            Scores = item.Scores,
                            SumComment = item.SumComment,
                            CertificatePath = item.CertificatePath,
                            DateCertificate = item.DateCertificate,
                            StatusCertificate = item.StatusCertificate,
                            Total = item.Total
                        };
                        if (!string.IsNullOrEmpty(item.ImagePath))
                        {
                            courseViewModelMobile.ImagePath = item.ImagePath.Split(",");
                        }
                        else
                        {
                            courseViewModelMobile.ImagePath = new string[] { };
                        }
                        result.Add(courseViewModelMobile);
                    }
                    
                }
                return JsonUtil.Success(result, "Success", result.Count);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailCourseMobile(int customerId, string courseCode)
        {
            try
            {
                var course = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Code == courseCode);
                if (course == null) return JsonUtil.Error(ValidatorMessage.Course.NotExist);
                if (course.IsActive == false) return JsonUtil.Error(ValidatorMessage.Course.CourseIsActive);
                var data = _unitOfWork.Repository<Proc_GetListAssess>()
                    .ExecProcedure(Proc_GetListAssess.GetEntityProc(course.Id)).ToList();
                float sum;
                if (data.Count <= 0)
                {
                    sum = 0;
                }
                else
                {
                    sum = (float)data.Select(x => x.Value).Sum() / data.Count;
                }
                var sumComment = _unitOfWork.Repository<Proc_GetListAssess>()
                    .ExecProcedure(Proc_GetListAssess.GetEntityProc(course.Id)).Select(x => x.Comment).Count();
                var customerLikeCourse = _unitOfWork.RepositoryR<CustomerLikeCourse>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == course.Id && x.IsLiked == true);
                var customerShareCourse = _unitOfWork.RepositoryR<CustomerShareCourse>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == course.Id && x.IsShared == true);
                var sumLike = _unitOfWork.RepositoryR<CustomerLikeCourse>()
                    .Count(x => x.CourseId == course.Id && x.IsLiked == true);
                var sumShare = _unitOfWork.RepositoryR<CustomerShareCourse>()
                    .Count(x => x.CourseId == course.Id && x.IsShared == true);

                int courseType = 0;
                if (course.Fee == false)
                {
                    if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x =>
                        x.CourseId == course.Id && x.CustomerId == customerId && x.Status == false))
                    {
                        courseType = 1;
                    }
                }
                else
                {
                    if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x =>
                        x.CourseId == course.Id && x.CustomerId == customerId && x.Status == true))
                    {
                        courseType = 1;
                    }
                }

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                var timeCourse = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == course.Id).ToArray();
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
                        var descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
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

                var assessed = _unitOfWork.RepositoryR<Assess>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == course.Id);
                if(language != null){
                    if (language.Equals("vi"))
                    {
                        return JsonUtil.Success(new
                        {
                            CourseId = course.Id,
                            CourseName = course.Name,
                            CourseCode = course.Code,
                            CourseType = courseType,
                            TimeCourses = time.ToArray(),
                            ShortDescription = course.ShortDescription,
                            LongDescription = course.LongDescription,
                            QrInformation = course.QrInformation,
                            Objects = course.Objects,
                            ImagePath = course.ImagePath,
                            VideoPath = course.VideoPath,
                            Assess = sum,
                            IsFee = course.Fee,
                            Liked = customerLikeCourse,
                            SumLike = sumLike,
                            Shared = customerShareCourse,
                            sumShare = sumShare,
                            SumComment = sumComment,
                            Scores = course.Scores,
                            Assessed = assessed
                        });

                    }
                    else
                    {
                        string objectCourse = "";
                        if (course.Objects.Equals("Tất cả"))
                        {
                            objectCourse = "All User";
                        }
                        else
                        {
                            objectCourse = "Member OBC";
                        }
                        return JsonUtil.Success(new
                        {
                            CourseId = course.Id,
                            CourseName = course.Name,
                            CourseCode = course.Code,
                            CourseType = courseType,
                            TimeCourses = time.ToArray(),
                            ShortDescription = course.ShortDescription,
                            LongDescription = course.LongDescription,
                            QrInformation = course.QrInformation,
                            Objects = objectCourse,
                            ImagePath = course.ImagePath,
                            VideoPath = course.VideoPath,
                            Assess = sum,
                            IsFee = course.Fee,
                            Liked = customerLikeCourse,
                            SumLike = sumLike,
                            Shared = customerShareCourse,
                            sumShare = sumShare,
                            SumComment = sumComment,
                            Scores = course.Scores,
                            Assessed = assessed
                        });
                    }
                }else{
                    return JsonUtil.Success(new
                    {
                        CourseId = course.Id,
                        CourseName = course.Name,
                        CourseCode = course.Code,
                        CourseType = courseType,
                        TimeCourses = time.ToArray(),
                        ShortDescription = course.ShortDescription,
                        LongDescription = course.LongDescription,
                        QrInformation = course.QrInformation,
                        Objects = course.Objects,
                        ImagePath = course.ImagePath,
                        VideoPath = course.VideoPath,
                        Assess = sum,
                        IsFee = course.Fee,
                        Liked = customerLikeCourse,
                        SumLike = sumLike,
                        Shared = customerShareCourse,
                        sumShare = sumShare,
                        SumComment = sumComment,
                        Scores = course.Scores,
                        Assessed = assessed
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult LikedCourse(int customerId, int courseId)
        {
            try
            {
                var customerLikeCourse = _unitOfWork.RepositoryR<CustomerLikeCourse>()
                    .GetSingle(x => x.CustomerId == customerId && x.CourseId == courseId);
                if (customerLikeCourse == null)
                {
                    CustomerLikeCourse likedCourse = new CustomerLikeCourse()
                    {
                        Id = 0,
                        CustomerId = customerId,
                        IsEnabled = true,
                        CourseId = courseId,
                        IsLiked = true
                    };

                    _unitOfWork.RepositoryCRUD<CustomerLikeCourse>().Insert(likedCourse);
                    _unitOfWork.Commit();
                }
                else
                {
                    customerLikeCourse.IsLiked = !customerLikeCourse.IsLiked;
                    _unitOfWork.RepositoryCRUD<CustomerLikeCourse>().Update(customerLikeCourse);
                    _unitOfWork.Commit();
                }

                var sumLike = _unitOfWork.RepositoryR<CustomerLikeCourse>().Count(x => x.CourseId == courseId && x.IsLiked == true);

                return JsonUtil.Success(sumLike);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SharedCourse(int customerId, int courseId)
        {
            try
            {
                var customerShareCourse = _unitOfWork.RepositoryR<CustomerShareCourse>()
                    .GetSingle(x => x.CustomerId == customerId && x.CourseId == courseId);
                if (customerShareCourse == null)
                {
                    CustomerShareCourse shareCourse = new CustomerShareCourse()
                    {
                        Id = 0,
                        CustomerId = customerId,
                        IsEnabled = true,
                        CourseId = courseId,
                        IsShared = true
                    };

                    _unitOfWork.RepositoryCRUD<CustomerShareCourse>().Update(shareCourse);
                    _unitOfWork.Commit();
                }
                else
                {
                    customerShareCourse.IsShared = !customerShareCourse.IsShared;
                    _unitOfWork.RepositoryCRUD<CustomerShareCourse>().Update(customerShareCourse);
                    _unitOfWork.Commit();
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckCustomerRegisterCourse(int customerId, int courseId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var detailCourse = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == courseId);
                var transactionCourse = _unitOfWork.RepositoryR<TransactionCourse>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == courseId &&
                              x.StatusId == (int)EnumData.TransactionStatusEnum.PendingActive);
                if (detailCourse.Objects.ToLower().Equals("thành viên obc"))
                {
                    if (customer.CustomerRoleId == (int)EnumData.CustomerRoleEnum.PremiumMember && customer.StatusId == (int)EnumData.CustomerStatusEnum.Active)
                    {
                        if (transactionCourse)
                        {
                            if (customer.Language.Equals("en"))
                            {
                                if (detailCourse.Form.ToLower().Equals("video"))
                                {
                                    return JsonUtil.Error(ValidatorMessage.Video.NotRegisterEnglish);
                                }
                                else
                                {
                                    return JsonUtil.Error(ValidatorMessage.Course.NotRegisterEnglish);
                                }
                            }
                            else
                            {
                                if (detailCourse.Form.ToLower().Equals("video"))
                                {
                                    return JsonUtil.Error(ValidatorMessage.Video.NotRegister);
                                }
                                else
                                {
                                    return JsonUtil.Error(ValidatorMessage.Course.NotRegister);
                                }
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
                            if (detailCourse.Form.ToLower().Equals("video"))
                            {
                                return JsonUtil.Error(ValidatorMessage.Video.FreeMemberNotRegisterEnglish);
                            }
                            else
                            {
                                return JsonUtil.Error(ValidatorMessage.Course.FreeMemberNotRegisterEnglish);
                            }
                        }
                        else
                        {
                            if (detailCourse.Form.ToLower().Equals("video"))
                            {
                                return JsonUtil.Error(ValidatorMessage.Video.FreeMemberNotRegister);
                            }
                            else
                            {
                                return JsonUtil.Error(ValidatorMessage.Course.FreeMemberNotRegister);
                            }
                        }
                    }
                }
                else
                {
                    if (transactionCourse)
                    {
                        if (customer.Language.Equals("en"))
                        {
                            if (detailCourse.Form.ToLower().Equals("video"))
                            {
                                return JsonUtil.Error(ValidatorMessage.Video.NotRegisterEnglish);
                            }
                            else
                            {
                                return JsonUtil.Error(ValidatorMessage.Course.NotRegisterEnglish);
                            }
                        }
                        else
                        {
                            if (detailCourse.Form.ToLower().Equals("video"))
                            {
                                return JsonUtil.Error(ValidatorMessage.Video.NotRegister);
                            }
                            else
                            {
                                return JsonUtil.Error(ValidatorMessage.Course.NotRegister);
                            }
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

        public async Task<JsonResult> RegisterCourse(int customerId, int courseId, string phoneNumber, string email)
        {
            try
            {
                var detailCourse = await _unitOfWork.RepositoryR<Course>().GetSingleAsync(x => x.Id == courseId);
                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId.GetValueOrDefault();
                CustomerCourse customerCourse = new CustomerCourse()
                {
                    Id = 0,
                    CourseId = courseId,
                    CustomerId = customerId,
                    Status = false, // Đã đăng ký
                    PhoneNumber = phoneNumber,
                    Email = email
                };
                

                if (detailCourse.Form.ToLower().Equals("video"))
                {
                    string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                    if (language.Equals("vi"))
                    {
                        var notify = _notifyService.CreateNotify(customerId,
                            string.Format(ValidatorMessage.ContentNotify.RegisterVideo, detailCourse.Name),
                            (int)EnumData.NotifyType.Video, courseId, null, null);
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
                            string.Format(ValidatorMessage.ContentNotify.RegisterVideoEnglish, detailCourse.Name),
                            (int)EnumData.NotifyType.Video, courseId, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }

                    string formatPhoneNumber = customerCourse.PhoneNumber.Substring(0, 1);
                    if (formatPhoneNumber == "0")
                    {
                        formatPhoneNumber = customerCourse.PhoneNumber.Substring(1, customerCourse.PhoneNumber.Length - 1);
                    }
                    else
                    {
                        formatPhoneNumber = customerCourse.PhoneNumber;
                    }

                    var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                    string content = $"Chuc mung Anh/Chi da dang ky thanh cong video cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline}";
                   

                    _accountService.SendMailCustomer(customerId, "Mua video thành công", content);
                    var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);
                    

                    _unitOfWork.RepositoryCRUD<CustomerCourse>().Insert(customerCourse);
                    await _unitOfWork.CommitAsync();


                    detailCourse.NumberOfAttendees = _unitOfWork.RepositoryR<CustomerCourse>()
                        .Count(x => x.CourseId == courseId && x.Status == false);

                    _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                    await _unitOfWork.CommitAsync();

                    if (sendSms == false)
                    {
                        return JsonUtil.Success(new
                        {
                            CustomerCourseId = customerCourse.Id,
                            TransactionCourseId = 0
                        }, "Đã có lỗi xảy ra khi gửi sms, vui lòng kiểm tra lại!");
                    }
                    else
                    {
                        return JsonUtil.Success(new
                        {
                            CustomerCourseId = customerCourse.Id,
                            TransactionCourseId = 0
                        });
                    }
                }
                else
                {
                    string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                    if(language != null){
                        if (language.Equals("vi"))
                        {
                            var notify = _notifyService.CreateNotify(customerId,
                                string.Format(ValidatorMessage.ContentNotify.RegisterCourse, detailCourse.Name),
                                (int)EnumData.NotifyType.Course, courseId, null, null);
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
                                string.Format(ValidatorMessage.ContentNotify.RegisterCourseEnglish, detailCourse.Name),
                                (int)EnumData.NotifyType.Course, courseId, null, null);
                            var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                            var isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return notify;
                            }
                        }
                    }else{
                        var notify = _notifyService.CreateNotify(customerId,
                            string.Format(ValidatorMessage.ContentNotify.RegisterCourse, detailCourse.Name),
                            (int)EnumData.NotifyType.Course, courseId, null, null);
                        var success = notify.Value.GetType().GetProperty("isSuccess")?.GetValue(notify.Value, null);
                        var isSuccess = (int)success;
                        if (isSuccess == 0)
                        {
                            return notify;
                        }
                    }
                    

                    string formatPhoneNumber = customerCourse.PhoneNumber.Substring(0, 1);
                    if (formatPhoneNumber == "0")
                    {
                        formatPhoneNumber = customerCourse.PhoneNumber.Substring(1, customerCourse.PhoneNumber.Length - 1);
                    }
                    else
                    {
                        formatPhoneNumber = customerCourse.PhoneNumber;
                    }

                    var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;

                    string content = $"Chuc mung Anh/Chi da dang ky thanh cong khoa hoc cua OBC. OBC chan thanh cam on quy Anh/Chi. Hotline ho tro: {hotline}";
                    


                    _accountService.SendMailCustomer(customerId, "Mua khóa học thành công", content);
                    var sendSms = _accountService.SendOTPSOAPViettel(formatPhoneNumber, content);
                    

                    _unitOfWork.RepositoryCRUD<CustomerCourse>().Insert(customerCourse);
                    await _unitOfWork.CommitAsync();


                    detailCourse.NumberOfAttendees = _unitOfWork.RepositoryR<CustomerCourse>()
                        .Count(x => x.CourseId == courseId && x.Status == false);

                    _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                    await _unitOfWork.CommitAsync();

                    if (sendSms == false)
                    {
                        return JsonUtil.Success(new
                        {
                            CustomerCourseId = customerCourse.Id,
                            TransactionCourseId = 0
                        }, "Đã có lỗi xảy ra khi gửi sms, vui lòng kiểm tra lại!");
                    }
                    else
                    {
                        return JsonUtil.Success(new
                        {
                            CustomerCourseId = customerCourse.Id,
                            TransactionCourseId = 0
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadImageTransactionCourse(CourseViewModelUploadImageTransaction model, int customerId)
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

                CustomerCourse customerCourse = new CustomerCourse()
                {
                    Id = 0,
                    CourseId = model.CourseId,
                    CustomerId = customerId,
                    Status = false, // Đã đăng ký
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    ImagePath = null,
                    Note = null,
                    StatusCertificate = false,
                    TimeVideo = 0,
                    DateCertificate = null
                };
                _unitOfWork.RepositoryCRUD<CustomerCourse>().Insert(customerCourse);
                await _unitOfWork.CommitAsync();

                TransactionCourse transactionCourse = new TransactionCourse()
                {
                    Id = 0,
                    ChapterId = chapterId,
                    CustomerId = customerId,
                    CourseId = model.CourseId,
                    StatusId = (int)EnumData.TransactionStatusEnum.PendingActive
                };
                _unitOfWork.RepositoryCRUD<TransactionCourse>().Insert(transactionCourse);
                await _unitOfWork.CommitAsync();

                var tmp = "";
                for (int i = 0; i < (6 - transactionCourse.Id.ToString().Length); i++)
                {
                    tmp += "0";
                }

                var code = "KH" + String.Format("{0:MM}", transactionCourse.CreatedWhen.GetValueOrDefault()) +
                           transactionCourse.CreatedWhen.GetValueOrDefault().Year + tmp + transactionCourse.Id;
                

                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "TransactionCourse",
                        code + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }
                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;

                transactionCourse.Code = code;
                transactionCourse.ImagePath = link;

                _unitOfWork.RepositoryCRUD<TransactionCourse>().Update(transactionCourse);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult AssessCourse(int customerId, int courseId, int value, string comment)
        {
            try
            {
                Assess assess = new Assess()
                {
                    Id = 0,
                    CustomerId = customerId,
                    Comment = comment,
                    CourseId = courseId,
                    Value = value
                };

                _unitOfWork.RepositoryCRUD<Assess>().Insert(assess);
                _unitOfWork.Commit();

                var data = _unitOfWork.Repository<Proc_GetListAssess>()
                    .ExecProcedure(Proc_GetListAssess.GetEntityProc(courseId)).ToList();
                float sum;
                if (data.Count <= 0)
                {
                    sum = 0;
                }
                else
                {
                    sum = (float)data.Select(x => x.Value).Sum() / data.Count;
                }

                var course = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == courseId);
                if (course.Form.ToLower().Equals("video"))
                {
                    return JsonUtil.Success(new
                    {
                        VideoId = courseId,
                        Assess = sum
                    });
                }
                else
                {
                    return JsonUtil.Success(new
                    {
                        CourseId = courseId,
                        Assess = sum
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadCertificate(CourseViewModelUploadCertificate model, int customerId)
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

                var course = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == model.CourseId);
                var customerCourse = _unitOfWork.RepositoryR<CustomerCourse>()
                    .GetSingle(x => x.CourseId == model.CourseId && x.CustomerId == customerId);
                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "Certificate",
                        course.Code + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;

                customerCourse.ImagePath = link;
                customerCourse.StatusCertificate = true;
                customerCourse.DateCertificate = DateTime.Now;

                _unitOfWork.RepositoryCRUD<CustomerCourse>().Update(customerCourse);
                _unitOfWork.Commit();

                return JsonUtil.Success(new
                {
                    CourseName = course.Name,
                    StatusCertificate = "Đã hoàn thành",
                    DateCertificate = customerCourse.DateCertificate,
                    Scores = course.Scores,
                    certificatePath = customerCourse.ImagePath
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetCertificateWithCustomerId(int customerId, int courseId)
        {
            try
            {
                var customerCourse = _unitOfWork.RepositoryR<CustomerCourse>()
                    .GetSingle(x => x.CustomerId == customerId && x.CourseId == courseId);
                var detailCourse = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Id == courseId);
                string status =  customerCourse.StatusCertificate.GetValueOrDefault() == true ? "Đã hoàn thành" : "Chưa hoàn thành";

                return JsonUtil.Success(new
                {
                    CourseName = detailCourse.Name,
                    StatusCertificate = status,
                    DateCertificate = customerCourse.DateCertificate,
                    Scores = detailCourse.Scores,
                    certificatePath = customerCourse.ImagePath
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListVideoMobile(string keySearch, int customerId, int videoType, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListVideoMobile>()
                    .ExecProcedure(Proc_GetListVideoMobile.GetEntityProc(keySearch ,customerId, videoType, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);

                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<VideoViewModelMobile> result = new List<VideoViewModelMobile>();
                foreach (var item in data)
                {
                    List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                    var timeVideo = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == item.VideoId).ToArray();
                    if (videoType == 0)
                    {
                        for (int i = 0; i < timeVideo.Length; i++)
                        {
                            if (timeVideo[i].DateEnd > DateTime.Now)
                            {
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

                                VideoViewModelMobile videoViewModelMobile = new VideoViewModelMobile()
                                {
                                    TimeVideos = time.ToArray(),
                                    VideoId = item.VideoId,
                                    VideoType = item.VideoType,
                                    VideoCode = item.VideoCode,
                                    VideoName = item.VideoName,
                                    Liked = item.Liked,
                                    LongDescription = item.LongDescription,
                                    QrInformation = item.QrInformation,
                                    Objects = item.Objects,
                                    IsFee = item.IsFee,
                                    RowNum = item.RowNum,
                                    Shared = item.Shared,
                                    ShortDescription = item.ShortDescription,
                                    SumLike = item.SumLike,
                                    SumShare = item.SumShare,
                                    Assess = item.Assess,
                                    Assessed = item.Assessed,
                                    Scores = item.Scores,
                                    SumComment = item.SumComment,
                                    Total = item.Total
                                };
                                if (!string.IsNullOrEmpty(item.ImagePath))
                                {
                                    videoViewModelMobile.ImagePath = item.ImagePath.Split(",");
                                }
                                else
                                {
                                    videoViewModelMobile.ImagePath = new string[] { };
                                }
                                if (!string.IsNullOrEmpty(item.VideoPath))
                                {
                                    videoViewModelMobile.VideoPath = item.VideoPath.Split(",");
                                }
                                else
                                {
                                    videoViewModelMobile.VideoPath = new string[] { };
                                }
                                result.Add(videoViewModelMobile);
                                i = timeVideo.Length;
                            }

                        }
                    }
                    else
                    {
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

                        VideoViewModelMobile videoViewModelMobile = new VideoViewModelMobile()
                        {
                            TimeVideos = time.ToArray(),
                            VideoId = item.VideoId,
                            VideoType = item.VideoType,
                            VideoCode = item.VideoCode,
                            VideoName = item.VideoName,
                            Liked = item.Liked,
                            LongDescription = item.LongDescription,
                            QrInformation = item.QrInformation,
                            Objects = item.Objects,
                            IsFee = item.IsFee,
                            RowNum = item.RowNum,
                            Shared = item.Shared,
                            ShortDescription = item.ShortDescription,
                            SumLike = item.SumLike,
                            SumShare = item.SumShare,
                            Assess = item.Assess,
                            Assessed = item.Assessed,
                            Scores = item.Scores,
                            SumComment = item.SumComment,
                            Total = item.Total
                        };
                        if (!string.IsNullOrEmpty(item.ImagePath))
                        {
                            videoViewModelMobile.ImagePath = item.ImagePath.Split(",");
                        }
                        else
                        {
                            videoViewModelMobile.ImagePath = new string[] { };
                        }
                        if (!string.IsNullOrEmpty(item.VideoPath))
                        {
                            videoViewModelMobile.VideoPath = item.VideoPath.Split(",");
                        }
                        else
                        {
                            videoViewModelMobile.VideoPath = new string[] { };
                        }
                        result.Add(videoViewModelMobile);
                    }
                    
                }
                return JsonUtil.Success(result, "Success", result.Count);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadImageTransactionVideo(VideoViewModelUploadImageTransaction model, int customerId)
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

                CustomerCourse customerCourse = new CustomerCourse()
                {
                    Id = 0,
                    CourseId = model.VideoId,
                    CustomerId = customerId,
                    Status = false, // Đã đăng ký
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    ImagePath = null,
                    Note = null,
                    StatusCertificate = false,
                    TimeVideo = 0,
                    DateCertificate = null
                };
                _unitOfWork.RepositoryCRUD<CustomerCourse>().Insert(customerCourse);
                await _unitOfWork.CommitAsync();

                TransactionCourse transactionCourse = new TransactionCourse()
                {
                    Id = 0,
                    ChapterId = chapterId,
                    CustomerId = customerId,
                    CourseId = model.VideoId,
                    StatusId = (int)EnumData.TransactionStatusEnum.PendingActive
                };
                _unitOfWork.RepositoryCRUD<TransactionCourse>().Insert(transactionCourse);
                await _unitOfWork.CommitAsync();

                var tmp = "";
                for (int i = 0; i < (6 - transactionCourse.Id.ToString().Length); i++)
                {
                    tmp += "0";
                }

                var code = "KH" + String.Format("{0:MM}", transactionCourse.CreatedWhen.GetValueOrDefault()) +
                           transactionCourse.CreatedWhen.GetValueOrDefault().Year + tmp + transactionCourse.Id;


                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "TransactionVideo",
                        code + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;

                transactionCourse.Code = code;
                transactionCourse.ImagePath = link;

                _unitOfWork.RepositoryCRUD<TransactionCourse>().Update(transactionCourse);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListAssessMobile(int courseId, int pageNum, int pageSize)
        {
            try
            {
                var result = _unitOfWork.Repository<Proc_GetListAssessMobile>()
                    .ExecProcedure(Proc_GetListAssessMobile.GetEntityProc(courseId,pageNum,pageSize)).ToList();
                var sumComment = _unitOfWork.Repository<Proc_GetListAssessMobile>()
                    .ExecProcedure(Proc_GetListAssessMobile.GetEntityProc(courseId, pageNum, pageSize)).Select(x => x.Comment).Count();
                return JsonUtil.Success(new
                {
                    ListAssess = result,
                    SumComment = sumComment
                }, "Success", result.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult UpdateTimeVideo(int customerId, int videoId, int timeVideo)
        {
            try
            {
                var customerCourse = _unitOfWork.RepositoryR<CustomerCourse>()
                    .GetSingle(x => x.CourseId == videoId && x.CustomerId == customerId);
                customerCourse.TimeVideo = timeVideo;

                _unitOfWork.RepositoryCRUD<CustomerCourse>().Update(customerCourse);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailVideoMobile(int customerId, string videoCode)
        {
            try
            {
                var video = _unitOfWork.RepositoryR<Course>().GetSingle(x => x.Code == videoCode);
                if (video == null) return JsonUtil.Error(ValidatorMessage.Video.NotExist);
                if (video.IsActive == false) return JsonUtil.Error(ValidatorMessage.Video.CourseIsActive);
                var data = _unitOfWork.Repository<Proc_GetListAssess>()
                    .ExecProcedure(Proc_GetListAssess.GetEntityProc(video.Id)).ToList();
                float sum;
                if (data.Count <= 0)
                {
                    sum = 0;
                }
                else
                {
                    sum = (float)data.Select(x => x.Value).Sum() / data.Count;
                }
                var sumComment = _unitOfWork.Repository<Proc_GetListAssess>()
                    .ExecProcedure(Proc_GetListAssess.GetEntityProc(video.Id)).Select(x => x.Comment).Count();
                var customerLikeVideo = _unitOfWork.RepositoryR<CustomerLikeCourse>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == video.Id && x.IsLiked == true);
                var customerShareVideo = _unitOfWork.RepositoryR<CustomerShareCourse>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == video.Id && x.IsShared == true);
                var sumLike = _unitOfWork.RepositoryR<CustomerLikeCourse>()
                    .Count(x => x.CourseId == video.Id && x.IsLiked == true);
                var sumShare = _unitOfWork.RepositoryR<CustomerShareCourse>()
                    .Count(x => x.CourseId == video.Id && x.IsShared == true);
                var customerCourse = _unitOfWork.RepositoryR<CustomerCourse>()
                    .GetSingle(x => x.CourseId == video.Id && x.CustomerId == customerId);
                var assessed = _unitOfWork.RepositoryR<Assess>()
                    .Any(x => x.CustomerId == customerId && x.CourseId == video.Id);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                List<TimeCourseMobile> time = new List<TimeCourseMobile>();
                var timeVideo = _unitOfWork.RepositoryR<TimeCourse>().FindBy(x => x.CourseId == video.Id).ToArray();
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
                        var descriptionDateEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)date);
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
                var imagePath = new string[]{};
                var videoPath = new string[] { };
                if (!string.IsNullOrEmpty(video.ImagePath))
                {
                    imagePath = video.ImagePath.Split(",");
                }
                if (!string.IsNullOrEmpty(video.VideoPath))
                {
                    videoPath = video.VideoPath.Split(",");
                }

                int videoType = 0;
                if (video.Fee == false)
                {
                    if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x =>
                        x.CourseId == video.Id && x.CustomerId == customerId && x.Status == false))
                    {
                        videoType = 1;
                    }
                }
                else
                {
                    if (_unitOfWork.RepositoryR<CustomerCourse>().Any(x =>
                        x.CourseId == video.Id && x.CustomerId == customerId && x.Status == true))
                    {
                        videoType = 1;
                    }
                }

                int? videoTime;
                if (customerCourse == null)
                {
                    videoTime = 0;
                }
                else
                {
                    videoTime = customerCourse.TimeVideo;
                }


                if(language != null){
                    if (language.Equals("vi"))
                    {
                        return JsonUtil.Success(new
                        {
                            VideoId = video.Id,
                            VideoName = video.Name,
                            VideoCode = video.Code,
                            VideoType = videoType,
                            TimeVideos = time.ToArray(),
                            ShortDescription = video.ShortDescription,
                            LongDescription = video.LongDescription,
                            QrInformation = video.QrInformation,
                            Objects = video.Objects,
                            ImagePath = imagePath,
                            VideoPath = videoPath,
                            Assess = sum,
                            IsFee = video.Fee,
                            Liked = customerLikeVideo,
                            SumLike = sumLike,
                            Shared = customerShareVideo,
                            sumShare = sumShare,
                            SumComment = sumComment,
                            videoTime = videoTime,
                            Assessed = assessed,
                            Scores = video.Scores
                        });
                    }
                    else
                    {
                        string objectVideo = "";
                        if (video.Objects.Equals("Tất cả"))
                        {
                            objectVideo = "All User";
                        }
                        else
                        {
                            objectVideo = "Member OBC";
                        }
                        return JsonUtil.Success(new
                        {
                            VideoId = video.Id,
                            VideoName = video.Name,
                            VideoCode = video.Code,
                            VideoType = videoType,
                            TimeVideos = time.ToArray(),
                            ShortDescription = video.ShortDescription,
                            LongDescription = video.LongDescription,
                            QrInformation = video.QrInformation,
                            Objects = objectVideo,
                            ImagePath = imagePath,
                            VideoPath = videoPath,
                            Assess = sum,
                            IsFee = video.Fee,
                            Liked = customerLikeVideo,
                            SumLike = sumLike,
                            Shared = customerShareVideo,
                            sumShare = sumShare,
                            SumComment = sumComment,
                            videoTime = videoTime,
                            Assessed = assessed,
                            Scores = video.Scores
                        });
                    }
                }else{
                    return JsonUtil.Success(new
                    {
                        VideoId = video.Id,
                        VideoName = video.Name,
                        VideoCode = video.Code,
                        VideoType = videoType,
                        TimeVideos = time.ToArray(),
                        ShortDescription = video.ShortDescription,
                        LongDescription = video.LongDescription,
                        QrInformation = video.QrInformation,
                        Objects = video.Objects,
                        ImagePath = imagePath,
                        VideoPath = videoPath,
                        Assess = sum,
                        IsFee = video.Fee,
                        Liked = customerLikeVideo,
                        SumLike = sumLike,
                        Shared = customerShareVideo,
                        sumShare = sumShare,
                        SumComment = sumComment,
                        videoTime = videoTime,
                        Assessed = assessed,
                        Scores = video.Scores
                    });
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GenerateInformationQrCodePath(FileStreamResult streamResult, int courseId)
        {
            try
            {
                var detailCourse = await _unitOfWork.RepositoryR<Course>().GetSingleAsync(x => x.Id == courseId);
                var QrPath = GenerateQrCode(streamResult, "QrCodeInformationCourse_" + detailCourse.Code,
                    "QrCodeInformationCourse");

                detailCourse.QrInformation = QrPath;
                _unitOfWork.RepositoryCRUD<Course>().Update(detailCourse);
                _unitOfWork.Commit();

                return JsonUtil.Success(courseId);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailAssessCourse(int courseId, int customerId)
        {
            try
            {
                var assess = _unitOfWork.RepositoryR<Assess>()
                    .FindBy(x => x.CourseId == courseId && x.CustomerId == customerId).OrderByDescending(x => x.Id).FirstOrDefault();
                return JsonUtil.Success(new
                {
                    Comment = assess.Comment,
                    Value = assess.Value
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public  string GenerateQrCode(FileStreamResult streamResult, string fileName, string folderName)
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

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
