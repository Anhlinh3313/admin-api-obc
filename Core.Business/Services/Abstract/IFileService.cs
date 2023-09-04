 using System;
 using System.Collections.Generic;
 using System.Threading.Tasks;
using Core.Business.ViewModels.ParticipatingProvince;
 using Core.Business.ViewModels.Thanks;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IFileService
    {
        Task<JsonResult> UploadImageOptional(IFormFile file, string folderName, string fileName);
        Task<JsonResult> UploadFile(IFormFile file, string folderName, string fileName);
    }
}
