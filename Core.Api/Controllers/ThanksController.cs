using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using Core.Business.ViewModels.Thanks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThanksController : BaseController
    {
        private readonly IThanksService _thanksService;

        public ThanksController(
            IThanksService thanksService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _thanksService = thanksService;
        }

        [HttpGet("GetListThanks")]
        public IActionResult GetListThanks(string keySearch, DateTime fromDate, DateTime toDate, string type, int? pageNum, int? pageSize)
        {
            return _thanksService.GetListThanks(keySearch, fromDate, toDate, type, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpPost("CreateThanks")]
        public async Task<IActionResult> CreateThanks([FromBody] ThanksViewModelCreate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _thanksService.CreateThanks(model, currentUserId);
        }

        [HttpGet("GetThanksReceiver")]
        public IActionResult GetThanksReceiver(int thanksId)
        {
            var currentUserId = GetCurrentUserId();
            return _thanksService.GetThanksReceiver(thanksId, currentUserId);
        }

        [HttpGet("GetThanksGive")]
        public IActionResult GetThanksGive(int thanksId)
        {
            var currentUserId = GetCurrentUserId();
            return _thanksService.GetThanksGive(thanksId,currentUserId);
        }
    }
}
