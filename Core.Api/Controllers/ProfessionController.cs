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
    public class ProfessionController : BaseController
    {
        private readonly IProfessionService _professionService;

        public ProfessionController(
            IProfessionService professionService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _professionService = professionService;
        }
        [AllowAnonymous]
        [HttpGet("DropdownProfession")]
        public async Task<IActionResult> DropdownProfession(string keySearch, string language)
        {
            return await _professionService.DropdownProfessionAsync(keySearch, language);
        }

        [HttpGet("DropdownProfessionFieldOperations")]
        public async Task<IActionResult> DropdownProfessionFieldOperations(string keySearch)
        {
            return await _professionService.DropdownProfessionFieldOperationsAsync(keySearch);
        }

        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
