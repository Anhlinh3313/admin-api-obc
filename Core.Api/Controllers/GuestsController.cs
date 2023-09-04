using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using Core.Business.ViewModels.Guests;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GuestsController : BaseController
    {
        private readonly IGuestsService _guestsService;

        public GuestsController(
            IGuestsService guestsService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _guestsService = guestsService;
        }

        [HttpGet("GetListGuests")]
        public IActionResult GetListGuests(string keySearch, DateTime fromDate, DateTime toDate,int? statusId, int? chapterId, int? pageNum, int? pageSize)
        {
            return _guestsService.GetListGuests(keySearch, fromDate, toDate, statusId.GetValueOrDefault(0), chapterId.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetAllStatusGuests")]
        public IActionResult GetAllStatusGuests()
        {
            return _guestsService.GetAllStatusGuests();
        }

        [HttpPost("CreateGuests")]
        public async Task<IActionResult> CreateGuests([FromBody] GuestsViewModelCreate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _guestsService.CreateGuests(model, currentUserId);
        }

        [HttpGet("CheckInGuests")]
        public IActionResult CheckInGuests(int guestsId, int? checkIn)
        {
            return _guestsService.CheckInGuests(guestsId, checkIn.GetValueOrDefault(0));
        }

        [HttpGet("GetDetailGuests")]
        public IActionResult GetDetailGuests(int guestsId)
        {
            return _guestsService.GetDetailGuests(guestsId);
        }

        [HttpGet("GetListGuestsWithMeetingChapterId")]
        public IActionResult GetListGuestsWithMeetingChapterId(string keySearch, int meetingChapterId, int pageNum, int pageSize)
        {
            var currentUserId = GetCurrentUserId();
            return _guestsService.GetListGuestsWithMeetingChapterId(keySearch,meetingChapterId,pageNum,pageSize, currentUserId);
        }

        [HttpPost("CreateGoInstead")]
        public async Task<IActionResult> CreateGoInstead([FromBody] GuestsViewModelCreate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _guestsService.CreateGoInstead(model, currentUserId);
        }
        [HttpGet("GetDetailGoInstead")]
        public IActionResult GetDetailGoInstead(int goInsteadId)
        {
            return _guestsService.GetDetailGoInstead(goInsteadId);
        }
    }
}
