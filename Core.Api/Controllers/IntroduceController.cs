using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Introduce;
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
    public class IntroduceController : BaseController
    {
        private readonly IIntroduceService _introduceService;

        public IntroduceController(
            IIntroduceService introduceService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _introduceService = introduceService;
        }
        // GET: api/<UserController>
        [HttpGet("GetIntroduce")]
        public async Task<IActionResult> GetIntroduce()
        {
            return await _introduceService.GetListIntroduceAsync();
        }

        // POST api/<UserController>
        [HttpPost("CreateOrUpdateIntroduce")]
        public async Task<IActionResult> CreateOrUpdateIntroduce([FromBody]IntroduceViewModel model)
        {
            return await _introduceService.CreateIntroduceAsync(model);
        }
    }
}
