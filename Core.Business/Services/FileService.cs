using System;
using System.IO;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class FileService : BaseService, IFileService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public FileService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> UploadImageOptional(IFormFile file, string folderName, string fileName)
        {

            try
            {
                if (file == null || file.Length == 0)
                    return JsonUtil.Error("File not selected");
                else
                {
                    if (file.ContentType.Split('/')[0] != "image")
                    {
                        return JsonUtil.Error("Chỉ cho phép hình ảnh, vui lòng chọn lại!");
                    }

                    if (file.Length > 10000000)
                    {
                        return JsonUtil.Error("Dung lượng tệp quá giới hạn cho phép, vui lòng chọn lại!");
                    }
                }

                FileInfo fi = new FileInfo(file.FileName);
                string extn = fi.Extension;

                string path = $@"{ApplicationEnvironment.ApplicationBasePath}{folderName}";


                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fullPath = Path.Combine(path, fileName + extn);

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
                    await file.CopyToAsync(fileStream);
                }

                string result = folderName + '/' + fileName + extn;
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UploadFile(IFormFile file, string folderName, string fileName)
        {
            {
                if (file == null || file.Length == 0)
                    return JsonUtil.Error("File not selected");
                else
                {
                    if (file.Length > 2148000000)
                    {
                        return JsonUtil.Error("Dung lượng tệp quá giới hạn cho phép, vui lòng chọn lại!");
                    }
                }

                FileInfo fi = new FileInfo(file.FileName);
                string extn = fi.Extension;

                string fullPath = $@"{ApplicationEnvironment.ApplicationBasePath}{folderName}";

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var uniqueFileName = $"{fileName}{extn}";

                using (var fileStream = new FileStream(Path.Combine(fullPath, uniqueFileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                //Files newFile = new Files();
                //newFile.path = folderName + '/' + uniqueFileName;
                //newFile.createdWhen = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //newFile.updatedWhen = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //_context.Files.Add(newFile);
                //_context.SaveChanges();

                string result = folderName + '/' + fileName + extn;
                return JsonUtil.Success(result);

            }
        }
    }
}
