using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IMembershipDuesReportService
    {
        JsonResult GetMembershipDuesReport(int chapterId, DateTime fromDate);
    }
}
