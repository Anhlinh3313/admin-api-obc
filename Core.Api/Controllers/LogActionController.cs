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
    [Authorize]
    public class LogActionController : BaseController
    {
        private readonly ILogActionService _logActionService;

        public LogActionController(
            ILogActionService logActionService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _logActionService = logActionService;
        }

        [HttpGet("GetListLogAction")]
        public async Task<IActionResult> GetListLogAction(int customerId)
        {
            return await _logActionService.GetListLogAction(customerId);
        }

        [HttpGet("CreateNoteLogAction")]
        public async Task<IActionResult> CreateNoteLogAction(int customerId, string note)
        {
            var currentUserId = GetCurrentUserId();
            return await _logActionService.CreateLogAction(currentUserId, null, null, note, customerId);
        }
    }
}
