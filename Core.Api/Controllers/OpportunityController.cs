using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using Core.Business.ViewModels.Opportunity;
using Core.Business.ViewModels.Role;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OpportunityController : BaseController
    {
        private readonly IOpportunityService _opportunityService;

        public OpportunityController(
            IOpportunityService opportunityService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _opportunityService = opportunityService;
        }

        [HttpGet("GetListOpportunity")]
        public IActionResult GetListOpportunity(string keySearch, DateTime fromDate, DateTime toDate, string type, int? pageNum, int? pageSize)
        {
            return _opportunityService.GetListOpportunity(keySearch, fromDate, toDate, type, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("ChangeStatusOpportunity")]
        public IActionResult ChangeStatusOpportunity(int opportunityId, int statusId, string note)
        {
            var currentUserId = GetCurrentUserId();
            return _opportunityService.ChangeStatusOpportunity(opportunityId, statusId, note, currentUserId);
        }

        [HttpPost("CreateOpportunity")]
        public async Task<IActionResult> CreateOpportunity([FromBody] OpportunityViewModelCreate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _opportunityService.CreateOpportunity(model, currentUserId);
        }

        [HttpGet("GetOpportunityReceiver")]
        public IActionResult GetOpportunityReceiver(int opportunityId)
        {
            var currentUserId = GetCurrentUserId();
            return _opportunityService.GetOpportunityReceiver(currentUserId, opportunityId);
        }

        [HttpGet("GetOpportunityGive")]
        public IActionResult GetOpportunityGive(int opportunityId)
        {
            var currentUserId = GetCurrentUserId();
            return _opportunityService.GetOpportunityGive(currentUserId, opportunityId);
        }

        [HttpGet("GetStatusOpportunity")]
        public IActionResult GetStatusOpportunity()
        {
            var currentUserId = GetCurrentUserId();
            return _opportunityService.GetStatusOpportunity(currentUserId);
        }
    }
}
