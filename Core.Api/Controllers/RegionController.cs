using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Region;
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
    public class RegionController : BaseController
    {
        private readonly IRegionService _regionService;

        public RegionController(
            IRegionService regionService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _regionService = regionService;
        }
        // GET: api/<UserController>
        [HttpGet("GetListRegion")]
        public IActionResult GetListRegion(string keySearch, string province, int? pageNum, int? pageSize)
        {
            return _regionService.GetListRegionAsync(keySearch, province, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailRegion")]
        public async Task<IActionResult> GetDetailRegion(int regionId)
        {
            return await _regionService.GetDetailRegionAsync(regionId);
        }

        // POST api/<UserController>
        [HttpPost("CreateRegion")]
        public async Task<IActionResult> CreateRegion([FromBody]RegionViewModelCreate model)
        {
            return await _regionService.CreateRegionAsync(model);
        }

        // PUT api/<UserController>/5
        [HttpPost("UpdateRegion")]
        public async Task<IActionResult> UpdateRegion([FromBody] RegionViewModelCreate model)
        {
            return await _regionService.UpdateRegionAsync(model);
        }

        [HttpGet("DeActiveRegion")]
        public async Task<IActionResult> DeActiveRegion(int regionId)
        {
            return await _regionService.DeActiveRegionAsync(regionId);
        }

        [HttpGet("DeEnableRegion")]
        public async Task<IActionResult> DeEnableRegion(int regionId)
        {
            return await _regionService.DeEnabledRegionAsync(regionId);
        }

        [HttpGet("DropdownRegion")]
        public async Task<IActionResult> DropdownRegion(string keySearch, string province)
        {
            return await _regionService.GetAllRegionAsync(keySearch, province);
        }

        [HttpGet("GetAllRegion")]
        public IActionResult GetAllRegion(string keySearch)
        {
            return  _regionService.GetAllRegion(keySearch);
        }
    }
}
