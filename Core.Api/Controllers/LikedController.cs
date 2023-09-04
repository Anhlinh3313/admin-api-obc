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
    public class LikedController : BaseController
    {
        private readonly IEventService _eventService;

        public LikedController(
            IEventService eventService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
        ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _eventService = eventService;
        }

        [HttpGet("GetListLikedEventAndCourse")]
        public IActionResult GetListLikedEventAndCourse(string keySearch, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _eventService.GetListLikedEventAndCourse(keySearch,currentUserId, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }
    }
}
