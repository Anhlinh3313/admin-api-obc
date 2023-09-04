using System.Collections.Generic;
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
    public class HomeController : BaseController
    {
        private readonly IHomeService _homeService;

        public HomeController(
            IHomeService homeService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _homeService = homeService;
        }
       
        [HttpGet("GetTypeSearchHomeMobile")]
        public IActionResult GetTypeSearchHomeMobile()
        {
            var currentUserId = GetCurrentUserId();
            return _homeService.GetTypeSearchHomeMobile(currentUserId);
        }

        [HttpGet("GetListSearchHomeMobile")]
        public IActionResult GetListSearchHomeMobile(string keySearch, string typeId, int? pageNum, int? pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _homeService.GetListSearchHomeMobile(keySearch, typeId, currentUserId, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
