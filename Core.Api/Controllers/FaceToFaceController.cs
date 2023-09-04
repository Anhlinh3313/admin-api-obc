using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using Core.Business.ViewModels.FaceToFace;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FaceToFaceController : BaseController
    {
        private readonly IFaceToFaceService _faceToFaceService;

        public FaceToFaceController(
            IFaceToFaceService faceToFaceService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _faceToFaceService = faceToFaceService;
        }

        [HttpGet("GetListFaceToFace")]
        public IActionResult GetListFaceToFace(string keySearch, DateTime fromDate, DateTime toDate, string type, int? statusId, int? pageNum, int? pageSize)
        {
            return _faceToFaceService.GetListFaceToFace(keySearch, fromDate, toDate, type, statusId.GetValueOrDefault(0), pageNum.GetValueOrDefault(1), pageSize.GetValueOrDefault(10));
        }

        [HttpGet("GetAllStatusFaceToFace")]
        public IActionResult GetAllStatusFaceToFace()
        {
            return _faceToFaceService.GetAllStatusFaceToFace();
        }

        [HttpPost("CreateFaceToFace")]
        public async Task<IActionResult> CreateFaceToFace([FromBody] FaceToFaceViewModelCreate model)
        {
            var currentUserId = GetCurrentUserId();
            return await _faceToFaceService.CreateFaceToFace(model, currentUserId);
        }

        [HttpGet("GetFaceToFaceReceiver")]
        public IActionResult GetFaceToFaceReceiver(int faceToFaceId)
        {
            var currentUserId = GetCurrentUserId();
            return _faceToFaceService.GetFaceToFaceReceiver(currentUserId,faceToFaceId);
        }

        [HttpGet("GetFaceToFaceGive")]
        public IActionResult GetFaceToFaceGive(int faceToFaceId)
        {
            var currentUserId = GetCurrentUserId();
            return _faceToFaceService.GetFaceToFaceGive(currentUserId,faceToFaceId);
        }

        [HttpGet("AcceptOrCancel")]
        public IActionResult AcceptOrCancel(int faceToFaceId, int? confirm, string reasonCancel)
        {
            var currentUserId = GetCurrentUserId();
            return _faceToFaceService.AcceptOrCancel(faceToFaceId, confirm.GetValueOrDefault(0), reasonCancel, currentUserId);
        }

        [HttpPost("UploadFileAfterMeeting")]
        public async Task<IActionResult> UploadFileAfterMeeting([FromForm] UploadFileFaceToFaceViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            return await _faceToFaceService.UploadFileAfterMeeting(model, currentUserId);
        }
    }
}
