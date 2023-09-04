using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Data.Abstract;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure.Helper;
using Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClassificationsController : BaseController
    {
        private readonly IClassificationsService _classificationsService;

        public ClassificationsController(
            IClassificationsService classificationsService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _classificationsService = classificationsService;
        }
       
        [HttpGet("GetClassifications")]
        public IActionResult GetClassifications()
        {
            return _classificationsService.GetClassifications();
        }

        [HttpGet("GetClassificationsNotInChapter")]
        public IActionResult GetClassificationsNotInChapter(int chapterId)
        {
            return _classificationsService.GetClassificationsNotInChapter(chapterId);
        }

        [HttpGet("DownloadExcelClassifications")]
        public IActionResult DownloadExcelClassifications()
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "Classifications.xlsx";
            var userFullName = GetCurrentUser().FullName;
            var time = DateTime.Now.ToString("HH:mm dd-MM-yyyy");

            var data = _classificationsService.GetClassifications();
            var success = data.Value.GetType().GetProperty("isSuccess")?.GetValue(data.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return data;
            }
            var value = data.Value.GetType().GetProperty("data")?.GetValue(data.Value, null);
            var list = (List<Proc_GetClassifications>)value;
            var dataAdd = list.ToArray();
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                        workbook.Worksheets.Add("Classifications");

                    worksheet.Style.Font.SetFontName("sans-serif").Font.FontSize = 10;

                    worksheet.Cell("A1").Value = "Chapter ► Classifications";
                    worksheet.Range("A1:E1").Merge();

                    worksheet.Cell("A2").Value = "Người dùng xuất báo cáo";
                    worksheet.Cell("A3").Value = userFullName;

                    worksheet.Cell("B2").Value = "Thời gian xuất báo cáo";
                    worksheet.Cell("B3").Value = time;

                    worksheet.Cell("C2").Value = "Quốc gia";
                    worksheet.Range("C2:D2").Merge();
                    worksheet.Cell("C3").Value = "Việt Nam";
                    worksheet.Range("C3:D3").Merge();

                    //worksheet.Cell("H2").Value = "Vùng";
                    //worksheet.Cell("H3").Value = region.Name;

                    //worksheet.Cell("I2").Value = "Chapter";
                    //worksheet.Cell("I3").Value = chapter.Name;

                    worksheet.Cell("A4").Value = "Những ngành nghề đang hoạt động";
                    worksheet.Range("A4:C4").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("D4").Value = "Số lượng chapter có ngành nghề này";
                    worksheet.Range("D4:F4").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < dataAdd.Length; i++)
                    {
                        var numCell = 5 + i;
                        worksheet.Cell("A" + numCell).Value = dataAdd[i].ProfessionName;
                        worksheet.Range("A" + numCell + ":C" + numCell).Merge().Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D" + numCell).Value = dataAdd[i].CountChapter;
                        worksheet.Range("D" + numCell + ":F" + numCell).Merge().Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Left;
                    }
                    worksheet.Columns().AdjustToContents();
                    //worksheet.Rows().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }

        [HttpGet("DownloadExcelClassificationsNotInChapter")]
        public IActionResult DownloadExcelClassificationsNotInChapter(int chapterId)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "ClassificationsNotInChapter.xlsx";
            var userFullName = GetCurrentUser().FullName;
            var time = DateTime.Now.ToString("HH:mm dd-MM-yyyy");
            var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);
            var region = _unitOfWork.RepositoryR<Region>().GetSingle(x => x.Id == chapter.RegionId);

            var data = _classificationsService.GetClassificationsNotInChapter(chapterId);
            var success = data.Value.GetType().GetProperty("isSuccess")?.GetValue(data.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return data;
            }
            var value = data.Value.GetType().GetProperty("data")?.GetValue(data.Value, null);
            var list = (List<Proc_GetClassificationsNotInChapter>)value;
            var dataAdd = list.ToArray();
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                        workbook.Worksheets.Add("Classifications");

                    worksheet.Style.Font.SetFontName("sans-serif").Font.FontSize = 10;

                    worksheet.Cell("A1").Value = "Chapter ► Classifications Not In Chapter";
                    worksheet.Range("A1:E1").Merge();

                    worksheet.Cell("A2").Value = "Người dùng xuất báo cáo";
                    worksheet.Cell("A3").Value = userFullName;

                    worksheet.Cell("B2").Value = "Thời gian xuất báo cáo";
                    worksheet.Cell("B3").Value = time;

                    worksheet.Cell("C2").Value = "Quốc gia";
                    worksheet.Range("C2:D2").Merge();
                    worksheet.Cell("C3").Value = "Việt Nam";
                    worksheet.Range("C3:D3").Merge();

                    //worksheet.Cell("H2").Value = "Vùng";
                    //worksheet.Cell("H3").Value = region.Name;

                    //worksheet.Cell("I2").Value = "Chapter";
                    //worksheet.Cell("I3").Value = chapter.Name;

                    worksheet.Cell("A4").Value = "Thông số";
                    worksheet.Range("A4:F4").Merge();

                    worksheet.Cell("A5").Value = "Vùng:";
                    worksheet.Range("A5:C5").Merge();
                    worksheet.Cell("D5").Value = region.Name;
                    worksheet.Range("D5:F5").Merge();

                    worksheet.Cell("A6").Value = "Chapter:";
                    worksheet.Range("A6:C6").Merge();
                    worksheet.Cell("D6").Value = chapter.Name;
                    worksheet.Range("D6:F6").Merge();

                    worksheet.Cell("A7").Value = "Những ngành nghề không có trong chapter của bạn";
                    worksheet.Range("A7:C7").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("D7").Value = "Số lượng chapter có ngành nghề này";
                    worksheet.Range("D7:F7").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (int i = 0; i < dataAdd.Length; i++)
                    {
                        var numCell = 8 + i;
                        worksheet.Cell("A" + numCell).Value = dataAdd[i].ProfessionName;
                        worksheet.Range("A" + numCell + ":C" + numCell).Merge().Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("D" + numCell).Value = dataAdd[i].CountChapter;
                        worksheet.Range("D" + numCell + ":F" + numCell).Merge().Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Left;
                    }

                    worksheet.Columns().AdjustToContents();
                    //worksheet.Rows().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonUtil.Error(ex.Message);
            }
        }
    }
}
