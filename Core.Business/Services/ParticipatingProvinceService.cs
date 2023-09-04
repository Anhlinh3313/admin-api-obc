using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.Chapter;
using Core.Business.ViewModels.ParticipatingProvince;
using Core.Business.ViewModels.Region;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class ParticipatingProvinceService : BaseService, IParticipatingProvinceService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ParticipatingProvinceService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> GetListProvinceAsync(string keySearch, int pageNum, int pageSize)
        {
            try
            {
                var listProvince = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .FindBy(x => x.IsEnabled == true &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Code.ToLower().Contains(keySearch.Trim().ToLower()) ||
                                  x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).OrderByDescending(x => x.Id).ToListAsync();
                var total = listProvince.Count();
                var totalPage = (int)Math.Ceiling((double)total / pageSize);
                var result = listProvince.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
                return JsonUtil.Success(result, "Success", total);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetDetailProvinceAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.RepositoryR<ParticipatingProvince>().GetSingleAsync(x => x.Id == id);
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> CreateProvinceAsync(ParticipatingProvinceViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Province.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Province.NameNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                if (_unitOfWork.RepositoryR<ParticipatingProvince>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Province.UniqueCode);
                if (_unitOfWork.RepositoryR<ParticipatingProvince>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                    return JsonUtil.Error(ValidatorMessage.Province.UniqueName);

                model.IsActive = true;
                var result = await _iGeneralRawService.Create<ParticipatingProvince, ParticipatingProvinceViewModel>(model);
                return JsonUtil.Success(result);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> UpdateProvinceAsync(ParticipatingProvinceViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrWhiteSpace(model.Code))
                    return JsonUtil.Error(ValidatorMessage.Province.CodeNotEmpty);
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                    return JsonUtil.Error(ValidatorMessage.Province.NameNotEmpty);

                model.Code = model.Code.Trim();
                model.Name = model.Name.Trim();

                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>().GetSingleAsync(x => x.Id == model.Id);
                if (!province.Code.ToLower().Contains(model.Code.ToLower()))
                    if (_unitOfWork.RepositoryR<ParticipatingProvince>().Any(x => x.Code.ToLower().Equals(model.Code.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Province.UniqueCode);
                if (!province.Name.ToLower().Contains(model.Name.ToLower()))
                    if (_unitOfWork.RepositoryR<ParticipatingProvince>().Any(x => x.Name.ToLower().Equals(model.Name.ToLower()) && x.Id != model.Id))
                        return JsonUtil.Error(ValidatorMessage.Province.UniqueName);

                if (model.IsActive == null) model.IsActive = province.IsActive;
                model.IsEnabled = true;
                var result = await _iGeneralRawService.Update<ParticipatingProvince, ParticipatingProvinceViewModel>(model);
                return JsonUtil.Success(result);

            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeActiveProvinceAsync(int provinceId)
        {
            try
            {
                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>().GetSingleAsync(x => x.Id == provinceId);
                if (province == null) return JsonUtil.Error(ValidatorMessage.Province.NotExist);
                if (province.IsActive)
                {
                    var region = await _unitOfWork.RepositoryR<Region>().AnyAsync(x => x.ProvinceId == provinceId && 
                                                                                       x.IsActive == true);
                    if (region) return JsonUtil.Error(ValidatorMessage.General.NotDeActive);
                }

                province.IsActive = !province.IsActive;
                _unitOfWork.RepositoryCRUD<ParticipatingProvince>().Update(province);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DeEnabledProvinceAsync(int provinceId)
        {
            try
            {
                var region = await _unitOfWork.RepositoryR<Region>().AnyAsync(x => x.ProvinceId == provinceId && x.IsEnabled == true);
                if (region) return JsonUtil.Error(ValidatorMessage.General.NotDestroy);
                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>().GetSingleAsync(x => x.Id == provinceId);
                if (province == null) return JsonUtil.Error(ValidatorMessage.Province.NotExist);
                province.IsEnabled = !province.IsEnabled;
                _unitOfWork.RepositoryCRUD<ParticipatingProvince>().Update(province);
                await _unitOfWork.CommitAsync();
                return JsonUtil.Success(true);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DropdownProvinceAsync(string keySearch)
        {
            try
            {
                var listProvince = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .FindBy(x => x.IsActive == true &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                return JsonUtil.Success(listProvince);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetProvinceAndRegionWithChapterId(int chapterId)
        {
            try
            {
                var data = _unitOfWork.Repository<Poc_GetProvinceRegionWithChapterId>()
                    .ExecProcedure(Poc_GetProvinceRegionWithChapterId.GetEntityProc(chapterId)).ToList();
                return JsonUtil.Success(data);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetProvinceTreeViewAsync(string keySearch)
        {
            try
            {
                var province = await _unitOfWork.RepositoryR<ParticipatingProvince>()
                    .FindBy(x => x.IsEnabled == true &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Code.ToLower().Contains(keySearch.Trim().ToLower()) ||
                                  x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                var result = new List<ParticipatingProvinceViewModelTreeView>();
                foreach (var item in province)
                {
                    var region = await _unitOfWork.RepositoryR<Region>()
                        .FindBy(x => x.IsEnabled == true && x.ProvinceId == item.Id).ToListAsync();
                    var listRegion = new List<RegionViewModelTreeView>();
                    foreach (var itemRegion in region)
                    {
                        var chapter = await _unitOfWork.RepositoryR<Chapter>()
                            .FindBy(x => x.IsEnabled == true && x.RegionId == itemRegion.Id).ToListAsync();
                        listRegion.Add(new RegionViewModelTreeView()
                        {
                            Id = itemRegion.Id,
                            Code = itemRegion.Code,
                            IsActive = itemRegion.IsActive,
                            IsEnabled = itemRegion.IsEnabled,
                            Name = itemRegion.Name,
                            Note = itemRegion.Note,
                            ProvinceId = item.Id,
                            Children = Mapper.Map<List<ChapterViewModelCreate>>(chapter)
                        });
                    }
                    result.Add(new ParticipatingProvinceViewModelTreeView()
                    {
                        Id = item.Id,
                        Code = item.Code,
                        IsActive = item.IsActive,
                        IsEnabled = item.IsEnabled,
                        Name = item.Name,
                        Note = item.Note,
                        Children = listRegion
                    });
                }

                var test = result;
                return JsonUtil.Success(result);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
