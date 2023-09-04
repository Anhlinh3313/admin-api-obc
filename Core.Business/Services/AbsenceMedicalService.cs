using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.AbsenceMedical;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class AbsenceMedicalService : BaseService, IAbsenceMedicalService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IFileService _fileService;
        public AbsenceMedicalService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor,
            IFileService fileService,
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _fileService = fileService;
        }

        public async Task<JsonResult> CreateAbsenceMedical(AbsenceMedicalViewModel model, int currentId)
        {
            try
            {
                var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentId).Language;
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

                var checkAbsenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().Any(x =>
                    x.CreatedBy == currentId && x.MeetingChapterId == model.MeetingChapterId);
                if (checkAbsenceMedical)
                {
                    if (language.Equals("en"))
                    {
                        return JsonUtil.Error(
                            "You have declared your absence from the meeting at this time, please do not report again");
                    }
                    else
                    {
                        return JsonUtil.Error(
                            "Bạn đã khai báo vắng mặt buổi họp vào thời gian này, vui lòng không khai báo lại");
                    }
                }

                AbsenceMedical absenceMedical = new AbsenceMedical()
                {
                    Content = model.Content,
                    MeetingChapterId = model.MeetingChapterId
                };
                _unitOfWork.RepositoryCRUD<AbsenceMedical>().Insert(absenceMedical);
                await _unitOfWork.CommitAsync();

                var uploadFile =
                    await _fileService.UploadImageOptional(model.File, "AbsenceMedical",
                        "AbsenceMedical" + absenceMedical.Id + "_" + DateTime.Now.ToString("ddMMyyyyHHmmssffff"));
                var success = uploadFile.Value.GetType().GetProperty("isSuccess")?.GetValue(uploadFile.Value, null);
                var isSuccess = (int)success;
                if (isSuccess == 0)
                {
                    return uploadFile;
                }

                var value = uploadFile.Value.GetType().GetProperty("data")?.GetValue(uploadFile.Value, null);
                var link = (dynamic)value;

                absenceMedical.ImagePath = link;

                _unitOfWork.RepositoryCRUD<AbsenceMedical>().Update(absenceMedical);
                await _unitOfWork.CommitAsync();

                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                     .GetSingleNotEnabled(x => x.Id == absenceMedical.MeetingChapterId);

                if (language.Equals("en"))
                {
                    var loopDescription = Extensions.GetDescription((EnumData.LoopMeetingChapterEnglish)meetingChapter.Loop);
                    return JsonUtil.Success(new
                    {
                        AbsenceMedical = new
                        {
                            Id = absenceMedical.Id,
                            Content = absenceMedical.Content,
                            ImagePath = absenceMedical.ImagePath
                        },
                        InformationMeetingChapter = new
                        {
                            MeetingChapterName = meetingChapter.Name,
                            Time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("MM-dd-yyyy"),
                            Loop = loopDescription,
                            DateEnd = meetingChapter.DateEnd,
                            Link = meetingChapter.Link,
                            Address = meetingChapter.Address
                        }
                    });
                }
                else
                {
                    var loopDescription = Extensions.GetDescription((EnumData.LoopMeetingChapter)meetingChapter.Loop);
                    return JsonUtil.Success(new
                    {
                        AbsenceMedical = new
                        {
                            Id = absenceMedical.Id,
                            Content = absenceMedical.Content,
                            ImagePath = absenceMedical.ImagePath
                        },
                        InformationMeetingChapter = new
                        {
                            MeetingChapterName = meetingChapter.Name,
                            Time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                            Loop = loopDescription,
                            DateEnd = meetingChapter.DateEnd,
                            Link = meetingChapter.Link,
                            Address = meetingChapter.Address
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailAbsenceMedical(int absenceMedicalId, int currentId)
        {
            try
            {
                var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x => x.Id == absenceMedicalId);
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .GetSingleNotEnabled(x => x.Id == absenceMedical.MeetingChapterId);
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentId);
                if (customer.Language.Equals("en"))
                {
                    var loopDescription = Extensions.GetDescription((EnumData.LoopMeetingChapterEnglish)meetingChapter.Loop);
                    return JsonUtil.Success(new
                    {
                        AbsenceMedical = new
                        {
                            Id = absenceMedical.Id,
                            Content = absenceMedical.Content,
                            ImagePath = absenceMedical.ImagePath
                        },
                        InformationMeetingChapter = new
                        {
                            MeetingChapterName = meetingChapter.Name,
                            Time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("MM-dd-yyyy"),
                            Loop = loopDescription,
                            DateEnd = meetingChapter.DateEnd,
                            Link = meetingChapter.Link,
                            Address = meetingChapter.Address
                        }
                    });
                }
                else
                {
                    var loopDescription = Extensions.GetDescription((EnumData.LoopMeetingChapter)meetingChapter.Loop);
                    return JsonUtil.Success(new
                    {
                        AbsenceMedical = new
                        {
                            Id = absenceMedical.Id,
                            Content = absenceMedical.Content,
                            ImagePath = absenceMedical.ImagePath
                        },
                        InformationMeetingChapter = new
                        {
                            MeetingChapterName = meetingChapter.Name,
                            Time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                            Loop = loopDescription,
                            DateEnd = meetingChapter.DateEnd,
                            Link = meetingChapter.Link,
                            Address = meetingChapter.Address
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
