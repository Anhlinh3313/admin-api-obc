using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.FAQs;
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
    public class FAQsController : BaseController
    {
        private readonly IFAQsService _faqService;

        public FAQsController(
            IFAQsService faqService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _faqService = faqService;
        }
        // GET: api/<UserController>
        [HttpGet("GetListFAQs")]
        public async Task<IActionResult> GetListFAQs(string keySearch, int? pageNum, int? pageSize)
        {
            return await _faqService.GetListFAQsAsync(keySearch, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailFAQs")]
        public async Task<IActionResult> GetDetailFAQs(int faqId)
        {
            return await _faqService.GetDetailFAQsAsync(faqId);
        }

        // POST api/<UserController>
        [HttpPost("CreateFAQs")]
        public async Task<IActionResult> CreateFAQs([FromBody]FAQsViewModel model)
        {
            return await _faqService.CreateFAQsAsync(model);
        }

        // PUT api/<UserController>/5
        [HttpPost("UpdateFAQs")]
        public async Task<IActionResult> UpdateFAQs([FromBody] FAQsViewModel model)
        {
            return await _faqService.UpdateFAQsAsync(model);
        }

        [HttpGet("DeActiveFAQs")]
        public async Task<IActionResult> DeActiveFAQs(int faqId)
        {
            return await _faqService.DeActiveFAQsAsync(faqId);
        }

        [HttpGet("DeEnableFAQs")]
        public async Task<IActionResult> DeEnableFAQs(int faqId)
        {
            return await _faqService.DeEnabledFAQsAsync(faqId);
        }

        [HttpGet("GetListFAQsMobile")]
        public async Task<IActionResult> GetListFAQsMobile(string keySearch, int? pageNum, int? pageSize)
        {
            return await _faqService.GetListFAQsMobile(keySearch, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetAllFAQsMobile")]
        public async Task<IActionResult> GetAllFAQsMobile(string keySearch)
        {
            return await _faqService.GetAllFAQsMobile(keySearch);
        }
    }
}
