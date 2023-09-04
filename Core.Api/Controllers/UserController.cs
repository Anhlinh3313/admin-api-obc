using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels;
using Core.Business.ViewModels.User;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public UserController(
            IUserService userService,
            IAccountService accountService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _userService = userService;
            _accountService = accountService;
        }
        // GET: api/<UserController>
        [HttpGet("GetListUser")]
        public IActionResult GetListUser(string keySearch, int? pageNum, int? pageSize)
        {
            return _userService.GetListUser(keySearch, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailUser")]
        public async Task<IActionResult> GetDetailUser(int userId)
        {
            return await _userService.GetDetailUserAsync(userId);
        }

        // POST api/<UserController>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel model)
        {
            return await _userService.CreateUserAsync(model);
        }

        // PUT api/<UserController>/5
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            return await _userService.UpdateUserAsync(model);
        }

        [HttpGet("DeEnableUser")]
        public async Task<IActionResult> DeEnableUser(int userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == userId) return JsonUtil.Error(ValidatorMessage.Account.NoDeEnableCurrent);
            return await _userService.DeEnabledUserAsync(userId);
        }

        [HttpGet("DeActiveUser")]
        public async Task<IActionResult> DeActiveUser(int userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == userId) return JsonUtil.Error(ValidatorMessage.Account.NoDeActiveCurrent);
            return await _userService.DeActiveUserAsync(userId);
        }

        [HttpGet("GetAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            return await _userService.GetAllRole();
        }

        [HttpGet("CheckUser")]
        public IActionResult CheckUser()
        {
            var currentUserId = GetCurrentUserId();
            return _userService.CheckUser(currentUserId);
        }
        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
