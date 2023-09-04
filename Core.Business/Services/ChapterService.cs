using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Chapter;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class ChapterService : BaseService, IChapterService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ChapterService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetListChapterAsync(string keySearch, string province, string region, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(keySearch) || !string.IsNullOrWhiteSpace(keySearch)) keySearch = keySearch.Trim();
                if (!string.IsNullOrEmpty(province) || !string.IsNullOrWhiteSpace(province)) province = province.Trim();
                if (!string.IsNullOrEmpty(region) || !string.IsNullOrWhiteSpace(region)) region = region.Trim();
                var data = _unitOfWork.Repository<Proc_GetListChapter>()
                        .ExecProcedure(Proc_GetListChapter.GetEntityProc(province, region, keySearch, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailChapterAsync(int id)
        {
            try
            {
                var chapter = await _unitOfWork.RepositoryR<Chapter>().GetSingleAsync(x => x.Id == id);
                return JsonUtil.Success(chapter);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetChapterInformation(int id)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetChapterInformation>()
                    .ExecProcedure(Proc_GetChapterInformation.GetEntityProc(id)).ToList();
                return JsonUtil.Success(data, "Success");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateChapterAsync(ChapterViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Chapter.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Chapter.NameNotEmpty);
                if (string.IsNullOrEmpty(model.LinkGroupChat) || string.IsNullOrWhiteSpace(model.LinkGroupChat))
                    model.LinkGroupChat = "";
                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();
                model.LinkGroupChat = model.LinkGroupChat.Trim();

                if (_unitOfWork.RepositoryR<Chapter>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Chapter.UniqueCode);
                if (_unitOfWork.RepositoryR<Chapter>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Chapter.UniqueName);

                var region = await _unitOfWork.RepositoryR<Region>()
                    .GetSingleAsync(x => x.Id == model.RegionId && x.IsActive == true);
                if (region == null)
                {
                    return JsonUtil.Error(ValidatorMessage.Chapter.RegionNotActive);
                }

                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .GetSingleAsync(x => x.Id == model.ProvinceId && x.IsActive == true);
                if (province == null)
                {
                    return JsonUtil.Error(ValidatorMessage.Chapter.ProvinceNotActive);
                }
                model.IsActive = true;
                var chapter = await _iGeneralRawService.Create<Chapter, ChapterViewModelCreate>(model);
                return JsonUtil.Success(new ChapterViewModelReturn()
                {
                    ChapterName = model.Name,
                    RegionName = region.Name,
                    ProvinceName = province.Name,
                    ChapterId = ((Chapter) chapter.Data).Id
                });

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateChapterAsync(ChapterViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Chapter.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Chapter.NameNotEmpty);
                if (string.IsNullOrEmpty(model.LinkGroupChat) || string.IsNullOrWhiteSpace(model.LinkGroupChat))
                    model.LinkGroupChat = "";

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();
                model.LinkGroupChat = model.LinkGroupChat.Trim();

                var chapterOld = await _unitOfWork.RepositoryR<Chapter>().GetSingleAsync(x => x.Id == model.Id);
                if (!chapterOld.Code.ToLower().Equals(model.Code.ToLower()))
                    if (_unitOfWork.RepositoryR<Chapter>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Chapter.UniqueCode);
                if (!chapterOld.Name.ToLower().Equals(model.Name.ToLower()))
                    if (_unitOfWork.RepositoryR<Chapter>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.Id  != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Chapter.UniqueName);
                var region = await _unitOfWork.RepositoryR<Region>()
                    .GetSingleAsync(x => x.Id == model.RegionId && x.IsActive == true);

                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .GetSingleAsync(x => x.Id == model.ProvinceId && x.IsActive == true);
                if (model.IsActive == null) model.IsActive = chapterOld.IsActive;
                model.IsEnabled = true;
                var chapter = await _iGeneralRawService.Update<Chapter, ChapterViewModelCreate>(model);
                return JsonUtil.Success(new ChapterViewModelReturn()
                {
                    ChapterName = model.Name,
                    RegionName = region.Name,
                    ProvinceName = province.Name,
                    ChapterId = ((Chapter)chapter.Data).Id
                });

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledChapterAsync(int chapterId)
        {
            try
            {
                var chapter = await _unitOfWork.RepositoryR<Chapter>().GetSingleAsync(x => x.Id == chapterId);
                if (chapter == null) return JsonUtil.Error(ValidatorMessage.Chapter.NotExist);
                if (chapter.IsEnabled)
                {
                    var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                        .AnyAsync(x => x.IsEnabled == true && x.ParticipatingChapterId == chapterId);
                    if (business) return JsonUtil.Error(ValidatorMessage.General.NotDestroy);
                }
                chapter.IsEnabled = !chapter.IsEnabled;
                _unitOfWork.RepositoryCRUD<Chapter>().Update(chapter);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveChapterAsync(int chapterId)
        {
            try
            {
                var chapter = await _unitOfWork.RepositoryR<Chapter>().GetSingleAsync(x => x.Id == chapterId);
                if (chapter == null) return JsonUtil.Error(ValidatorMessage.Chapter.NotExist);
                if (chapter.IsActive)
                {
                    var business = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                        .GetSingleAsync(x => x.IsActive == true && x.ParticipatingChapterId == chapterId);
                    if (business != null) return JsonUtil.Error(ValidatorMessage.General.NotDeActive);
                }

                if (!chapter.IsActive)
                {
                    var region = await _unitOfWork.RepositoryR<Region>()
                        .AnyAsync(x => x.Id == chapter.RegionId && x.IsActive == true);
                    if (!region) return JsonUtil.Error(ValidatorMessage.Chapter.NotActive);
                }
                chapter.IsActive = !chapter.IsActive;
                _unitOfWork.RepositoryCRUD<Chapter>().Update(chapter);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetChapterWithRegionIdAsync(int regionId, string keySearch)
        {
            try
            {
                var chapter = await _unitOfWork.RepositoryR<Chapter>().FindBy(x => 
                                        x.RegionId == regionId &&
                                        x.IsActive == true &&
                                        (string.IsNullOrEmpty(keySearch) ||
                                         x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                return JsonUtil.Success(chapter);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetChapterMobile(string keySearch)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetListChapterMobile>()
                    .ExecProcedure(Proc_GetListChapterMobile.GetEntityProc( keySearch)).ToList();
                
                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CheckFieldOperationUnique(int fieldOperationId, int chapterId)
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .AnyAsync(x => x.FieldOperationsId == fieldOperationId && x.ParticipatingChapterId == chapterId);
                if (result) return JsonUtil.Success("Trong Chapter đã có lĩnh vực này, k thể thêm !!!");
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetDetailMemberChapter(int businessId)
        {
            try
            {
                var data = _unitOfWork.Repository<Proc_GetDetailMemberChapter>()
                    .ExecProcedure(Proc_GetDetailMemberChapter.GetEntityProc(businessId)).FirstOrDefault();
                return JsonUtil.Success(data, "Success");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GenerateQrCodeCustomer(FileStreamResult streamResult, int chapterId)
        {
            try
            {
                var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);
                var QrPath = GenerateQrCode(streamResult, chapterId.ToString(),
                    "QrCodeChapter");

                chapter.QrCodePath = QrPath;
                _unitOfWork.RepositoryCRUD<Chapter>().Update(chapter);
                _unitOfWork.Commit();

                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetInformationChapterInGuests(int customerId)
        {
            try
            {
                var chapterId = _unitOfWork.RepositoryR<Entity.Entities.Business>()
                    .GetSingle(x => x.CustomerId == customerId).ParticipatingChapterId.GetValueOrDefault();
                var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);
                var meetingChapter = _unitOfWork.RepositoryR<MeetingChapter>().FindBy(x => x.ChapterId == chapterId)
                    .ToList();
                return JsonUtil.Success(new
                {
                    ChapterName = chapter.Name,
                    MeetingChapter = meetingChapter,
                    Link = meetingChapter.FirstOrDefault().Link,
                    Address = meetingChapter.FirstOrDefault().Address
                });
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public string GenerateQrCode(FileStreamResult streamResult, string fileName, string folderName)
        {
            try
            {
                string path = $@"{ApplicationEnvironment.ApplicationBasePath}{folderName}";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fullPath = Path.Combine(path, fileName + ".png");

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
                    streamResult.FileStream.CopyTo(fileStream);
                }

                string result = folderName + '/' + fileName + ".png";

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
