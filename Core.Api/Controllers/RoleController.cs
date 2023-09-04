using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Role;
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
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(
            IRoleService roleService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _roleService = roleService;
        }
        // GET: api/<UserController>
        [HttpGet("GetListRole")]
        public IActionResult GetListRole(string keySearch, int? pageNum, int? pageSize)
        {
            return _roleService.GetListRoleAsync(keySearch,pageNum.GetValueOrDefault(1),pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetRoleDetail")]
        public async Task<IActionResult> GetRoleDetail(int roleId)
        {
            return await _roleService.GetRoleDetailAsync(roleId);
        }

        // POST api/<UserController>
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody]RoleViewModelCreate model)
        {
            return await _roleService.CreateRoleAsync(model);
        }

        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleViewModelCreate model)
        {
            return await _roleService.UpdateRoleAsync(model);
        }

        [HttpGet("DeEnabledRole")]
        public async Task<IActionResult> DeEnabledRole(int roleId)
        {
            return await _roleService.DeEnabledAsync(roleId);
        }

        [HttpGet("GetAllRoleType")]
        public async Task<IActionResult> GetAllRoleType()
        {
            return await _roleService.GetAllRoleType();
        }

        [HttpGet("GetRoleWithModuleName")]
        public IActionResult GetRoleWithModuleName(string moduleName)
        {
            return _roleService.GetRoleWithModuleName(moduleName);
        }
    }
}
