using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.MeetingChapter;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Core.Infrastructure.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetingChapterController : BaseController
    {
        private readonly IMeetingChapterService _meetingChapterService;
        private readonly IEventService _eventService;

        public MeetingChapterController(
            IEventService eventService,
            IMeetingChapterService meetingChapterService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _meetingChapterService = meetingChapterService;
            _eventService = eventService;
        }

        [HttpPost("CreateMeetingChapter")]
        public async Task<IActionResult> CreateMeetingChapter([FromBody] MeetingChapterViewModel model)
        {
            JsonResult meetingChapter, generateQr;
            object success, value;
            int isSuccess;
            MeetingChapter link;
            string text;
            byte[] image;
            FileContentResult files;
            FileStreamResult streamResult;

            var currentUserId = GetCurrentUserId();
            switch (model.Loop)
            {
                case (int)EnumData.LoopMeetingChapter.EveryDay:
                    if (model.DateEnd != null && model.DateEnd < model.Time)
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.DateEndInCorrect);
                    if (model.DateEnd != null && model.DateEnd.GetValueOrDefault().AddDays(1) < model.Time.AddDays(2))
                    {
                        TimeSpan time = model.DateEnd.GetValueOrDefault().AddDays(1) - model.Time;
                        for (int i = 0; i < time.Days + 1; i++)
                        {
                            model.Time = model.Time.AddDays(i);
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0)
                            {
                                model.Time = model.Time;
                            }
                            else
                            {
                                model.Time = model.Time.AddDays(i - (i - 1));
                            }

                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    break;
                case (int)EnumData.LoopMeetingChapter.EveryWeek:
                    if (model.DateEnd != null && model.DateEnd.GetValueOrDefault().AddDays(1) < model.Time.AddDays(7 * 2))
                    {
                        TimeSpan time = model.DateEnd.GetValueOrDefault().AddDays(1) - model.Time;
                        for (int i = 0; i < (time.Days / 7) + 1; i++)
                        {
                            model.Time = model.Time.AddDays(7 * i);
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0)
                            {
                                model.Time = model.Time;
                            }
                            else
                            {
                                model.Time = model.Time.AddDays(7 * (i - (i - 1)));
                            }
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    break;
                case (int)EnumData.LoopMeetingChapter.TwoWeek:
                    if (model.DateEnd != null && model.DateEnd.GetValueOrDefault().AddDays(1) <= model.Time.AddDays(14 * 2))
                    {
                        TimeSpan time = model.DateEnd.GetValueOrDefault().AddDays(1) - model.Time;
                        for (int i = 0; i < (time.Days / 14) + 1; i++)
                        {
                            model.Time = model.Time.AddDays(14 * i);
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0)
                            {
                                model.Time = model.Time;
                            }
                            else
                            {
                                model.Time = model.Time.AddDays(14 * (i - (i - 1)));
                            }
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    break;
                case (int)EnumData.LoopMeetingChapter.EveryMonth:
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            model.Time = model.Time;
                        }
                        else
                        {
                            model.Time = model.Time.AddMonths(i - (i - 1));
                        }
                        if (model.DateEnd != null && model.DateEnd.GetValueOrDefault().AddDays(1) > model.Time)
                        {
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                        if (model.DateEnd == null)
                        {
                            meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                            success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return meetingChapter;
                            }
                            value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                            link = (MeetingChapter)value;
                            text = "MeetingChapterId: " + link.Id.ToString();

                            image = _eventService.GenerateByteArray(text);
                            files = File(image, "image/png");
                            streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                                files.ContentType);
                            streamResult.FileDownloadName = files.FileDownloadName;

                            generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                            success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                            isSuccess = (int)success;
                            if (isSuccess == 0)
                            {
                                return generateQr;
                            }
                        }
                    }
                    break;
                default:
                    meetingChapter = await _meetingChapterService.CreateMeetingChapter(model, currentUserId);
                    success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                    isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return meetingChapter;
                    }
                    value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
                    link = (MeetingChapter)value;
                    text = "MeetingChapterId: " + link.Id.ToString();

                    image = _eventService.GenerateByteArray(text);
                    files = File(image, "image/png");
                    streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                        files.ContentType);
                    streamResult.FileDownloadName = files.FileDownloadName;

                    generateQr = await _meetingChapterService.GenerateQrCodePath(streamResult, link.Id);
                    success = generateQr.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
                    isSuccess = (int)success;
                    if (isSuccess == 0)
                    {
                        return generateQr;
                    }
                    break;
            }

            return JsonUtil.Success(true);
        }

        [HttpGet("GetMeetingChapter")]
        public async Task<IActionResult> GetMeetingChapter()
        {
            List<ListMeetingChapterViewModel> result = new List<ListMeetingChapterViewModel>();

            var currentUserId = GetCurrentUserId();
            var language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentUserId).Language;
            var meetingChapter = await _meetingChapterService.GetListMeetingChapter(currentUserId);
            var success = meetingChapter.Value.GetType().GetProperty("isSuccess")?.GetValue(meetingChapter.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return meetingChapter;
            }
            var value = meetingChapter.Value.GetType().GetProperty("data")?.GetValue(meetingChapter.Value, null);
            var listMeetingChapter = (List<MeetingChapter>)value;

            if (listMeetingChapter.Count == 0) return JsonUtil.Success(listMeetingChapter);

            foreach (var item in listMeetingChapter)
            {
                var text = "MeetingChapterId: " + item.Id.ToString();

                var image = _eventService.GenerateByteArray(text);
                var files = File(image, "image/png");
                var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                    files.ContentType);
                streamResult.FileDownloadName = files.FileDownloadName;

                await _meetingChapterService.GenerateQrCodePath(streamResult, item.Id);
            }

            for (int i = 0; i < listMeetingChapter.Count; i++)
            {
                if (listMeetingChapter[i].Time.AddHours(1) > DateTime.Now)
                {
                    var time = listMeetingChapter[i].Time.DayOfWeek.GetHashCode();
                    var descriptionTime = Extensions.GetDescription((EnumData.DayOfWeek)time);
                    var descriptionTimeEnglish = Extensions.GetDescription((EnumData.DayOfWeekEnglish)time);
                    var action = ((EnumData.LoopMeetingChapter)listMeetingChapter[i].Loop).GetEnumDisplayName();
                    var description = Extensions.GetDescription((EnumData.LoopMeetingChapter)listMeetingChapter[i].Loop);
                    ListMeetingChapterViewModel item = new ListMeetingChapterViewModel();
                    if (language.Equals("en"))
                    {
                        item = new ListMeetingChapterViewModel()
                        {
                            Id = listMeetingChapter[i].Id,
                            IsEnabled = listMeetingChapter[i].IsEnabled,
                            Form = listMeetingChapter[i].Form,
                            DateEnd = listMeetingChapter[i].DateEnd,
                            Address = listMeetingChapter[i].Address,
                            Link = listMeetingChapter[i].Link,
                            Loop = description,
                            MeetingName = listMeetingChapter[i].Name,
                            QrCodePath = listMeetingChapter[i].QrCodePath,
                            SumGuests = _unitOfWork.RepositoryR<Guests>().Count(x => x.MeetingChapterId == listMeetingChapter[i].Id && (x.IsGuests == null || x.IsGuests == true)),
                            Time = listMeetingChapter[i].Time.ToString("HH:mm") + " | " + descriptionTimeEnglish + ", " + listMeetingChapter[i].Time.ToString("MM/dd/yyyy")
                        };
                    }
                    else
                    {
                        item = new ListMeetingChapterViewModel()
                        {
                            Id = listMeetingChapter[i].Id,
                            IsEnabled = listMeetingChapter[i].IsEnabled,
                            Form = listMeetingChapter[i].Form,
                            DateEnd = listMeetingChapter[i].DateEnd,
                            Address = listMeetingChapter[i].Address,
                            Link = listMeetingChapter[i].Link,
                            Loop = description,
                            MeetingName = listMeetingChapter[i].Name,
                            QrCodePath = listMeetingChapter[i].QrCodePath,
                            SumGuests = _unitOfWork.RepositoryR<Guests>().Count(x => x.MeetingChapterId == listMeetingChapter[i].Id && (x.IsGuests == null || x.IsGuests == true)),
                            Time = listMeetingChapter[i].Time.ToString("HH:mm") + " | " + descriptionTime + ", " + listMeetingChapter[i].Time.ToString("dd/MM/yyyy")
                        };
                    }
                    result.Add(item);
                    i = listMeetingChapter.Count;
                }
                else
                {
                    await _meetingChapterService.UpdateEnabledMeetingChapter(listMeetingChapter[i]);
                }
            }
            return JsonUtil.Success(result);
        }

        [HttpGet("GetAllMeetingChapterExpired")]
        public async Task<IActionResult> GetAllMeetingChapterExpired()
        {
            var listMeetingChapterId = await _meetingChapterService.GetAllMeetingChapterExpired();
            var success = listMeetingChapterId.Value.GetType().GetProperty("isSuccess")?.GetValue(listMeetingChapterId.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return listMeetingChapterId;
            }
            var value = listMeetingChapterId.Value.GetType().GetProperty("data")?.GetValue(listMeetingChapterId.Value, null);
            var listMeetingChapter = (List<int>)value;
            foreach (var item in listMeetingChapter)
            {
                var text = "MeetingChapterId: " + item.ToString();

                var image = _eventService.GenerateByteArray(text);
                var files = File(image, "image/png");
                var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                    files.ContentType);
                streamResult.FileDownloadName = files.FileDownloadName;

                await _meetingChapterService.GenerateQrCodePath(streamResult, item);
            }

            return JsonUtil.Success(true);
        }

        [HttpGet("GetDetailMeetingChapter")]
        public IActionResult GetDetailMeetingChapter(int meetingChapterId, int? isEdit)
        {
            var currentUserId = GetCurrentUserId();
            return _meetingChapterService.GetDetailMeetingChapter(meetingChapterId, currentUserId, isEdit.GetValueOrDefault(0));
        }

        [HttpGet("GetMeetingChapterWithChapterId")]
        public IActionResult GetMeetingChapterWithChapterId(int chapterId)
        {
            return _meetingChapterService.GetMeetingChapterWithChapterId(chapterId);
        }

        [HttpGet("MeetingChapterCheckInCustomer")]
        public IActionResult MeetingChapterCheckInCustomer(int meetingChapterId)
        {
            var currentUserId = GetCurrentUserId();
            return _meetingChapterService.MeetingChapterCheckInCustomer(meetingChapterId, currentUserId);
        }

        [HttpGet("GetLoopMeetingChapter")]
        public IActionResult GetLoopMeetingChapter()
        {
            var currentUserId = GetCurrentUserId();
            return _meetingChapterService.GetLoopMeetingChapter(currentUserId);
        }

        [HttpGet("GetListMeetingChapterInGuests")]
        public IActionResult GetListMeetingChapterInGuests()
        {
            var currentUserId = GetCurrentUserId();
            return  _meetingChapterService.GetListMeetingChapterInGuests(currentUserId);
        }
        [HttpGet("EditMeetingChapter")]
        public IActionResult EditMeetingChapter(int meetingChapterId, string form, string link, string address)
        {
            return _meetingChapterService.EditMeetingChapter(meetingChapterId, form, link, address);
        }
        [HttpGet("EndMeetingChapter")]
        public IActionResult EndMeetingChapter(int meetingChapterId)
        {
            return _meetingChapterService.EndMeetingChapter(meetingChapterId);
        }
    }
}
