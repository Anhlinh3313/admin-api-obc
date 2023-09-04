using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.ParticipatingProvince;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParticipatingProvinceController : BaseController
    {
        private readonly IParticipatingProvinceService _provinceService;

        public ParticipatingProvinceController(
            IParticipatingProvinceService provinceService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _provinceService = provinceService;
        }
        // GET: api/<UserController>
        [HttpGet("GetListProvince")]
        public async Task<IActionResult> GetListProvince(string keySearch, int? pageNum, int? pageSize)
        {
            return await _provinceService.GetListProvinceAsync(keySearch, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailProvince")]
        public async Task<IActionResult> GetDetailProvince(int provinceId)
        {
            return await _provinceService.GetDetailProvinceAsync(provinceId);
        }

        // POST api/<UserController>
        [HttpPost("CreateProvince")]
        public async Task<IActionResult> CreateProvince([FromBody]ParticipatingProvinceViewModel model)
        {
            return await _provinceService.CreateProvinceAsync(model);
        }

        // PUT api/<UserController>/5
        [HttpPost("UpdateProvince")]
        public async Task<IActionResult> UpdateProvince([FromBody] ParticipatingProvinceViewModel model)
        {
            return await _provinceService.UpdateProvinceAsync(model);
        }

        [HttpGet("DeActiveProvince")]
        public async Task<IActionResult> DeActiveProvince(int provinceId)
        {
            return await _provinceService.DeActiveProvinceAsync(provinceId);
        }

        [HttpGet("DeEnableProvince")]
        public async Task<IActionResult> DeEnableProvince(int provinceId)
        {
            return await _provinceService.DeEnabledProvinceAsync(provinceId);
        }

        [HttpGet("DropdownProvince")]
        public async Task<IActionResult> DropdownProvince(string keySearch)
        {
            return await _provinceService.DropdownProvinceAsync(keySearch);
        }

        [HttpGet("GetProvinceAndRegion")]
        public async Task<IActionResult> GetProvinceAndRegion(int chapterId)
        {
            return await _provinceService.GetProvinceAndRegionWithChapterId(chapterId);
        }

        [HttpGet("GetProvinceTreeView")]
        public async Task<IActionResult> GetProvinceTreeView(string keySearch)
        {
            return await _provinceService.GetProvinceTreeViewAsync(keySearch);
        }
    }
}
