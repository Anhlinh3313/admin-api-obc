using System.Threading.Tasks;
using Core.Business.ViewModels.AbsenceMedical;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IAbsenceMedicalService
    {
        Task<JsonResult> CreateAbsenceMedical(AbsenceMedicalViewModel model, int currentId);
        JsonResult GetDetailAbsenceMedical(int absenceMedicalId, int currentId);
    }
}
