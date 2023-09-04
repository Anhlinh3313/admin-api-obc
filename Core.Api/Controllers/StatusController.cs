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
    public class StatusController : BaseController
    {
        private readonly IStatusService _statusService;

        public StatusController(
            IStatusService statusService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _statusService = statusService;
        }

        [HttpGet("GetAllStatusPendingActiveAndActive")]
        public async Task<IActionResult> GetAllStatusPendingActiveAndActive()
        {
            return await _statusService.GetAllStatusTransactionAsync();
        }

        [HttpGet("GetStatusCustomerInformation")]
        public async Task<IActionResult> GetStatusCustomerInformation()
        {
            return await _statusService.GetStatusCustomerInformationAsync();
        }
    }
}
