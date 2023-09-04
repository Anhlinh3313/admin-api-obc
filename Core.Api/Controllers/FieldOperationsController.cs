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
    public class FieldOperationsController : BaseController
    {
        private readonly IFieldOperationsService _fieldOperationsService;

        public FieldOperationsController(
            IFieldOperationsService fieldOperationsService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _fieldOperationsService = fieldOperationsService;
        }
        
        [HttpGet("DropdownFieldOperations")]
        public async Task<IActionResult> DropdownFieldOperations(string keySearch, int? professionId)
        {
            var currentUserId = GetCurrentUserId();
            return await _fieldOperationsService.GetAllFieldOperationsAsync(keySearch, professionId, currentUserId);
        }

        [HttpGet("GetAllFieldOperationsWithProfessionId")]
        public async Task<IActionResult> GetAllFieldOperationsWithProfessionId(string keySearch, int? professionId)
        {
            return await _fieldOperationsService.GetAllFieldOperationsInWebAsync(keySearch, professionId);
        }

    }
}
