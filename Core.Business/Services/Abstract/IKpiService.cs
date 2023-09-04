using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IKpiService
    {
        JsonResult GetKpiMobile(int customerId, int monthFrom, int yearFrom, int monthTo, int yearTo);
        JsonResult GetKpiWeb(int chapterId, DateTime fromDate);
    }
}
