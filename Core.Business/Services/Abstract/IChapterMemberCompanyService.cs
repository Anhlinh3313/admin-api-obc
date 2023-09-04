using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IChapterMemberCompanyService
    {
        JsonResult GetChapterMemberCompanyWeb(int chapterId, DateTime? dateSearch);
        JsonResult GetChapterMemberCompany(int chapterId, DateTime dateSearch);
    }
}
