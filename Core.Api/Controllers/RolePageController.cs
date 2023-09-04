using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.RolePage;
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
    public class RolePageController : BaseController
    {
        private readonly IRolePageService _rolePageService;

        public RolePageController(
            IRolePageService rolePageService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _rolePageService = rolePageService;
        }

        [HttpPost("CreateOrUpdateRolePage")]
        public async Task<IActionResult> CreateOrUpdateRolePage([FromBody] RolePageViewModelCreate model)
        {
            return await _rolePageService.CreateOrUpdateRolePage(model);
        }

        [HttpGet("GetRolePage")]
        public IActionResult GetRolePage(int roleId)
        {
            return _rolePageService.GetRolePage(roleId);
        }

    }
}
