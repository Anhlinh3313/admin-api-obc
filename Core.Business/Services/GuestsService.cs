using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Guests;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class GuestsService : BaseService, IGuestsService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public IAccountService _accountService { get; set; }
        public GuestsService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IAccountService accountService,
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
            _accountService = accountService;
        }

        public JsonResult GetListGuests(string keySearch, DateTime fromDate, DateTime toDate, int statusId,int chapterId, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListGuests>()
                    .ExecProcedure(Proc_GetListGuests.GetEntityProc(keySearch, fromDate, toDate, statusId,chapterId, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllStatusGuests()
        {
            try
            {
                var result = new List<StatusGuestsModel>()
                {
                    new StatusGuestsModel(){Id = 1, IsEnabled = true, Code = "Pending", Name = "Chờ xác nhận"},
                    new StatusGuestsModel(){Id = 2, IsEnabled = true, Code = "Presence", Name = "Có mặt"},
                    new StatusGuestsModel(){Id = 3, IsEnabled = true, Code = "Absent", Name = "Vắng mặt"}
                };
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateGuests(GuestsViewModelCreate model, int customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Guests.FullNameNotEmpty);
                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                    return JsonUtil.Error(ValidatorMessage.Guests.PhoneNumberNotEmpty);
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return JsonUtil.Error(ValidatorMessage.Guests.EmailNotEmpty);
                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Guests.AddressNotEmpty);

                model.Name = model.Name.Trim();
                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.Address = model.Address.Trim();

                Guests guests = new Guests()
                {
                    Address = model.Address,
                    CustomerId = customerId,
                    Email = model.Email,
                    MeetingChapterId = model.MeetingChapterId,
                    Name = model.Name,
                    Note = model.Note,
                    IsCheckin = null,
                    PhoneNumber = model.PhoneNumber,
                    StatusId = (int) EnumData.StatusFaceToFaceAndGuests.Pending,
                    EventId = null,
                    MeetingDate = null,
                    MeetingWhere = null,
                    IsGuests = true
                };

                _unitOfWork.RepositoryCRUD<Guests>().Insert(guests);
                await _unitOfWork.CommitAsync();

                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .GetSingleNotEnabled(x => x.Id == model.MeetingChapterId);
                string body = "";
                if (meetingChapter.Form.ToLower().Equals("online meeting"))
                {
                    body = $"<p style=\"color: black\">Dear {guests.Name}," + "<br/>" +
                           "<br/>" + "Anh/Chị được mời tham gia buổi Meeting Chapter " + meetingChapter.Name +
                           "<br/>" + "Buổi họp sẽ diễn ra lúc: " + meetingChapter.Time.ToString("HH:mm") + " " + meetingChapter.Time.ToString("dd/MM/yyyy") +
                           "<br/>" + "Link Meeting: " + meetingChapter.Link +
                            "<br/>" +
                            "<br/>" + "Rất hân hạnh được đón tiếp Anh/Chị." + "<br/><br/>" + "Trân trọng," + "<br/>" + "OBC.</p>";
                }
                else
                {
                    body = $"<p style=\"color: black\">Dear {guests.Name}," + "<br/>" +
                           "<br/>" + "Anh/Chị được mời tham gia buổi Meeting Chapter " + meetingChapter.Name +
                           "<br/>" + "Buổi họp sẽ diễn ra lúc: " + meetingChapter.Time.ToString("HH:mm") + " " + meetingChapter.Time.ToString("dd/MM/yyyy") +
                           "<br/>" + "Địa điểm: " + meetingChapter.Address +
                           "<br/>" +
                           "<br/>" + "Rất hân hạnh được đón tiếp Anh/Chị." + "<br/><br/>" + "Trân trọng," + "<br/><br/>" + "OBC.</p>";
                }

                _accountService.SendMailGuests(guests.Id, "Thư mời tham gia Meeting Chapter " + meetingChapter.Name,
                    body);
                var chapterName = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == meetingChapter.ChapterId)
                    .Name;
                return JsonUtil.Success(new
                {
                    Guests = new
                    {
                        Id = guests.Id,
                        FullName = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        Address = model.Address,
                        Note = model.Note
                    },
                    InformationMeetingChapter = new
                    {
                        ChapterName = chapterName,
                        Time = meetingChapter.Time.ToString("HH:mm") + ", " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                        Link = meetingChapter.Link,
                        Address = meetingChapter.Address
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CheckInGuests(int guestsId, int checkIn)
        {
            try
            {
                var guests = _unitOfWork.RepositoryR<Guests>().GetSingle(x => x.Id == guestsId);
                if (checkIn == 0)//từ chối
                {
                    guests.IsCheckin = false;
                    _unitOfWork.RepositoryCRUD<Guests>().Update(guests);
                    _unitOfWork.Commit();

                    var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                        .GetSingleNotEnabled(x => x.Id == guests.MeetingChapterId);
                    string body = $"<p style=\"color: black\">Dear {guests.Name}," + "<br/>" +
                                  "<br/>" + "Cảm ơn bạn đã quan tâm đến OBC - Kết nối doanh nghiệp Việt Nam." +
                                  "<br/>" + "Rất tiếc bạn đã không tham gia buổi Meeting Chapter " + meetingChapter.Name +
                                  "<br/>" + "Hi vọng sẽ gặp bạn trong một buổi Meeting kế tiếp." +
                                  "<br/>" +
                                  "<br/>" + "Trân trọng," + "<br/><br/>" + "OBC. </p>";


                    _accountService.SendMailGuests(guests.Id, "Xác nhận không tham gia Meeting Chapter " + meetingChapter.Name,
                        body);
                    
                }
                else // đồng ý
                {
                    guests.IsCheckin = true;
                    guests.DateCheckIn = DateTime.Now;
                    _unitOfWork.RepositoryCRUD<Guests>().Update(guests);
                    _unitOfWork.Commit();

                    var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                        .GetSingleNotEnabled(x => x.Id == guests.MeetingChapterId);
                    string body = $"<p style=\"color: black\">Dear {guests.Name}," + "<br/>" +
                                  "<br/>" + "Cảm ơn Anh/ Chị đã đến tham gia buổi Meeting Chapter " + meetingChapter.Name + " vào ngày " + DateTime.Now.ToString("dd/MM/yyyy") +
                                  "<br/>" + "Chúc Anh/ Chị có một buổi tham gia Meeting thật bổ ích." + "<br/>" +
                                  "<br/>" + "Trân trọng," + "<br/><br/>" + "OBC.</p>";


                    _accountService.SendMailGuests(guests.Id, "Xác nhận đã tham gia Meeting Chapter " + meetingChapter.Name,
                        body);
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailGuests(int guestsId)
        {
            try
            {
                var guests = _unitOfWork.RepositoryR<Guests>().GetSingle(x => x.Id == guestsId);
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .GetSingleNotEnabled(x => x.Id == guests.MeetingChapterId);
                var chapterName = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == meetingChapter.ChapterId)
                    .Name;

                return JsonUtil.Success(new
                {
                    Guests = new
                    {
                        Id = guests.Id,
                        FullName = guests.Name,
                        PhoneNumber = guests.PhoneNumber,
                        Email = guests.Email,
                        Address = guests.Address,
                        Note = guests.Note
                    },
                    InformationMeetingChapter = new
                    {
                        ChapterName = chapterName,
                        Time = meetingChapter.Time.ToString("HH:mm") + ", " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                        Link = meetingChapter.Link,
                        Address = meetingChapter.Address
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListGuestsWithMeetingChapterId(string keySearch, int meetingChapterId, int pageNum, int pageSize, int currentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch))
                    keySearch = keySearch.Trim();
                var guests = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                        (keySearch == null ||
                                         (x.Name.ToLower().Contains(keySearch.ToLower()) )) && x.IsCheckin == null &&
                                        x.MeetingChapterId == meetingChapterId )
                    .ToList();
                var total = guests.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var tmp = guests.Skip((pageNum - 1) * pageSize).Take(pageSize).OrderByDescending(x => x.Id).ToList();
                List<GuestsViewModel> guestsModel = new List<GuestsViewModel>();

                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == currentId);
                if (customer.Language.Equals("en"))
                {
                    foreach (var item in tmp)
                    {
                        var customerReceiver = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == item.CustomerId);
                        GuestsViewModel itemGuests = new GuestsViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Email = item.Email,
                            PhoneNumber = item.PhoneNumber,
                            Address = item.Address,
                            ReceiverName = customerReceiver.FullName
                        };
                        if (item.IsGuests == null || item.IsGuests == true)
                        {
                            itemGuests.Type = "Visitors";
                        }
                        else
                        {
                            itemGuests.Type = "Go instead";
                        }

                        guestsModel.Add(itemGuests);
                    }
                }
                else
                {
                    foreach (var item in tmp)
                    {
                        var customerReceiver = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == item.CustomerId);
                        GuestsViewModel itemGuests = new GuestsViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Email = item.Email,
                            PhoneNumber = item.PhoneNumber,
                            Address = item.Address,
                            ReceiverName = customerReceiver.FullName
                        };
                        if (item.IsGuests == null || item.IsGuests == true)
                        {
                            itemGuests.Type = "Khách mời";
                        }
                        else
                        {
                            itemGuests.Type = "Đi thay";
                        }

                        guestsModel.Add(itemGuests);
                    }
                }

                return JsonUtil.Success(guestsModel);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
        public async Task<JsonResult> CreateGoInstead(GuestsViewModelCreate model, int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var checkGoInstead = _unitOfWork.RepositoryR<Guests>().Any(x =>
                    x.CustomerId == customerId && x.IsGuests == false && x.MeetingChapterId == model.MeetingChapterId);
                if (checkGoInstead)
                {
                    if (customer.Language.Equals("en"))
                    {
                        return JsonUtil.Error("You have already declared the person who will replace the meeting, please do not report again");
                    }
                    else
                    {
                        return JsonUtil.Error("Bạn đã khai báo người đi thay buổi họp, vui lòng không khai báo lại");
                    }
                }

                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Guests.FullNameNotEmpty);
                if (string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                    return JsonUtil.Error(ValidatorMessage.Guests.PhoneNumberNotEmpty);
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email))
                    return JsonUtil.Error(ValidatorMessage.Guests.EmailNotEmpty);
                if (string.IsNullOrEmpty(model.Address) || string.IsNullOrWhiteSpace(model.Address))
                    return JsonUtil.Error(ValidatorMessage.Guests.AddressNotEmpty);

                model.Name = model.Name.Trim();
                model.PhoneNumber = model.PhoneNumber.Trim();
                model.Email = model.Email.Trim();
                model.Address = model.Address.Trim();

                Guests guests = new Guests()
                {
                    Address = model.Address,
                    CustomerId = customerId,
                    Email = model.Email,
                    MeetingChapterId = model.MeetingChapterId,
                    Name = model.Name,
                    Note = model.Note,
                    IsCheckin = null,
                    PhoneNumber = model.PhoneNumber,
                    StatusId = (int)EnumData.StatusFaceToFaceAndGuests.Pending,
                    EventId = null,
                    MeetingDate = null,
                    MeetingWhere = null,
                    IsGuests = false
                };

                _unitOfWork.RepositoryCRUD<Guests>().Insert(guests);
                await _unitOfWork.CommitAsync();

                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .GetSingleNotEnabled(x => x.Id == model.MeetingChapterId);
                string body = "";
                if (meetingChapter.Form.ToLower().Equals("online meeting"))
                {
                    body = $"<p style=\"color: black\">Dear {guests.Name}," + "<br/>" +
                           "<br/>" + "Anh/Chị được mời tham gia buổi Meeting Chapter " + meetingChapter.Name +
                           "<br/>" + "Buổi họp sẽ diễn ra lúc: " + meetingChapter.Time.ToString("HH:mm") + " " + meetingChapter.Time.ToString("dd/MM/yyyy") +
                           "<br/>" + "Link Meeting: " + meetingChapter.Link +
                            "<br/>" +
                            "<br/>" + "Rất hân hạnh được đón tiếp Anh/Chị." + "<br/><br/>" + "Trân trọng," + "<br/>" + "OBC.</p>";
                }
                else
                {
                    body = $"<p style=\"color: black\">Dear {guests.Name}," + "<br/>" +
                           "<br/>" + "Anh/Chị được mời tham gia buổi Meeting Chapter " + meetingChapter.Name +
                           "<br/>" + "Buổi họp sẽ diễn ra lúc: " + meetingChapter.Time.ToString("HH:mm") + " " + meetingChapter.Time.ToString("dd/MM/yyyy") +
                           "<br/>" + "Địa điểm: " + meetingChapter.Address +
                           "<br/>" +
                           "<br/>" + "Rất hân hạnh được đón tiếp Anh/Chị." + "<br/><br/>" + "Trân trọng," + "<br/><br/>" + "OBC.</p>";
                }

                _accountService.SendMailGuests(guests.Id, "Thư mời tham gia Meeting Chapter " + meetingChapter.Name,
                    body);
                var chapterName = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == meetingChapter.ChapterId)
                    .Name;
                return JsonUtil.Success(new
                {
                    GoInstead = new
                    {
                        Id = guests.Id,
                        FullName = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        Address = model.Address,
                        Note = model.Note
                    },
                    InformationMeetingChapter = new
                    {
                        ChapterName = chapterName,
                        Time = meetingChapter.Time.ToString("HH:mm") + ", " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                        Link = meetingChapter.Link,
                        Address = meetingChapter.Address
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailGoInstead(int goInsteadId)
        {
            try
            {
                var guests = _unitOfWork.RepositoryR<Guests>().GetSingle(x => x.Id == goInsteadId);
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .GetSingleNotEnabled(x => x.Id == guests.MeetingChapterId);
                var chapterName = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == meetingChapter.ChapterId)
                    .Name;

                return JsonUtil.Success(new
                {
                    GoInstead = new
                    {
                        Id = guests.Id,
                        FullName = guests.Name,
                        PhoneNumber = guests.PhoneNumber,
                        Email = guests.Email,
                        Address = guests.Address,
                        Note = guests.Note
                    },
                    InformationMeetingChapter = new
                    {
                        ChapterName = chapterName,
                        Time = meetingChapter.Time.ToString("HH:mm") + ", " + meetingChapter.Time.ToString("dd-MM-yyyy"),
                        Link = meetingChapter.Link,
                        Address = meetingChapter.Address
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
