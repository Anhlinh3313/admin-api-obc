using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Guests;
using Core.Business.ViewModels.MeetingChapter;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class MeetingChapterService : BaseService, IMeetingChapterService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public MeetingChapterService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> CreateMeetingChapter(MeetingChapterViewModel model, int customerId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    if (string.IsNullOrEmpty(model.MeetingName) || string.IsNullOrWhiteSpace(model.MeetingName))
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.NameNotEmpty);
                    if (model.DateEnd != null && model.DateEnd.GetValueOrDefault().AddDays(1) < model.Time)
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.DateEndInCorrect);
                    if (model.Time <= DateTime.Now)
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.TimeInCorrect);

                    model.MeetingName = model.MeetingName.Trim();
                    if (model.Form.ToLower().Equals("online meeting"))
                    {
                        if (string.IsNullOrEmpty(model.Link) || string.IsNullOrWhiteSpace(model.Link))
                            return JsonUtil.Error(ValidatorMessage.MeetingChapter.LinkNotEmpty);

                        model.Link = model.Link.Trim();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                            return JsonUtil.Error(ValidatorMessage.MeetingChapter.AddressNotEmpty);

                        model.Address = model.Address.Trim();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.MeetingName) || string.IsNullOrWhiteSpace(model.MeetingName))
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.NameNotEmptyEnglish);
                    if (model.DateEnd != null && model.DateEnd < model.Time)
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.DateEndInCorrectEnglish);
                    if (model.Time <= DateTime.Now)
                        return JsonUtil.Error(ValidatorMessage.MeetingChapter.TimeInCorrectEnglish);

                    model.MeetingName = model.MeetingName.Trim();
                    if (model.Form.ToLower().Equals("online meeting"))
                    {
                        if (string.IsNullOrEmpty(model.Link) || string.IsNullOrWhiteSpace(model.Link))
                            return JsonUtil.Error(ValidatorMessage.MeetingChapter.LinkNotEmptyEnglish);

                        model.Link = model.Link.Trim();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                            return JsonUtil.Error(ValidatorMessage.MeetingChapter.AddressNotEmptyEnglish);

                        model.Address = model.Address.Trim();
                    }
                }
                

                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId.GetValueOrDefault();
                MeetingChapter meetingChapter = new MeetingChapter()
                {
                    Address = model.Address,
                    DateEnd = model.DateEnd,
                    Form = model.Form,
                    Link = model.Link,
                    Loop = model.Loop,
                    Name = model.MeetingName,
                    Time = model.Time,
                    ChapterId = chapterId
                };

                _unitOfWork.RepositoryCRUD<MeetingChapter>().Insert(meetingChapter);
                await _unitOfWork.CommitAsync();

                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .FindBy(x => x.ParticipatingChapterId == chapterId).ToList();
                foreach (var item in business)
                {
                    MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                    {
                        MeetingChapterId = meetingChapter.Id,
                        CustomerId = item.CustomerId.GetValueOrDefault(),
                        IsEnabled = false
                    };

                    _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabled(meetingChapterCheckIn);
                    await _unitOfWork.CommitAsync();
                }
                return JsonUtil.Success(meetingChapter);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateEnabledMeetingChapter(MeetingChapter meetingChapter)
        {
            try
            {
                meetingChapter.IsEnabled = false;
                _unitOfWork.RepositoryCRUD<MeetingChapter>().Update(meetingChapter);
                await _unitOfWork.CommitAsync();

                var guests = _unitOfWork.RepositoryR<Guests>().FindBy(x => x.MeetingChapterId == meetingChapter.Id && x.IsCheckin == null && (x.IsGuests == null || x.IsGuests == true))
                    .ToList();
                foreach (var item in guests)
                {
                    item.IsEnabled = false;
                    _unitOfWork.RepositoryCRUD<Guests>().Update(item);
                    await _unitOfWork.CommitAsync();
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GenerateQrCodePath(FileStreamResult streamResult, int meetingChapterId)
        {
            try
            {
                var detailMeetingChapter = await _unitOfWork.RepositoryR<MeetingChapter>().GetSingleAsync(x => x.Id == meetingChapterId);
                var QrPath = GenerateQrCode(streamResult, "QrCodeMeetingChapter_" + meetingChapterId,
                    "QrCodeMeetingChapter");

                detailMeetingChapter.QrCodePath = QrPath;
                _unitOfWork.RepositoryCRUD<MeetingChapter>().Update(detailMeetingChapter);
                _unitOfWork.Commit();

                return JsonUtil.Success(detailMeetingChapter);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetListMeetingChapter(int customerId)
        {
            try
            {
                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId.GetValueOrDefault();
                var meetingChapter = await _unitOfWork.RepositoryR<MeetingChapter>().FindBy(x => x.ChapterId == chapterId)
                    .ToListAsync();
                return JsonUtil.Success(meetingChapter);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllMeetingChapterExpired()
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetAllMeetingChapterExpired>()
                    .ExecProcedure(Proc_GetAllMeetingChapterExpired.GetEntityProc()).ToList();
                MeetingChapter meetingChapter = new MeetingChapter();
                List<int> result = new List<int>();
                foreach (var item in data)
                {
                    switch (item.Loop)
                    {
                        case (int)EnumData.LoopMeetingChapter.EveryDay:
                            if (item.DateEnd.GetValueOrDefault().AddDays(1) < item.Time.AddDays(3))
                            {
                                TimeSpan time = item.DateEnd.GetValueOrDefault().AddDays(1) - item.Time;
                                for (int i = 1; i < time.Days+1; i++)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddDays(i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                            }
                            else
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddDays(i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                            }
                            break;
                        case (int)EnumData.LoopMeetingChapter.EveryWeek:
                            if (item.DateEnd.GetValueOrDefault().AddDays(1) < item.Time.AddDays(7*3))
                            {
                                TimeSpan time = item.DateEnd.GetValueOrDefault().AddDays(1) - item.Time;
                                for (int i = 1; i < (time.Days / 7)+1; i++)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddDays(7*i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                            }
                            else
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddDays(7*i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                            }
                            break;
                        case (int)EnumData.LoopMeetingChapter.TwoWeek:
                            if (item.DateEnd.GetValueOrDefault().AddDays(1) < item.Time.AddDays(14 * 3))
                            {
                                TimeSpan time = item.DateEnd.GetValueOrDefault().AddDays(1) - item.Time;
                                for (int i = 1; i < (time.Days / 14)+1; i++)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddDays(14 * i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                            }
                            else
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddDays(14 * i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                            }
                            break;
                        case (int)EnumData.LoopMeetingChapter.EveryMonth:
                            for (int i = 1; i < 4; i++)
                            {
                                if (item.DateEnd != null && item.DateEnd.GetValueOrDefault().AddDays(1) > item.Time)
                                {
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddMonths(i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);
                                }
                                if(item.DateEnd == null)
                                {
                                    
                                    meetingChapter = new MeetingChapter()
                                    {
                                        Address = item.Address,
                                        ChapterId = item.ChapterId,
                                        DateEnd = item.DateEnd,
                                        Form = item.Form,
                                        Link = item.Link,
                                        Loop = item.Loop,
                                        Name = item.Name,
                                        Time = item.Time.AddMonths(i)
                                    };
                                    _unitOfWork.RepositoryCRUD<MeetingChapter>().InsertNotCurrentUserId(meetingChapter);
                                    _unitOfWork.Commit();

                                    var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                                        .FindBy(x => x.ParticipatingChapterId == item.ChapterId).ToList();
                                    foreach (var itemBusiness in business)
                                    {
                                        MeetingChapterCheckIn meetingChapterCheckIn = new MeetingChapterCheckIn()
                                        {
                                            MeetingChapterId = meetingChapter.Id,
                                            CustomerId = itemBusiness.CustomerId.GetValueOrDefault(),
                                            IsEnabled = false
                                        };

                                        _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().InsertNotSetEnabledNotCurrentUserId(meetingChapterCheckIn);
                                        await _unitOfWork.CommitAsync();
                                    }

                                    result.Add(meetingChapter.Id);

                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailMeetingChapter(int meetingChapterId, int currentUserId, int isEdit)
        {
            try
            {
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().GetSingle(x => x.Id == meetingChapterId);

                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentUserId).Language;
                if (isEdit == 0)
                {
                    if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                    {
                        var description = Extensions.GetDescription((EnumData.LoopMeetingChapter)meetingChapter.Loop);
                        if (meetingChapter.DateEnd == null)
                        {
                            return JsonUtil.Success(new
                            {
                                Name = meetingChapter.Name,
                                time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                                Loop = description,
                                DateEnd = meetingChapter.DateEnd,
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link
                            });
                        }
                        else
                        {
                            return JsonUtil.Success(new
                            {
                                Name = meetingChapter.Name,
                                time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                                Loop = description,
                                DateEnd = meetingChapter.DateEnd.GetValueOrDefault().ToString("dd-MM-yyyy"),
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link
                            });
                        }
                    }
                    else
                    {
                        var description = Extensions.GetDescription((EnumData.LoopMeetingChapterEnglish)meetingChapter.Loop);
                        if (meetingChapter.DateEnd == null)
                        {
                            return JsonUtil.Success(new
                            {
                                Name = meetingChapter.Name,
                                time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("MM-dd-yyyy"),
                                Loop = description,
                                DateEnd = meetingChapter.DateEnd,
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link
                            });
                        }
                        else
                        {
                            return JsonUtil.Success(new
                            {
                                Name = meetingChapter.Name,
                                time = meetingChapter.Time.ToString("HH:mm") + " | " + meetingChapter.Time.ToString("MM-dd-yyyy"),
                                Loop = description,
                                DateEnd = meetingChapter.DateEnd.GetValueOrDefault().ToString("MM-dd-yyyy"),
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link
                            });
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                    {
                        var description = Extensions.GetDescription((EnumData.LoopMeetingChapter)meetingChapter.Loop);
                        if (meetingChapter.DateEnd == null)
                        {
                            return JsonUtil.Success(new
                            {
                                MeetingName = meetingChapter.Name,
                                time = meetingChapter.Time,
                                Loop = new
                                {
                                    id = meetingChapter.Loop,
                                    name = description
                                },
                                DateEnd = meetingChapter.DateEnd,
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link,
                                Form = meetingChapter.Form
                            });
                        }
                        else
                        {
                            return JsonUtil.Success(new
                            {
                                MeetingName = meetingChapter.Name,
                                time = meetingChapter.Time,
                                Loop = new
                                {
                                    id = meetingChapter.Loop,
                                    name = description
                                },
                                DateEnd = meetingChapter.DateEnd,
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link,
                                Form = meetingChapter.Form
                            });
                        }
                    }
                    else
                    {
                        var description = Extensions.GetDescription((EnumData.LoopMeetingChapterEnglish)meetingChapter.Loop);
                        if (meetingChapter.DateEnd == null)
                        {
                            return JsonUtil.Success(new
                            {
                                MeetingName = meetingChapter.Name,
                                time = meetingChapter.Time,
                                Loop = new
                                {
                                    id = meetingChapter.Loop,
                                    name = description
                                },
                                DateEnd = meetingChapter.DateEnd,
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link,
                                Form = meetingChapter.Form
                            });
                        }
                        else
                        {
                            return JsonUtil.Success(new
                            {
                                MeetingName = meetingChapter.Name,
                                time = meetingChapter.Time,
                                Loop = new
                                {
                                    id = meetingChapter.Loop,
                                    name = description
                                },
                                DateEnd = meetingChapter.DateEnd,
                                Address = meetingChapter.Address,
                                Link = meetingChapter.Link,
                                Form = meetingChapter.Form
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetMeetingChapterWithChapterId(int chapterId)
        {
            try
            {
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().FindBy(x => x.ChapterId == chapterId);
                return JsonUtil.Success(meetingChapter);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult MeetingChapterCheckInCustomer(int meetingChapterId, int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var language = customer.Language;
                var checkIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>().Any(x =>
                    x.MeetingChapterId == meetingChapterId && x.CustomerId == customerId);
                if (checkIn)
                {
                    if (language.Equals("en"))
                    {
                        return JsonUtil.Error("You have already checked in to this meeting, please do not check in again");
                    }
                    else
                    {
                        return JsonUtil.Error("Bạn đã check in buổi họp này, vui lòng không check in lại");
                    }
                }

                var business = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId);
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().GetSingle(x => x.Id == meetingChapterId);
                if (meetingChapter == null)
                {
                    if (language.Equals("en"))
                    {
                        return JsonUtil.Error("The meeting has ended.");
                    }
                    else
                    {
                        return JsonUtil.Error("Buổi họp đã kết thúc.");
                    }
                }
                var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == meetingChapter.ChapterId);

                if (customer.StatusId != (int)EnumData.CustomerStatusEnum.Active)
                {
                    if (language.Equals("en"))
                    {
                        return JsonUtil.Error("You are not yet a member of the chapter " + chapter.Name +
                                              " so you can't join the meeting " + meetingChapter.Name);
                    }
                    else
                    {
                        return JsonUtil.Error("Bạn chưa là thành viên của chapter " + chapter.Name +
                                              " nên không thể tham gia buổi họp " + meetingChapter.Name);
                    }
                }

                if (business.ParticipatingChapterId == null ||
                    business.ParticipatingChapterId != meetingChapter.ChapterId)
                {
                    if (language.Equals("en"))
                    {
                        return JsonUtil.Error("You are not in chapter " + chapter.Name +
                                              " so you can't join the meeting " + meetingChapter.Name);
                    }
                    else
                    {
                        return JsonUtil.Error("Bạn không thuộc chapter " + chapter.Name +
                                              " nên không thể tham gia buổi họp " + meetingChapter.Name);
                    }
                }

                var meetingChapterCheckIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>()
                    .GetSingleNotEnabled(x => x.MeetingChapterId == meetingChapterId && x.CustomerId == customerId);
                meetingChapterCheckIn.IsEnabled = true;
                _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().Update(meetingChapterCheckIn);
                _unitOfWork.Commit();

                if (language.Equals("en"))
                {
                    return JsonUtil.Success(true, "You have successfully checked in");
                }
                else
                {
                    return JsonUtil.Success(true, "Bạn đã check in thành công");
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetLoopMeetingChapter(int currentUserId)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentUserId).Language;
                if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                {
                    var result = new List<LoopMeetingChapterViewModel>()
                    {
                        new LoopMeetingChapterViewModel(){Id = 1, Name = "Không lặp lại"},
                        new LoopMeetingChapterViewModel(){Id = 2,Name = "Hàng ngày"},
                        new LoopMeetingChapterViewModel(){Id = 3,Name = "Hàng tuần"},
                        new LoopMeetingChapterViewModel(){Id = 4,Name = "2 tuần"},
                        new LoopMeetingChapterViewModel(){Id = 5,Name = "Hàng tháng"}
                    };
                    return JsonUtil.Success(result);
                }
                else
                {
                    var result = new List<LoopMeetingChapterViewModel>()
                    {
                        new LoopMeetingChapterViewModel(){Id = 1, Name = "Do not repeat"},
                        new LoopMeetingChapterViewModel(){Id = 2,Name = "Daily"},
                        new LoopMeetingChapterViewModel(){Id = 3,Name = "Weekly"},
                        new LoopMeetingChapterViewModel(){Id = 4,Name = "2 week"},
                        new LoopMeetingChapterViewModel(){Id = 5,Name = "Monthly"}
                    };
                    return JsonUtil.Success(result);
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListMeetingChapterInGuests(int customerId)
        {
            try
            {
                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId;
                MeetingChapterViewModelInGuests result = new MeetingChapterViewModelInGuests();
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().FindBy(x => x.ChapterId == chapterId)
                    .ToList();
                
                result.ChapterName = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId).Name;
                if (meetingChapter.Count <= 0)
                {
                    result.Link = "";
                    result.Address = "";
                    List<TimeModel> time = new List<TimeModel>();
                    result.Time = time;
                }
                else
                {
                    result.Link = meetingChapter[0].Link;
                    result.Address = meetingChapter[0].Address;
                    List<TimeModel> time = new List<TimeModel>();
                    for (int i = 0; i < meetingChapter.Count; i++)
                    {
                        TimeModel timeItem = new TimeModel()
                        {
                            Name = meetingChapter[i].Time.ToString("HH:mm") + ", " + meetingChapter[i].Time.ToString("dd-MM-yyyy"),
                            Id = i + 1,
                            MeetingChapterId = meetingChapter[i].Id
                        };
                        time.Add(timeItem);
                    }

                    result.Time = time;
                }
               
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult EditMeetingChapter(int meetingChapterId, string form, string link, string address)
        {
            try
            {
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().GetSingle(x => x.Id == meetingChapterId);
                if (form.ToLower().Equals("offline meeting"))
                {
                    link = "";
                }
                else
                {
                    address = "";
                }
                meetingChapter.Form = form;
                meetingChapter.Link = link;
                meetingChapter.Address = address;

                _unitOfWork.RepositoryCRUD<MeetingChapter>().Update(meetingChapter);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult EndMeetingChapter(int meetingChapterId)
        {
            try
            {
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().GetSingle(x => x.Id == meetingChapterId);
                TimeSpan time = meetingChapter.Time.AddHours(1) - DateTime.Now;
                if (time.TotalHours < 0)
                {
                    var guests = _unitOfWork.RepositoryR<Guests>().FindBy(x => x.MeetingChapterId == meetingChapterId && (x.IsGuests == null || x.IsGuests == true)).ToList();
                    foreach (var itemGuests in guests)
                    {
                        itemGuests.IsEnabled = false;
                        _unitOfWork.RepositoryCRUD<Guests>().Update(itemGuests);
                        _unitOfWork.Commit();
                    }

                    meetingChapter.IsEnabled = false;
                    _unitOfWork.RepositoryCRUD<MeetingChapter>().Update(meetingChapter);
                    _unitOfWork.Commit();
                }

                if (time.TotalHours > 1)
                {
                    var guests = _unitOfWork.RepositoryR<Guests>().FindBy(x => x.MeetingChapterId == meetingChapterId && (x.IsGuests == null || x.IsGuests == true)).ToList();
                    foreach (var itemGuests in guests)
                    {
                        itemGuests.IsEnabled = false;
                        _unitOfWork.RepositoryCRUD<Guests>().Update(itemGuests);
                        _unitOfWork.Commit();
                    }

                    var checkIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>()
                        .Any(x => x.MeetingChapterId == meetingChapterId);
                    if (checkIn == false)
                    {
                        var customerCheckIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>()
                            .FindByNotEnabled(x => x.MeetingChapterId == meetingChapterId).ToList();
                        if (customerCheckIn.Count > 0)
                        {
                            foreach (var itemCheckIn in customerCheckIn)
                            {
                                _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().DeleteEmpty(itemCheckIn);
                                _unitOfWork.Commit();
                            }
                        }
                    }

                    meetingChapter.IsEnabled = false;
                    _unitOfWork.RepositoryCRUD<MeetingChapter>().Update(meetingChapter);
                    _unitOfWork.Commit();
                }
                
                var listMeetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .FindBy(x => x.ChapterId == meetingChapter.ChapterId).ToList();
                foreach (var item in listMeetingChapter)
                {
                    if (item.Id != meetingChapterId)
                    {
                        var guests = _unitOfWork.RepositoryR<Guests>().FindBy(x => x.MeetingChapterId == item.Id && (x.IsGuests == null || x.IsGuests == true));
                        foreach (var itemGuests in guests)
                        {
                            itemGuests.IsEnabled = false;
                            _unitOfWork.RepositoryCRUD<Guests>().Update(itemGuests);
                            _unitOfWork.Commit();
                        }

                        var customerCheckIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>()
                            .FindByNotEnabled(x => x.MeetingChapterId == item.Id).ToList();
                        if (customerCheckIn.Count > 0)
                        {
                            foreach (var itemCheckIn in customerCheckIn)
                            {
                                _unitOfWork.RepositoryCRUD<MeetingChapterCheckIn>().DeleteEmpty(itemCheckIn);
                                _unitOfWork.Commit();
                            }
                        }

                        item.IsEnabled = false;
                        _unitOfWork.RepositoryCRUD<MeetingChapter>().Update(item);
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

        public string GenerateQrCode(FileStreamResult streamResult, string fileName, string folderName)
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
