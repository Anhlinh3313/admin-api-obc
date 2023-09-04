using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.ChapterMemberCompany;
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
    public class ChapterMemberCompanyController : BaseController
    {
        private readonly IChapterMemberCompanyService _chapterMemberCompanyService;

        public ChapterMemberCompanyController(
            IChapterMemberCompanyService chapterMemberCompanyService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _chapterMemberCompanyService = chapterMemberCompanyService;
        }
       
        [HttpGet("GetChapterMemberCompany")]
        public IActionResult GetChapterMemberCompany(int chapterId, DateTime? dateSearch)
        {
            return _chapterMemberCompanyService.GetChapterMemberCompanyWeb(chapterId,dateSearch);
        }

        [HttpGet("DownloadExcelChapterMemberCompany")]
        public IActionResult DownloadExcelChapterMemberCompany(int chapterId, DateTime? dateSearch)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "ChapterMemberCompany.xlsx";
            var userFullName = GetCurrentUser().FullName;
            var time = DateTime.Now.ToString("HH:mm dd-MM-yyyy");
            var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);
            //var region = _unitOfWork.RepositoryR<Region>().GetSingle(x => x.Id == chapter.RegionId);

            var data = _chapterMemberCompanyService.GetChapterMemberCompanyWeb(chapterId, dateSearch);
            var success = data.Value.GetType().GetProperty("isSuccess")?.GetValue(data.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return data;
            }
            var value = data.Value.GetType().GetProperty("data")?.GetValue(data.Value, null);
            var list = (List<ChapterMemberCompanyViewModel>)value;
            var dataAdd = list.ToArray();
            int numCell = 7;
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                        workbook.Worksheets.Add("ChapterMemberCompany");

                    worksheet.Style.Font.SetFontName("sans-serif").Font.FontSize = 10;

                    worksheet.Cell("A1").Value = "Chapter Member Company";
                    worksheet.Range("A1:H1").Merge();

                    worksheet.Cell("A2").Value = "Người dùng xuất báo cáo";
                    worksheet.Range("A2:C2").Merge();
                    worksheet.Cell("A3").Value = userFullName;
                    worksheet.Range("A3:C3").Merge();

                    worksheet.Cell("D2").Value = "Thời gian xuất báo cáo";
                    worksheet.Range("D2:E2").Merge();
                    worksheet.Cell("D3").Value = time;
                    worksheet.Range("D3:E3").Merge();

                    worksheet.Cell("F2").Value = "Quốc gia";
                    worksheet.Range("F2:G2").Merge();
                    worksheet.Cell("F3").Value = "Việt Nam";
                    worksheet.Range("F3:G3").Merge();

                    //worksheet.Cell("H2").Value = "Vùng";
                    //worksheet.Cell("H3").Value = region.Name;

                    //worksheet.Cell("I2").Value = "Chapter";
                    //worksheet.Cell("I3").Value = chapter.Name;

                    worksheet.Cell("A4").Value = "Báo cáo về ghi chú buổi họp cho:";
                    worksheet.Range("A4:I4").Merge();

                    worksheet.Cell("A5").Value = "Chapter:";
                    worksheet.Range("A5:F5").Merge();
                    worksheet.Cell("G5").Value = chapter.Name;
                    worksheet.Range("G5:I5").Merge();

                    worksheet.Cell("A6").Value = "Họ và tên";
                    worksheet.Range("A6:B6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("C6").Value = "Công ty";
                    worksheet.Range("C6:D6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("E6").Value = "Ghi chú";
                    worksheet.Range("E6:I6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < dataAdd.Length; i++)
                    {
                        for (int j = 0; j < dataAdd[i].Note.Count; j++)
                        {
                            worksheet.Cell("E" + numCell).Value = dataAdd[i].Note[j];
                            worksheet.Range("E" + numCell + ":I" + numCell).Merge();
                            numCell += 1;
                        }

                        var numCellStart = numCell - dataAdd[i].Note.Count;
                        var numCellEnd = numCell - 1;
                        worksheet.Cell("A" + numCellStart).Value = dataAdd[i].FullName;
                        worksheet.Range("A" + numCellStart + ":B" + numCellEnd).Merge().Style.Alignment.Vertical =
                            XLAlignmentVerticalValues.Center;
                        worksheet.Range("A" + numCellStart + ":B" + numCellEnd).Merge().Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Left;

                        worksheet.Cell("C" + numCellStart).Value = dataAdd[i].BusinessName;
                        worksheet.Range("C" + numCellStart + ":D" + numCellEnd).Merge().Style.Alignment.Vertical =
                            XLAlignmentVerticalValues.Center;
                        worksheet.Range("C" + numCellStart + ":D" + numCellEnd).Merge().Style.Alignment.Horizontal =
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
