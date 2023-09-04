using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Notify;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Core.Business.Services
{
    public class NotifyService : BaseService, INotifyService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        private static readonly HttpClient client = new HttpClient();
        public NotifyService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult CreateNotify(int customerId, string content, int notifyTypeId, int actionTypeId, int? customerCancelId, string reasonCancel)
        {
            try
            {
                Notify notify = new Notify()
                {
                    Id = 0,
                    ActionTypeId = actionTypeId,
                    CustomerId = customerId,
                    IsSeen = false,
                    NotifyTypeId = notifyTypeId,
                    CustomerCancelId = customerCancelId,
                    ReasonCancel = reasonCancel
                    
                };

                _unitOfWork.RepositoryCRUD<Notify>().Insert(notify);
                _unitOfWork.Commit();
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var seen = _unitOfWork.RepositoryR<Notify>().Count(x => x.CustomerId == customerId && x.IsSeen == false);
                if (customer.Language != null)
                {
                    if (customer.Language.Equals("en"))
                    {
                        dynamic body = new
                        {
                            to = customer.ExpoPushToken,
                            title = "Notification",
                            body = content,
                            sound = "default"
                        };
                        var myContent = JsonConvert.SerializeObject(body);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                    }
                    else
                    {
                        dynamic body = new
                        {
                            to = customer.ExpoPushToken,
                            title = "Thông báo",
                            body = content,
                            sound = "default"
                        };
                        var myContent = JsonConvert.SerializeObject(body);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                    }
                }
                else
                {
                    dynamic body = new
                    {
                        to = customer.ExpoPushToken,
                        title = "Thông báo",
                        body = content,
                        sound = "default"
                    };
                    var myContent = JsonConvert.SerializeObject(body);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                }
                

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> SeenNotify(int? notifyId, int customerId)
        {
            try
            {
                if (notifyId == null) notifyId = 0;
                if (notifyId == 0) // Nhấn seen all
                {
                    var notify = _unitOfWork.RepositoryR<Notify>().FindBy(x => x.CustomerId == customerId).ToList();
                    foreach (var item in notify)
                    {
                        item.IsSeen = true;

                        _unitOfWork.RepositoryCRUD<Notify>().Update(item);
                        await _unitOfWork.CommitAsync();
                    }
                }
                else // seen từng cái
                {
                    var notify = _unitOfWork.RepositoryR<Notify>().GetSingle(x => x.Id == notifyId);
                    notify.IsSeen = true;
                    _unitOfWork.RepositoryCRUD<Notify>().Update(notify);
                    await _unitOfWork.CommitAsync();
                }

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetListNotify(int customerId, int pageNum, int pageSize)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListNotify>()
                    .ExecProcedure(Proc_GetListNotify.GetEntityProc(customerId, pageNum, pageSize)).ToList();
                if (data.Count <= 0)
                    return JsonUtil.Success(data, "Success", 0);
                List<NotifyViewModel> result = new List<NotifyViewModel>();
                foreach (var item in data)
                {
                    NotifyViewModel model = new NotifyViewModel()
                    {
                        RowNum = item.RowNum,
                        NotifyId = item.NotifyId,
                        IsSeen = item.IsSeen,
                        OpportunityId = item.OpportunityId,
                        ThanksId = item.ThanksId,
                        FaceToFaceId = item.FaceToFaceId,
                        CourseId = item.CourseId,
                        EventId = item.EventId,
                        CreatedWhen = item.CreatedWhen,
                        PopUp = null,
                        VideoId = item.VideoId,
                        Total = item.Total,
                        EventCode = item.EventCode,
                        CourseCode = item.CourseCode,
                        VideoCode = item.VideoCode,
                        AvatarPath = item.AvatarPath,
                        ProfessionId = item.ProfessionId,
                        FieldOperationsId = item.FieldOperationsId
                    };
                    if (!string.IsNullOrEmpty(item.ReasonCancel))
                    {
                        PopUp popUp = new PopUp()
                        {
                            CustomerNameCancel = item.CustomerNameCancel,
                            ReasonCancel = item.ReasonCancel,
                            DateCancel = item.DateCancel
                        };
                        model.PopUp = popUp;
                    }

                    string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                    if (string.IsNullOrEmpty(language) || language.Equals("vi"))
                    {
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Customer &&
                        !string.IsNullOrEmpty(item.ChapterName) && string.IsNullOrEmpty(item.ReasonCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.AcceptChapter, item.ChapterName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.AcceptChapter;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ChapterName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Customer &&
                            !string.IsNullOrEmpty(item.ChapterName) && !string.IsNullOrEmpty(item.ReasonCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelChapter, item.ChapterName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelChapter;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ChapterName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Customer &&
                            string.IsNullOrEmpty(item.ChapterName))
                        {
                            model.Content = ValidatorMessage.ContentNotify.AcceptPremium;
                            model.NotifyTypeId = (int)EnumData.TypeNotify.AcceptPremium;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = null;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Thanks)
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.ThanksFor, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.ThanksFor;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Opportunity)
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.OpportunityFor, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.OpportunityFor;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.FaceToFace && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.FaceToFaceFor, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.FaceToFaceFor;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.FaceToFaceSuccess)
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.FaceToFaceSuccess, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.FaceToFaceSuccess;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.FaceToFace && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.FaceToFaceCancel, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.FaceToFaceCancel;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Event && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.RegisterEvent, item.EventName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.RegisterEvent;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.EventName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Event && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelRegisterEvent, item.EventName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelRegisterEvent;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.EventName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Course && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.RegisterCourse, item.CourseName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.RegisterCourse;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.CourseName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Course && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelRegisterCourse, item.CourseName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelRegisterCourse;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.CourseName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Video && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.RegisterVideo, item.VideoName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.RegisterVideo;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.VideoName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Video && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelRegisterVideo, item.VideoName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelRegisterVideo;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.VideoName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeProfession && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.ChangeProfession, item.ProfessionName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.ChangeProfession;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ProfessionName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeProfession && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelChangeProfession);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelChangeProfession;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = "";
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeFieldOperations && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.ChangeFieldOperations, item.FieldOperationsName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.ChangeFieldOperations;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.FieldOperationsName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeFieldOperations && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelChangeFieldOperations);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelChangeFieldOperations;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = "";
                        }
                    }
                    else
                    {
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Customer &&
                        !string.IsNullOrEmpty(item.ChapterName) && string.IsNullOrEmpty(item.ReasonCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.AcceptChapterEnglish, item.ChapterName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.AcceptChapter;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ChapterName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Customer &&
                            !string.IsNullOrEmpty(item.ChapterName) && !string.IsNullOrEmpty(item.ReasonCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelChapterEnglish, item.ChapterName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelChapter;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ChapterName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Customer &&
                            string.IsNullOrEmpty(item.ChapterName))
                        {
                            model.Content = ValidatorMessage.ContentNotify.AcceptPremiumEnglish;
                            model.NotifyTypeId = (int)EnumData.TypeNotify.AcceptPremium;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = null;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Thanks)
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.ThanksForEnglish, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.ThanksFor;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Opportunity)
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.OpportunityForEnglish, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.OpportunityFor;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.FaceToFace && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.FaceToFaceForEnglish, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.FaceToFaceFor;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.FaceToFaceSuccess)
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.FaceToFaceSuccessEnglish, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.FaceToFaceSuccess;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.FaceToFace && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.FaceToFaceCancelEnglish, item.ReceiverName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.FaceToFaceCancel;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ReceiverName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Event && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.RegisterEventEnglish, item.EventName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.RegisterEvent;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.EventName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Event && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelRegisterEventEnglish, item.EventName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelRegisterEvent;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.EventName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Course && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.RegisterCourseEnglish, item.CourseName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.RegisterCourse;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.CourseName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Course && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelRegisterCourseEnglish, item.CourseName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelRegisterCourse;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.CourseName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Video && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.RegisterVideoEnglish, item.VideoName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.RegisterVideo;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.VideoName;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.Video && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelRegisterVideoEnglish, item.VideoName);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelRegisterVideo;
                            var action = ((EnumData.TypeNotify)model.NotifyTypeId).GetEnumDisplayName();
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.VideoName;
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeProfession && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.ChangeProfessionEnglish, item.ProfessionCode);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.ChangeProfession;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.ProfessionCode;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeProfession && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelChangeProfessionEnglish);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelChangeProfession;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = "";
                        }

                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeFieldOperations && string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.ChangeFieldOperationsEnglish, item.FieldOperationsCode);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.ChangeFieldOperations;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = item.FieldOperationsCode;
                        }
                        if (item.NotifyTypeId == (int)EnumData.NotifyType.ChangeFieldOperations && !string.IsNullOrEmpty(item.CustomerNameCancel))
                        {
                            model.Content = string.Format(ValidatorMessage.ContentNotify.CancelChangeFieldOperationsEnglish);
                            model.NotifyTypeId = (int)EnumData.TypeNotify.CancelChangeFieldOperations;
                            model.NotifyTypeName = Extensions.GetDescription((EnumData.TypeNotify)model.NotifyTypeId);
                            model.Keyword = "";
                        }
                    }

                    

                    result.Add(model);
                }

                return JsonUtil.Success(result, "Success", result.FirstOrDefault().Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult SumUnSeenNotify(int customerId)
        {
            try
            {
                var seen = _unitOfWork.RepositoryR<Notify>().Count(x => x.CustomerId == customerId && x.IsSeen == false);

                return JsonUtil.Success(seen);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CreateNotifyWhenDeActiveCustomer(int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;
                if (customer.Language != null)
                {
                    if (customer.Language.Equals("en"))
                    {
                        dynamic body = new
                        {
                            to = customer.ExpoPushToken,
                            title = "Account notification",
                            body = $"Your account has been deactivated, please contact the hotline for support: {hotline}"
                        };
                        var myContent = JsonConvert.SerializeObject(body);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                    }
                    else
                    {
                        dynamic body = new
                        {
                            to = customer.ExpoPushToken,
                            title = "Thông báo tài khoản",
                            body = $"Tài khoản của bạn đã bị hủy kích hoạt, vui lòng liên hệ hotline để hỗ trợ: {hotline}"
                        };
                        var myContent = JsonConvert.SerializeObject(body);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                    }
                }
                

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult CreateNotifyWhenCustomerLogInDeviceDifferent(int customerId)
        {
            try
            {
                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                var hotline = _unitOfWork.RepositoryR<Introduce>().GetAll().FirstOrDefault().PhoneNumber;
                if (customer.Language != null)
                {
                    if (customer.Language.Equals("en"))
                    {
                        dynamic body = new
                        {
                            to = customer.ExpoPushToken,
                            title = "Signed in notification",
                            body = $"The account is logged in on another machine. Please check again or contact the hotline for support: {hotline}"
                        };
                        var myContent = JsonConvert.SerializeObject(body);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                    }
                    else
                    {
                        dynamic body = new
                        {
                            to = customer.ExpoPushToken,
                            title = "Thông báo đã đăng nhập",
                            body = $"Tài khoản đã đăng nhập ở máy khác. Vui lòng kiểm tra lại hoặc liên hệ hotline để hỗ trợ: {hotline}"
                        };
                        var myContent = JsonConvert.SerializeObject(body);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.PostAsync("https://exp.host/--/api/v2/push/send", byteContent);
                    }
                }


                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
