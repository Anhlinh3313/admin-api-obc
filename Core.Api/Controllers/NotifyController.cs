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
    public class NotifyController : BaseController
    {
        private readonly INotifyService _notifyService;

        public NotifyController(
            INotifyService notifyService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _notifyService = notifyService;
        }
       
        [HttpGet("SeenNotify")]
        public async Task<IActionResult> SeenNotify(int? notifyId)
        {
            var currentUserId = GetCurrentUserId();
            return await _notifyService.SeenNotify(notifyId, currentUserId);
        }
        [HttpGet("GetListNotify")]
        public IActionResult GetListNotify(int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _notifyService.GetListNotify(currentUserId, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("SumUnSeenNotify")]
        public IActionResult SumUnSeenNotify(int customerId)
        {
            return  _notifyService.SumUnSeenNotify(customerId);
        }
    }
}
