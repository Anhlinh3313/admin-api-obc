using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.MembershipAction;
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
    public class MembershipActionController : BaseController
    {
        private readonly IMembershipActionService _membershipActionService;

        public MembershipActionController(
            IMembershipActionService membershipActionService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _membershipActionService = membershipActionService;
        }

        [HttpGet("GetMembershipActionWithCustomerId")]
        public async Task<IActionResult> GetMembershipActionWithCustomerId(int id)
        {
            return await _membershipActionService.GetMembershipActionWithCustomerId(id);
        }

        [HttpGet("GetMembershipWithExpenseId")]
        public async Task<IActionResult> GetMembershipWithExpenseId(int? id)
        {
            return await _membershipActionService.GetMembershipWithExpenseId(id.GetValueOrDefault(0));
        }

        [HttpPost("CreateOrUpdateMembershipAction")]
        public async Task<IActionResult> CreateOrUpdateMembershipAction([FromBody] MembershipActionViewModel model)
        {
            return await _membershipActionService.CreateMembershipAction(model);
        }

    }
}
