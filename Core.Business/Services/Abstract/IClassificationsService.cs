using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IClassificationsService
    {
        JsonResult GetClassifications();
        JsonResult GetClassificationsNotInChapter(int chapterId);
    }
}
