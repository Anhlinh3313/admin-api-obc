using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Chapter;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Utils;
using Core.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChapterController : BaseController
    {
        private readonly IChapterService _chapterService;
        private readonly IEventService _eventService;

        public ChapterController(
            IChapterService chapterService,
            IEventService eventService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _chapterService = chapterService;
            _eventService = eventService;
        }

        // GET: api/<UserController>
        [HttpGet("GetListChapter")]
        public IActionResult GetListChapter(string keySearch, string province, string region, int? pageNum, int? pageSize)
        {
            return _chapterService.GetListChapterAsync(keySearch, province, region, pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        // GET api/<UserController>/5
        [HttpGet("GetDetailChapter")]
        public async Task<IActionResult> GetDetailChapter(int chapterId)
        {
            return await _chapterService.GetDetailChapterAsync(chapterId);
        }

        [HttpGet("GetChapterInformation")]
        public IActionResult GetChapterInformation(int chapterId)
        {
            return _chapterService.GetChapterInformation(chapterId);
        }

        // POST api/<UserController>
        [HttpPost("CreateChapter")]
        public async Task<IActionResult> CreateChapter([FromBody]ChapterViewModelCreate model)
        {
            var chapter = await _chapterService.CreateChapterAsync(model);
            var success = chapter.Value.GetType().GetProperty("isSuccess")?.GetValue(chapter.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return chapter;
            }
            var value = chapter.Value.GetType().GetProperty("data")?.GetValue(chapter.Value, null);
            var link = (ChapterViewModelReturn)value;
            var text = "ChapterId: " + link.ChapterId +
                       "\nTỉnh: " + link.ProvinceName +
                       "\nVùng: " + link.RegionName +
                       "\nChapter: " + link.ChapterName;
            var image = _eventService.GenerateByteArray(text);
            var files = File(image, "image/png");
            var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                files.ContentType);
            streamResult.FileDownloadName = files.FileDownloadName;
            return _chapterService.GenerateQrCodeCustomer(streamResult, link.ChapterId);
        }

        // PUT api/<UserController>/5
        [HttpPost("UpdateChapter")]
        public async Task<IActionResult> UpdateChapter([FromBody] ChapterViewModelCreate model)
        {
            var chapter = await _chapterService.UpdateChapterAsync(model);
            var success = chapter.Value.GetType().GetProperty("isSuccess")?.GetValue(chapter.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return chapter;
            }
            var value = chapter.Value.GetType().GetProperty("data")?.GetValue(chapter.Value, null);
            var link = (ChapterViewModelReturn)value;
            var text = "ChapterId: " + link.ChapterId +
                        "\nTỉnh: " + link.ProvinceName +
                       "\nVùng: " + link.RegionName +
                       "\nChapter: " + link.ChapterName;
            var image = _eventService.GenerateByteArray(text);
            var files = File(image, "image/png");
            var streamResult = new FileStreamResult(new MemoryStream(files.FileContents),
                files.ContentType);
            streamResult.FileDownloadName = files.FileDownloadName;
            return _chapterService.GenerateQrCodeCustomer(streamResult, link.ChapterId);
        }

        [HttpGet("DeActiveChapter")]
        public async Task<IActionResult> DeActiveChapter(int chapterId)
        {
            return await _chapterService.DeActiveChapterAsync(chapterId);
        }

        [HttpGet("DeEnableChapter")]
        public async Task<IActionResult> DeEnableChapter(int chapterId)
        {
            return await _chapterService.DeEnabledChapterAsync(chapterId);
        }

        [HttpGet("DropdownChapter")]
        public async Task<IActionResult> DropdownChapter(int? regionId, string keySearch)
        {
            if (regionId == null) return JsonUtil.Error("Vui lòng chọn vùng");
            return await _chapterService.GetChapterWithRegionIdAsync(regionId.GetValueOrDefault(1), keySearch);
        }

        [HttpGet("DropdownChapterMobile")]
        public async Task<IActionResult> DropdownChapterMobile(string keySearch)
        {
            return await _chapterService.GetChapterMobile(keySearch);
        }

        [HttpGet("CheckFieldOperationUnique")]
        public async Task<IActionResult> CheckFieldOperationUnique(int fieldOperationId, int chapterId)
        {
            return await _chapterService.CheckFieldOperationUnique(fieldOperationId, chapterId);
        }

        [HttpGet("GetDetailMemberChapter")]
        public IActionResult GetDetailMemberChapter(int businessId)
        {
            return _chapterService.GetDetailMemberChapter(businessId);
        }

        [HttpGet("GetAllChapter")]
        public async Task<IActionResult> GetAllChapter()
        {
            return await _chapterService.GetChapterMobile(null);
        }


        [HttpGet("GetInformationChapterInGuests")]
        public IActionResult GetInformationChapterInGuests()
        {
            var currentUserId = GetCurrentUserId();
            return _chapterService.GetInformationChapterInGuests(currentUserId);
        }
    }
}
