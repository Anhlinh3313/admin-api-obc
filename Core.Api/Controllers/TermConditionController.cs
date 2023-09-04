using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.TermCondition;
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
    public class TermConditionController : BaseController
    {
        private readonly ITermConditionService _termConditionService;

        public TermConditionController(
            ITermConditionService termConditionService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _termConditionService = termConditionService;
        }
        // GET: api/<UserController>
        [HttpGet("GetTermCondition")]
        public async Task<IActionResult> GetTermCondition()
        {
            return await _termConditionService.GetListTermConditionAsync();
        }

        // POST api/<UserController>
        [HttpPost("CreateOrUpdateTermCondition")]
        public async Task<IActionResult> CreateOrUpdateTermCondition([FromBody]TermConditionViewModel model)
        {
            return await _termConditionService.CreateTermConditionAsync(model);
        }
    }
}
