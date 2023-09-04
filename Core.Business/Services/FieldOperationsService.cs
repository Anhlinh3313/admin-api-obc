using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.FieldOperations;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class FieldOperationsService : BaseService, IFieldOperationsService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public FieldOperationsService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }
        public async Task<JsonResult> GetAllFieldOperationsAsync(string keySearch, int? professionId, int customerId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();

                var customer = _unitOfWork.RepositoryR<Customer>().GetSingle(x => x.Id == customerId);
                if (customer == null || string.IsNullOrEmpty(customer.Language) || customer.Language.Equals("vi"))
                {
                    if (professionId == null)
                    {
                        var data = await _unitOfWork.RepositoryR<FieldOperations>().FindBy(x => x.IsEnabled == true &&
                                                                                                (string.IsNullOrEmpty(keySearch) ||
                                                                                                 x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                        List<FieldOperationsViewModel> result = new List<FieldOperationsViewModel>();
                        foreach (var item in data)
                        {
                            FieldOperationsViewModel itemResult = new FieldOperationsViewModel()
                            {
                                Id = item.Id,
                                Name = item.Name,
                                ProfessionId = item.ProfessionId
                            };
                            result.Add(itemResult);
                        }
                        return JsonUtil.Success(result, "Success");
                    }
                    else
                    {
                        var data = await _unitOfWork.RepositoryR<FieldOperations>().FindBy(x => x.IsEnabled == true && x.ProfessionId == professionId &&
                                                                                                (string.IsNullOrEmpty(keySearch) ||
                                                                                                 x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                        List<FieldOperationsViewModel> result = new List<FieldOperationsViewModel>();
                        foreach (var item in data)
                        {
                            FieldOperationsViewModel itemResult = new FieldOperationsViewModel()
                            {
                                Id = item.Id,
                                Name = item.Name,
                                ProfessionId = item.ProfessionId
                            };
                            result.Add(itemResult);
                        }
                        return JsonUtil.Success(result, "Success");
                    }
                }
                else
                {
                    if (professionId == null)
                    {
                        var data = await _unitOfWork.RepositoryR<FieldOperations>().FindBy(x => x.IsEnabled == true &&
                                                                                                (string.IsNullOrEmpty(keySearch) ||
                                                                                                 x.Code.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                        List<FieldOperationsViewModel> result = new List<FieldOperationsViewModel>();
                        foreach (var item in data)
                        {
                            FieldOperationsViewModel itemResult = new FieldOperationsViewModel()
                            {
                                Id = item.Id,
                                Name = item.Code,
                                ProfessionId = item.ProfessionId
                            };
                            result.Add(itemResult);
                        }
                        return JsonUtil.Success(result, "Success");
                    }
                    else
                    {
                        var data = await _unitOfWork.RepositoryR<FieldOperations>().FindBy(x => x.IsEnabled == true && x.ProfessionId == professionId &&
                                                                                                (string.IsNullOrEmpty(keySearch) ||
                                                                                                 x.Code.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                        List<FieldOperationsViewModel> result = new List<FieldOperationsViewModel>();
                        foreach (var item in data)
                        {
                            FieldOperationsViewModel itemResult = new FieldOperationsViewModel()
                            {
                                Id = item.Id,
                                Name = item.Code,
                                ProfessionId = item.ProfessionId
                            };
                            result.Add(itemResult);
                        }
                        return JsonUtil.Success(result, "Success");
                    }
                }

                
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        public async Task<JsonResult> GetAllFieldOperationsInWebAsync(string keySearch, int? professionId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(keySearch) || !string.IsNullOrEmpty(keySearch)) keySearch = keySearch.Trim();

                if (professionId == null)
                {
                    var data = await _unitOfWork.RepositoryR<FieldOperations>().FindBy(x => x.IsEnabled == true &&
                                                                                            (string.IsNullOrEmpty(keySearch) ||
                                                                                             x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                    List<FieldOperationsViewModel> result = new List<FieldOperationsViewModel>();
                    foreach (var item in data)
                    {
                        FieldOperationsViewModel itemResult = new FieldOperationsViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            ProfessionId = item.ProfessionId
                        };
                        result.Add(itemResult);
                    }
                    return JsonUtil.Success(result, "Success");
                }
                else
                {
                    var data = await _unitOfWork.RepositoryR<FieldOperations>().FindBy(x => x.IsEnabled == true && x.ProfessionId == professionId &&
                                                                                            (string.IsNullOrEmpty(keySearch) ||
                                                                                             x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                    List<FieldOperationsViewModel> result = new List<FieldOperationsViewModel>();
                    foreach (var item in data)
                    {
                        FieldOperationsViewModel itemResult = new FieldOperationsViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            ProfessionId = item.ProfessionId
                        };
                        result.Add(itemResult);
                    }
                    return JsonUtil.Success(result, "Success");
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
