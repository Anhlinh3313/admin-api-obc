using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Kpi;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class KpiService : BaseService, IKpiService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public KpiService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }


        public JsonResult GetKpiMobile(int customerId, int monthFrom, int yearFrom, int monthTo, int yearTo)
        {
            try
            {
                string language = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId).Language;
                if (language.ToLower().Equals("vi"))
                {
                    var dataError = new List<Proc_GetKPIMobile>()
                    {
                        new Proc_GetKPIMobile(){Id = 1,ActionName = "Lời cảm ơn - Trao đi", Value = 0},
                        new Proc_GetKPIMobile(){Id = 2,ActionName = "Lời cảm ơn - Nhận được", Value = 0},
                        new Proc_GetKPIMobile(){Id = 3,ActionName = "Cơ hội kinh doanh - Trao đi", Value = 0},
                        new Proc_GetKPIMobile(){Id = 4,ActionName = "Cơ hội kinh doanh - Nhận được", Value = 0},
                        new Proc_GetKPIMobile(){Id = 5,ActionName = "Face to face - Hẹn gặp", Value = 0},
                        new Proc_GetKPIMobile(){Id = 6,ActionName = "Face to face - Được mời", Value = 0},
                        new Proc_GetKPIMobile(){Id = 7,ActionName = "Khách mời", Value = 0},
                        new Proc_GetKPIMobile(){Id = 8,ActionName = "Giáo dục", Value = 0},
                    };
                    if (yearFrom == yearTo)
                    {
                        if (monthFrom > monthTo) return JsonUtil.Error("Tháng bắt đầu không được lớn hơn tháng kết thúc", dataError);
                    }
                    if (yearFrom > yearTo) return JsonUtil.Error("Năm bắt đầu không được lớn hơn năm kết thúc", dataError);
                    var data = _unitOfWork.Repository<Proc_GetKPIMobile>()
                        .ExecProcedure(Proc_GetKPIMobile.GetEntityProc(customerId, monthFrom, yearFrom, monthTo, yearTo,"vi")).ToList();
                    return JsonUtil.Success(data);
                }
                else
                {
                    var dataError = new List<Proc_GetKPIMobile>()
                    {
                        new Proc_GetKPIMobile(){Id = 1,ActionName = "Thank you For the closed Business - Transfer", Value = 0},
                        new Proc_GetKPIMobile(){Id = 2,ActionName = "Thank you For the closed Business - Receive", Value = 0},
                        new Proc_GetKPIMobile(){Id = 3,ActionName = "Referral - Transfer", Value = 0},
                        new Proc_GetKPIMobile(){Id = 4,ActionName = "Referral - Receive", Value = 0},
                        new Proc_GetKPIMobile(){Id = 5,ActionName = "Face to Face - Transfer", Value = 0},
                        new Proc_GetKPIMobile(){Id = 6,ActionName = "Face to Face - Receive", Value = 0},
                        new Proc_GetKPIMobile(){Id = 7,ActionName = "Guests", Value = 0},
                        new Proc_GetKPIMobile(){Id = 8,ActionName = "CEU", Value = 0},
                    };
                    if (yearFrom == yearTo)
                    {
                        if (monthFrom > monthTo) return JsonUtil.Error("The start month cannot be greater than the end month", dataError);
                    }
                    if (yearFrom > yearTo) return JsonUtil.Error("The start year cannot be greater than the end year", dataError);
                    var data = _unitOfWork.Repository<Proc_GetKPIMobile>()
                        .ExecProcedure(Proc_GetKPIMobile.GetEntityProc(customerId, monthFrom, yearFrom, monthTo, yearTo,"en")).ToList();
                    return JsonUtil.Success(data);
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetKpiWeb(int chapterId, DateTime fromDate)
        {
            try
            {
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>()
                    .FindByNotEnabled(x => x.ChapterId == chapterId & x.IsEnabled == false & x.Time > fromDate).ToList();
               
                var listCustomer = _unitOfWork.Repository<Proc_GetCustomerInChapterWithChapterId>()
                    .ExecProcedure(Proc_GetCustomerInChapterWithChapterId.GetEntityProc(chapterId, fromDate)).ToList();
                List<KpiViewModel> result = new List<KpiViewModel>();
                foreach (var customer in listCustomer)
                {
                    int P = 0, A = 0, L = 0, M = 0, S = 0;
                    KpiViewModel itemResult = new KpiViewModel()
                    {
                        FullName = customer.FullName
                    };
                    if (meetingChapter.Any())
                    {
                        foreach (var itemMeetingChapter in meetingChapter)
                        {
                            var checkIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>().FindByNotEnabled(x =>
                                x.CustomerId == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (checkIn != null)
                            {
                                if (checkIn.IsEnabled == false)
                                {
                                    var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                    var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                        x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id);
                                    if (goInstead != null && absenceMedical != null)
                                    {
                                        if (goInstead.CreatedWhen < absenceMedical.CreatedWhen)
                                        {
                                            if (goInstead.IsCheckin == true)
                                            {
                                                S += 1;
                                            }
                                            else
                                            {
                                                M += 1;
                                            }
                                        }
                                        else
                                        {
                                            M += 1;
                                        }
                                    }
                                    else

                                    if (goInstead != null && absenceMedical == null)
                                    {
                                        if (goInstead.IsCheckin == true)
                                        {
                                            S += 1;
                                        }
                                        else
                                        {
                                            A += 1;
                                        }
                                    }
                                    else

                                    if (absenceMedical != null && goInstead == null)
                                    {
                                        M += 1;
                                    }
                                    else

                                    if (goInstead == null && absenceMedical == null) A += 1;
                                }
                                else
                                {
                                    TimeSpan time = checkIn.ModifiedWhen.GetValueOrDefault() - itemMeetingChapter.Time;

                                    if (time.TotalMinutes < 1)
                                    {
                                        var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                        var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id);
                                        if (goInstead != null && absenceMedical != null)
                                        {
                                            if (goInstead.CreatedWhen > absenceMedical.CreatedWhen)
                                            {
                                                if (goInstead.IsCheckin == true)
                                                {
                                                    S += 1;
                                                }
                                                else
                                                {
                                                    M += 1;
                                                }
                                            }
                                            else
                                            {
                                                M += 1;
                                            }
                                        }
                                        else

                                        if (goInstead != null && absenceMedical == null)
                                        {
                                            if (goInstead.IsCheckin == true)
                                            {
                                                S += 1;
                                            }
                                            else
                                            {
                                                P += 1;
                                            }
                                        }
                                        else

                                        if (absenceMedical != null && goInstead == null)
                                        {
                                            M += 1;
                                        }
                                        else

                                        if (goInstead == null && absenceMedical == null) P += 1;
                                    }
                                    else if (time.TotalMinutes >= 1 && time.TotalMinutes < 60)
                                    {
                                        var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                        var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id);
                                        if (goInstead != null && absenceMedical != null)
                                        {
                                            if (goInstead.CreatedWhen < absenceMedical.CreatedWhen)
                                            {
                                                if (goInstead.IsCheckin == true)
                                                {
                                                    S += 1;
                                                }
                                                else
                                                {
                                                    M += 1;
                                                }
                                            }
                                            else
                                            {
                                                M += 1;
                                            }
                                        }
                                        else

                                        if (goInstead != null && absenceMedical == null)
                                        {
                                            if (goInstead.IsCheckin == true)
                                            {
                                                S += 1;
                                            }
                                            else
                                            {
                                                L += 1;
                                            }
                                        }
                                        else

                                        if (absenceMedical != null && goInstead == null)
                                        {
                                            M += 1;
                                        }
                                        else

                                        if (goInstead == null && absenceMedical == null) L += 1;
                                    }
                                    else
                                    {
                                        var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                        var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeetingChapter.Id);
                                        if (goInstead != null && absenceMedical != null)
                                        {
                                            if (goInstead.CreatedWhen < absenceMedical.CreatedWhen)
                                            {
                                                if (goInstead.IsCheckin == true)
                                                {
                                                    S += 1;
                                                }
                                                else
                                                {
                                                    M += 1;
                                                }
                                            }
                                            else
                                            {
                                                M += 1;
                                            }
                                        }
                                        else

                                        if (goInstead != null && absenceMedical == null)
                                        {
                                            if (goInstead.IsCheckin == true)
                                            {
                                                S += 1;
                                            }
                                            else
                                            {
                                                A += 1;
                                            }
                                        }
                                        else

                                        if (absenceMedical != null && goInstead == null)
                                        {
                                            M += 1;
                                        }
                                        else

                                        if (goInstead == null && absenceMedical == null) A += 1;
                                    }
                                }
                            }
                            

                        }
                    }
                    

                    var itemMeeting = _unitOfWork.RepositoryR<MeetingChapter>().FindBy(x => x.ChapterId == chapterId).FirstOrDefault();
                    if (itemMeeting != null)
                    {
                        var checkIn = _unitOfWork.RepositoryR<MeetingChapterCheckIn>().FindBy(x =>
                            x.CustomerId == customer.Id && x.MeetingChapterId == itemMeeting.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                        if (checkIn != null)
                        {
                            TimeSpan time = checkIn.ModifiedWhen.GetValueOrDefault() - itemMeeting.Time;

                            if (time.TotalMinutes < 1)
                            {
                                var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                    x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id);
                                if (goInstead != null && absenceMedical != null)
                                {
                                    if (goInstead.CreatedWhen > absenceMedical.CreatedWhen)
                                    {
                                        if (goInstead.IsCheckin == true)
                                        {
                                            S += 1;
                                        }
                                        else
                                        {
                                            M += 1;
                                        }
                                    }
                                    else
                                    {
                                        M += 1;
                                    }
                                }else

                                if (goInstead != null && absenceMedical == null)
                                {
                                    if (goInstead.IsCheckin == true)
                                    {
                                        S += 1;
                                    }
                                    else
                                    {
                                        P += 1;
                                    }
                                }
                                else

                                if (absenceMedical != null && goInstead == null)
                                {
                                    M += 1;
                                }
                                else

                                if (goInstead == null && absenceMedical == null) P += 1;
                            }
                            else if (time.TotalMinutes >= 1 && time.TotalMinutes < 60)
                            {
                                var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                    x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id);
                                if (goInstead != null && absenceMedical != null)
                                {
                                    if (goInstead.CreatedWhen > absenceMedical.CreatedWhen)
                                    {
                                        if (goInstead.IsCheckin == true)
                                        {
                                            S += 1;
                                        }
                                        else
                                        {
                                            M += 1;
                                        }
                                    }
                                    else
                                    {
                                        M += 1;
                                    }
                                }
                                else

                                if (goInstead != null && absenceMedical == null)
                                {
                                    if (goInstead.IsCheckin == true)
                                    {
                                        S += 1;
                                    }
                                    else
                                    {
                                        L += 1;
                                    }
                                }
                                else

                                if (absenceMedical != null && goInstead == null)
                                {
                                    M += 1;
                                }
                                else

                                if (goInstead == null && absenceMedical == null) L += 1;
                            }
                            else
                            {
                                var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                           x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id &&
                                           x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                    x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id);
                                if (goInstead != null && absenceMedical != null)
                                {
                                    if (goInstead.CreatedWhen < absenceMedical.CreatedWhen)
                                    {
                                        if (goInstead.IsCheckin == true)
                                        {
                                            S += 1;
                                        }
                                        else
                                        {
                                            M += 1;
                                        }
                                    }
                                    else
                                    {
                                        M += 1;
                                    }
                                }
                                else

                                if (goInstead != null && absenceMedical == null)
                                {
                                    if (goInstead.IsCheckin == true)
                                    {
                                        S += 1;
                                    }
                                    else
                                    {
                                        A += 1;
                                    }
                                }
                                else

                                if (absenceMedical != null && goInstead == null)
                                {
                                    M += 1;
                                }
                                else

                                if (goInstead == null && absenceMedical == null) A += 1;
                            }
                        }
                        else
                        {
                            if (itemMeeting.Time.AddHours(1) < DateTime.Now)
                            {
                                var goInstead = _unitOfWork.RepositoryR<Guests>().FindBy(x =>
                                            x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id &&
                                            x.IsGuests == false).OrderByDescending(x => x.Id).FirstOrDefault();
                                var absenceMedical = _unitOfWork.RepositoryR<AbsenceMedical>().GetSingle(x =>
                                    x.CreatedBy == customer.Id && x.MeetingChapterId == itemMeeting.Id);
                                if (goInstead != null && absenceMedical != null)
                                {
                                    if (goInstead.CreatedWhen < absenceMedical.CreatedWhen)
                                    {
                                        if (goInstead.IsCheckin == true)
                                        {
                                            S += 1;
                                        }
                                        else
                                        {
                                            M += 1;
                                        }
                                    }
                                    else
                                    {
                                        M += 1;
                                    }
                                }else

                                if (goInstead != null && absenceMedical == null)
                                {
                                    if (goInstead.IsCheckin == true)
                                    {
                                        S += 1;
                                    }
                                    else
                                    {
                                        A += 1;
                                    }
                                }
                                else

                                if (absenceMedical != null && goInstead == null)
                                {
                                    M += 1;
                                }
                                else

                                if (goInstead == null && absenceMedical == null) A += 1;
                            }
                        }
                    }

                    itemResult.P = P;
                    itemResult.A = A;
                    itemResult.L = L;
                    itemResult.M = M;
                    itemResult.S = S;

                    itemResult.RGI = customer.RGI;
                    itemResult.RGO = customer.RGO;
                    itemResult.RRI = customer.RRI;
                    itemResult.RRO = customer.RRO;
                    itemResult.V = customer.V;
                    itemResult.F2F = customer.F2F;
                    itemResult.TYFCB = customer.TYFCB;

                    itemResult.CEU = customer.CEU;
                    int sumPALMS = itemResult.P + itemResult.A + itemResult.L + itemResult.M + itemResult.S;
                    if (sumPALMS == 0)
                    {
                        itemResult.R = 0;
                        itemResult.AvgF2F = 0;
                        itemResult.AvgV = 0;
                    }
                    else
                    {
                        double r = (itemResult.RGI + itemResult.RGO) / (float)sumPALMS;
                        itemResult.R = Math.Round(r, 2);
                        double f2f = (itemResult.F2F) / (float)sumPALMS;
                        itemResult.AvgF2F = Math.Round(f2f, 2);
                        double v = (itemResult.V) / (float)sumPALMS;
                        itemResult.AvgV = Math.Round(v, 2);
                    }

                    if (itemResult.A > 2)
                    {
                        itemResult.PointA = 0;
                        itemResult.ColorA = "#E6E6E6";
                    }else if (itemResult.A == 2)
                    {
                        itemResult.PointA = 5;
                        itemResult.ColorA = "#b92719";
                    }else if (itemResult.A == 1)
                    {
                        itemResult.PointA = 10;
                        itemResult.ColorA = "#F8D849";
                    }else 
                    {
                        itemResult.PointA = 15;
                        itemResult.ColorA = "#A5A137";
                    }


                    if (itemResult.L >= 2)
                    {
                        itemResult.PointL = 0;
                        itemResult.ColorL = "#E6E6E6";
                    }
                    else if (itemResult.L == 1)
                    {
                        itemResult.PointL = 5;
                        itemResult.ColorL = "#F8D849";
                    }
                    else
                    {
                        itemResult.PointL = 10;
                        itemResult.ColorL = "#A5A137";
                    }

                    itemResult.RG = itemResult.RGI + itemResult.RGO;
                    itemResult.RR = itemResult.RRI + itemResult.RRO;

                    if (itemResult.R < 0.75)
                    {
                        itemResult.PointRG = 0;
                        itemResult.ColorRG = "#E6E6E6";
                    }
                    else if (itemResult.R < 1)
                    {
                        itemResult.PointRG = 5;
                        itemResult.ColorRG = "#b92719";
                    }
                    else if (itemResult.R < 1.2)
                    {
                        itemResult.PointRG = 10;
                        itemResult.ColorRG = "#F8D849";
                    }
                    else if (itemResult.R < 1.5)
                    {
                        itemResult.PointRG = 15;
                        itemResult.ColorRG = "#F8D849";
                    }
                    else
                    {
                        itemResult.PointRG = 20;
                        itemResult.ColorRG = "#A5A137";
                    }


                    if (itemResult.AvgF2F <= 1)
                    {
                        itemResult.PointF2F = 0;
                        itemResult.ColorF2F = "#E6E6E6";
                    }
                    else if (itemResult.AvgF2F < 2)
                    {
                        itemResult.PointF2F = 5;
                        itemResult.ColorF2F = "#F8D849";
                    }
                    else
                    {
                        itemResult.PointF2F = 10;
                        itemResult.ColorF2F = "#A5A137";
                    }


                    if (itemResult.AvgV < 0.1)
                    {
                        itemResult.PointV = 0;
                        itemResult.ColorV = "#E6E6E6";
                    }
                    else if (itemResult.AvgV < 0.25)
                    {
                        itemResult.PointV = 5;
                        itemResult.ColorV = "#b92719";
                    }
                    else if (itemResult.AvgV < 0.5)
                    {
                        itemResult.PointV = 10;
                        itemResult.ColorV = "#F8D849";
                    }
                    else if (itemResult.AvgV < 0.75)
                    {
                        itemResult.PointV = 15;
                        itemResult.ColorV = "#F8D849";
                    }
                    else
                    {
                        itemResult.PointV = 20;
                        itemResult.ColorV = "#A5A137";
                    }

                    if (itemResult.CEU == 0)
                    {
                        itemResult.PointT = 0;
                        itemResult.ColorT = "#E6E6E6";
                    }
                    else if (itemResult.CEU == 1)
                    {
                        itemResult.PointT = 5;
                        itemResult.ColorT = "#F8D849";
                    }
                    else
                    {
                        itemResult.PointT = 10;
                        itemResult.ColorT = "#A5A137";
                    }

                    if (itemResult.TYFCB < 50000000)
                    {
                        itemResult.PointTYFCB = 0;
                        itemResult.ColorTYFCB = "#E6E6E6";
                    }
                    else if (itemResult.TYFCB < 100000000)
                    {
                        itemResult.PointTYFCB = 5;
                        itemResult.ColorTYFCB = "#b92719";
                    }
                    else if (itemResult.TYFCB < 250000000)
                    {
                        itemResult.PointTYFCB = 10;
                        itemResult.ColorTYFCB = "#F8D849";
                    }
                    else
                    {
                        itemResult.PointTYFCB = 15;
                        itemResult.ColorTYFCB = "#A5A137";
                    }

                    itemResult.Pts = itemResult.PointA + itemResult.PointL + itemResult.PointRG + itemResult.PointF2F +
                                     itemResult.PointV + itemResult.PointT + itemResult.PointTYFCB;

                    if (itemResult.Pts >= 70)
                    {
                        itemResult.ColorRow = "#A5A137";
                    }else if (itemResult.Pts >= 50 && itemResult.Pts < 70)
                    {
                        itemResult.ColorRow = "#F8D849";
                    }else if (itemResult.Pts >= 30 && itemResult.Pts < 50)
                    {
                        itemResult.ColorRow = "#b92719";
                    }
                    else
                    {
                        itemResult.ColorRow = "#E6E6E6";
                    }

                    result.Add(itemResult);
                }
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
