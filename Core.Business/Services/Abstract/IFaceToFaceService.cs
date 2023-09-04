using System;
using System.Threading.Tasks;
using Core.Business.ViewModels.FaceToFace;
using Core.Business.ViewModels.ParticipatingProvince;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IFaceToFaceService
    {
        JsonResult GetListFaceToFace(string keySearch, DateTime fromDate, DateTime toDate, string type, int statusId, int pageNum, int pageSize);
        JsonResult GetAllStatusFaceToFace();
        Task<JsonResult> CreateFaceToFace(FaceToFaceViewModelCreate model, int customerId);
        JsonResult GetFaceToFaceReceiver(int customerId, int faceToFaceId);
        JsonResult GetFaceToFaceGive(int customerId, int faceToFaceId);
        JsonResult AcceptOrCancel(int faceToFaceId, int confirm, string reasonCancel, int customerId);
        Task<JsonResult> UploadFileAfterMeeting(UploadFileFaceToFaceViewModel model, int customerId);
    }
}
