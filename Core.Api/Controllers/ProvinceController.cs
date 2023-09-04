using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProvinceController : BaseController
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(
            IProvinceService provinceService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _provinceService = provinceService;
        }

        [HttpGet("DropdownProvince")]
        public async Task<IActionResult> DropdownProvince(string keySearch)
        {
            return await _provinceService.DropdownProvinceAsync(keySearch);
        }
    }
}
