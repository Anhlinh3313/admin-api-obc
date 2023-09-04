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
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;

        public PageController(
            IPageService pageService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _pageService = pageService;
        }
       
        [HttpGet("GetListPage")]
        public IActionResult GetListPage()
        {
            var currentUserId = GetCurrentUserId();
            return _pageService.GetListPage(currentUserId);
        }

        [HttpGet("CheckPermission")]
        public IActionResult CheckPermission(string pathName)
        {
            var currentUserId = GetCurrentUserId();
            return _pageService.CheckPermission(pathName ,currentUserId);
        }

        [HttpGet("CheckPermissionMobile")]
        public IActionResult CheckPermissionMobile(string pageName)
        {
            var currentUserId = GetCurrentUserId();
            return _pageService.CheckPermissionMobile(pageName, currentUserId);
        }

    }
}
