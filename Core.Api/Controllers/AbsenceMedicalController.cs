using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.AbsenceMedical;
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
    public class AbsenceMedicalController : BaseController
    {
        private readonly IAbsenceMedicalService _absenceMedicalService;

        public AbsenceMedicalController(
            IAbsenceMedicalService absenceMedicalService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _absenceMedicalService = absenceMedicalService;
        }

        [HttpPost("CreateAbsenceMedical")]
        public async Task<IActionResult> CreateAbsenceMedical([FromForm] AbsenceMedicalViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            return await _absenceMedicalService.CreateAbsenceMedical(model, currentUserId);
        }
        [HttpGet("GetDetailAbsenceMedical")]
        public IActionResult GetDetailAbsenceMedical(int absenceMedicalId)
        {
            var currentUserId = GetCurrentUserId();
            return _absenceMedicalService.GetDetailAbsenceMedical(absenceMedicalId, currentUserId);
        }
    }
}
