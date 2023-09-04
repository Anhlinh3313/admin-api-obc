using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Business.Services
{
    public class ProvinceService : BaseService, IProvinceService
    {
        public IGeneralService _iGeneralRawService { get; set; }
        public ProvinceService(Microsoft.Extensions.Logging.ILogger<dynamic> logger, 
            IOptions<AppSettings> optionsAccessor, 
            IUnitOfWork unitOfWork,
            IGeneralService iGeneralRawService) : base(logger, optionsAccessor, unitOfWork)
        {
            _iGeneralRawService = iGeneralRawService;
        }

        public async Task<JsonResult> DropdownProvinceAsync(string keySearch)
        {
            try
            {
                var listProvince = await _unitOfWork.RepositoryR<Provinces>()
                    .FindBy(x => x.IsEnabled == true &&
                                 (string.IsNullOrEmpty(keySearch) ||
                                  x.Name.ToLower().Contains(keySearch.Trim().ToLower()))).ToListAsync();
                return JsonUtil.Success(listProvince);
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
