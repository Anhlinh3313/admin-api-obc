using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Region;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class RegionService : BaseService, IRegionService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public RegionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public JsonResult GetListRegionAsync(string keySearch, string province, int pageNum, int pageSize)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(province) || !string.IsNullOrEmpty(province)) province = province.Trim();
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                var data = _unitOfWork.Repository<Proc_GetListRegion>()
                                        .ExecProcedure(Proc_GetListRegion.GetEntityProc(province, keySearch, pageNum, pageSize)).ToList();
                if (!data.Any()) return JsonUtil.Success(data, "Success", 0);
                return JsonUtil.Success(data, "Success", data.FirstOrDefault()?.Total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailRegionAsync(int id)
        {
            try
            {
                var region = await _unitOfWork.RepositoryR<Region>().GetSingleAsync(x => x.Id == id);
                return JsonUtil.Success(region, "Success");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateRegionAsync(RegionViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Region.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Region.NameNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                if (_unitOfWork.RepositoryR<Region>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Region.UniqueCode);
                if (_unitOfWork.RepositoryR<Region>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Region.UniqueName);

                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .AnyAsync(x => x.Id == model.ProvinceId && x.IsActive == true);
                if (!province)
                {
                    return JsonUtil.Error(ValidatorMessage.Region.ProvinceNotActive); 
                }
                model.IsActive = true;
                //Region region = Mapper.Map<Region>(model);
                //region.Id = 0;
                //_unitOfWork.RepositoryCRUD<Region>().Insert(region);
                //await _unitOfWork.CommitAsync();
                var region = await _iGeneralRawService.Create<Region, RegionViewModelCreate>(model);
                return JsonUtil.Success(region, "Success");

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateRegionAsync(RegionViewModelCreate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Region.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Region.NameNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                var regionOld = await _unitOfWork.RepositoryR<Region>().GetSingleAsync(x => x.Id == model.Id);
                if (!regionOld.Code.ToLower().Equals(model.Code.ToLower()))
                    if (_unitOfWork.RepositoryR<Region>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Region.UniqueCode);
                if (!regionOld.Name.ToLower().Equals(model.Name.ToLower()))
                    if (_unitOfWork.RepositoryR<Region>().Any(x => x.Name.Equals(model.Name.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Region.UniqueName);
                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .AnyAsync(x => x.Id == model.ProvinceId && x.IsActive == true);
                if (!province)
                {
                    return JsonUtil.Error(ValidatorMessage.Region.ProvinceNotActive);
                }
                if (model.IsActive == null) model.IsActive = regionOld.IsActive;
                model.IsEnabled = true;
                var region = await _iGeneralRawService.Update<Region, RegionViewModelCreate>(model);
                return JsonUtil.Success(region);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledRegionAsync(int regionId)
        {
            try
            {
                var chapter = await _unitOfWork.RepositoryR<Chapter>().AnyAsync(x => x.RegionId == regionId && x.IsEnabled == true);
                if (chapter) return JsonUtil.Error(ValidatorMessage.General.NotDestroy);
                var region = await _unitOfWork.RepositoryR<Region>().GetSingleAsync(x => x.Id == regionId);
                if (region == null) return JsonUtil.Error(ValidatorMessage.Region.NotExist);

                region.IsEnabled = !region.IsEnabled;
                _unitOfWork.RepositoryCRUD<Region>().Update(region);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveRegionAsync(int regionId)
        {
            try
            {
                var region = await _unitOfWork.RepositoryR<Region>().GetSingleAsync(x => x.Id == regionId);
                if (region == null) return JsonUtil.Error(ValidatorMessage.Region.NotExist);
                if (region.IsActive)
                {
                    var chapter = await _unitOfWork.RepositoryR<Chapter>().AnyAsync(x => x.RegionId == regionId && x.IsActive == true);
                    if (chapter) return JsonUtil.Error(ValidatorMessage.General.NotDeActive);
                }

                if (!region.IsActive)
                {
                    var province = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                        .AnyAsync(x => x.Id == region.ProvinceId && x.IsActive == true);
                    if (!province) return JsonUtil.Error(ValidatorMessage.Region.NotActive);
                }
                region.IsActive = !region.IsActive;
                _unitOfWork.RepositoryCRUD<Region>().Update(region);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllRegionAsync(string keySearch, string province)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(province) || string.IsNullOrEmpty(province))
                    return JsonUtil.Error("Vui lòng chọn Tỉnh/Thành");

                province = province.Trim();
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();
                
                var data = _unitOfWork.Repository<Proc_GetListRegionWithProvince>()
                    .ExecProcedure(Proc_GetListRegionWithProvince.GetEntityProc(province, keySearch)).ToList();
                return JsonUtil.Success(data, "Success");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public JsonResult GetAllRegion(string keySearch)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();

                var data = _unitOfWork.Repository<Proc_GetListRegionWithProvince>()
                    .ExecProcedure(Proc_GetListRegionWithProvince.GetEntityProc("", keySearch)).ToList();
                return JsonUtil.Success(data, "Success");
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
