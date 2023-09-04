using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Profession;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class ProfessionService : BaseService, IProfessionService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ProfessionService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> DropdownProfessionAsync(string keySearch, string language)
        {
            try
            {
                if (language != null){
                    if (language.Equals("vi"))
                    {
                        var listProfession = await _unitOfWork.RepositoryR<Profession>()
                            .FindBy(x => x.IsEnabled == true &&
                                        (string.IsNullOrEmpty(keySearch) ||
                                        x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();

                        List<ProfessionModel> result = new List<ProfessionModel>();
                        foreach (var item in listProfession)
                        {
                            ProfessionModel itemResult = new ProfessionModel()
                            {
                                Id = item.Id,
                                Name = item.Name
                            };
                            result.Add(itemResult);
                        }
                        return JsonUtil.Success(result);
                    }
                    else
                    {
                        var listProfession = await _unitOfWork.RepositoryR<Profession>()
                            .FindBy(x => x.IsEnabled == true &&
                                        (string.IsNullOrEmpty(keySearch) ||
                                        x.Code.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();

                        List<ProfessionModel> result = new List<ProfessionModel>();
                        foreach (var item in listProfession)
                        {
                            ProfessionModel itemResult = new ProfessionModel()
                            {
                                Id = item.Id,
                                Name = item.Code
                            };
                            result.Add(itemResult);
                        }
                        return JsonUtil.Success(result);
                    }
                }else{
                    var listProfession = await _unitOfWork.RepositoryR<Profession>()
                        .FindBy(x => x.IsEnabled == true &&
                                    (string.IsNullOrEmpty(keySearch) ||
                                    x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();

                    List<ProfessionModel> result = new List<ProfessionModel>();
                    foreach (var item in listProfession)
                    {
                        ProfessionModel itemResult = new ProfessionModel()
                        {
                            Id = item.Id,
                            Name = item.Name
                        };
                        result.Add(itemResult);
                    }
                    return JsonUtil.Success(result);
                }
                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> DropdownProfessionFieldOperationsAsync(string keySearch)
        {
            try
            {
                var pro = await _unitOfWork.RepositoryR<Profession>()
                    .FindBy(x => x.IsEnabled == true ).ToListAsync();
                List<ProfessionViewModel> profession = new List<ProfessionViewModel>();
                foreach (var proItem in pro)
                {
                    var field = await _unitOfWork.RepositoryR<FieldOperations>()
                        .FindBy(x => x.ProfessionId == proItem.Id && x.IsEnabled == true && 
                                     (string.IsNullOrEmpty(keySearch) ||
                                      x.Name.ToLower().Contains(keySearch.Trim().ToLower()) ||
                                      x.Code.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                    if (field.Count <= 0)
                    {
                        field = await _unitOfWork.RepositoryR<FieldOperations>()
                            .FindBy(x => x.ProfessionId == proItem.Id && x.IsEnabled == true ).ToListAsync();
                    }
                    ProfessionViewModel professionDetail = new ProfessionViewModel()
                    {
                        Id = proItem.Id,
                        IsEnabled = proItem.IsEnabled,
                        Name = proItem.Name,
                        Code = proItem.Code,
                        FieldOperations = field
                    };
                    profession.Add(professionDetail);
                }

                var result = profession.Find(x => (string.IsNullOrEmpty(keySearch) ||
                                                   x.Name.ToLower().Contains(keySearch.Trim().ToLower())));
                return JsonUtil.Success(profession);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
