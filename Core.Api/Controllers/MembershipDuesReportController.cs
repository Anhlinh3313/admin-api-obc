using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.MembershipDuesReport;
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
    public class MembershipDuesReportController : BaseController
    {
        private readonly IMembershipDuesReportService _membershipDuesReportService;

        public MembershipDuesReportController(
            IMembershipDuesReportService membershipDuesReportService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _membershipDuesReportService = membershipDuesReportService;
        }
       
        [HttpGet("GetMembershipDuesReport")]
        public IActionResult GetMembershipDuesReport(int chapterId, DateTime? fromDate)
        {
            return _membershipDuesReportService.GetMembershipDuesReport(chapterId, fromDate.GetValueOrDefault(DateTime.Now.Date));
        }


        [HttpGet("DownloadExcelMembershipDuesReport")]
        public IActionResult DownloadExcelMembershipDuesReport(int chapterId, DateTime? fromDate)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "MembershipDuesReport.xlsx";
            var userFullName = GetCurrentUser().FullName;
            var time = DateTime.Now.ToString("HH:mm dd-MM-yyyy");
            var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);

            var data = _membershipDuesReportService.GetMembershipDuesReport(chapterId, fromDate.GetValueOrDefault(DateTime.Now.Date));
            var success = data.Value.GetType().GetProperty("isSuccess")?.GetValue(data.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return data;
            }
            var value = data.Value.GetType().GetProperty("data")?.GetValue(data.Value, null);
            var list = (MembershipDuesReportViewModel)value;
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                        workbook.Worksheets.Add("MembershipDuesReport");

                    worksheet.Style.Font.SetFontName("sans-serif").Font.FontSize = 10;

                    worksheet.Cell("A1").Value = "Report ► Membership Dues Report";
                    worksheet.Range("A1:M1").Merge();

                    worksheet.Cell("A2").Value = "Người dùng xuất báo cáo";
                    worksheet.Range("A2:C2").Merge();
                    worksheet.Cell("A3").Value = userFullName;
                    worksheet.Range("A3:C3").Merge();

                    worksheet.Cell("D2").Value = "Thời gian xuất báo cáo";
                    worksheet.Range("D2:F2").Merge();
                    worksheet.Cell("D3").Value = time;
                    worksheet.Range("D3:F3").Merge();

                    worksheet.Cell("G2").Value = "Quốc gia";
                    worksheet.Range("G2:J2").Merge();
                    worksheet.Cell("G3").Value = "Việt Nam";
                    worksheet.Range("G3:J3").Merge();

                    //worksheet.Cell("H2").Value = "Vùng";
                    //worksheet.Cell("H3").Value = region.Name;

                    //worksheet.Cell("I2").Value = "Chapter";
                    //worksheet.Cell("I3").Value = chapter.Name;

                    worksheet.Cell("A4").Value = "Chapter:";
                    worksheet.Range("A4:H4").Merge();
                    worksheet.Cell("I4").Value = chapter.Name;
                    worksheet.Range("I4:P4").Merge();

                    worksheet.Cell("A5").Value = "Ngày xuất báo cáo:";
                    worksheet.Range("A5:H5").Merge();
                    worksheet.Cell("I5").Value = "'" + time;
                    worksheet.Range("I5:P5").Merge();

                    worksheet.Cell("B6").Value = "Tên thành viên";
                    worksheet.Range("B6:G6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("H6").Value = "Ngành nghề";
                    worksheet.Range("H6:I6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("J6").Value = "Chức vụ";
                    worksheet.Range("J6:L6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("M6").Value = "Trạng thái thành viên";
                    worksheet.Range("M6:O6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("P6").Value = "Ngày hết hạn";
                    worksheet.Range("P6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (int i = 0; i < list.AllMember.Count; i++)
                    {
                        var numCell = 7 + i;

                        worksheet.Cell("A" + numCell).Value = i+1;
                        worksheet.Cell("A" + numCell).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell("B"+ numCell).Value = list.AllMember[i].FullName;
                        worksheet.Range("B" + numCell + ":G" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("H" + numCell).Value = list.AllMember[i].ProfessionName;
                        worksheet.Range("H" + numCell + ":I" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("J" + numCell).Value = list.AllMember[i].RoleName;
                        worksheet.Range("J" + numCell + ":L" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("M" + numCell).Value = list.AllMember[i].Status;
                        worksheet.Range("M" + numCell + ":O" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("P" + numCell).Value = "'" + list.AllMember[i].EndDate.ToString("dd-MM-yyyy");
                        worksheet.Cell("P" + numCell).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }

                    var newRowNewMember = list.AllMember.Count + 7;
                    worksheet.Cell("A" + newRowNewMember).Value = "Thành viên mới từ";
                    worksheet.Range("A" + newRowNewMember + ":P" + newRowNewMember).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("A" + (newRowNewMember + 1)).Value = fromDate.GetValueOrDefault(DateTime.Now.Date).ToString("dd-MM-yyyy");
                    worksheet.Range("A" + (newRowNewMember + 1) + ":P" + (newRowNewMember + 1)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell("B" + (newRowNewMember + 2)).Value = "Ngày bắt đầu";
                    worksheet.Range("B" + (newRowNewMember + 2) + ":D" + (newRowNewMember + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("E" + (newRowNewMember + 2)).Value = "Tên thành viên";
                    worksheet.Range("E" + (newRowNewMember + 2) + ":I" + (newRowNewMember + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("J" + (newRowNewMember + 2)).Value = "Ngành nghề";
                    worksheet.Range("J" + (newRowNewMember + 2) + ":L" + (newRowNewMember + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("M" + (newRowNewMember + 2)).Value = "Chức vụ";
                    worksheet.Range("M" + (newRowNewMember + 2) + ":O" + (newRowNewMember + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("P" + (newRowNewMember + 2)).Value = "Ngày hết hạn";
                    worksheet.Cell("P" + (newRowNewMember + 2)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < list.AllNewMember.Count; i++)
                    {
                        var numCell = newRowNewMember + 3 + i;

                        worksheet.Cell("A" + numCell).Value = i + 1;
                        worksheet.Cell("A" + numCell).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell("B" + numCell).Value = list.AllNewMember[i].DateJoin;
                        worksheet.Range("B" + numCell + ":D" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("E" + numCell).Value = list.AllNewMember[i].FullName;
                        worksheet.Range("E" + numCell + ":I" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("J" + numCell).Value = list.AllNewMember[i].ProfessionName;
                        worksheet.Range("J" + numCell + ":L" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("M" + numCell).Value = list.AllNewMember[i].RoleName;
                        worksheet.Range("M" + numCell + ":O" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("P" + numCell).Value = "'" + list.AllNewMember[i].EndDate.ToString("dd-MM-yyyy");
                        worksheet.Cell("P" + numCell).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }

                    var newRowMemberLate = list.AllNewMember.Count + newRowNewMember + 3;
                    worksheet.Cell("A" + newRowMemberLate).Value = "Thành viên trễ hạn từ";
                    worksheet.Range("A" + newRowMemberLate + ":P" + newRowMemberLate).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("A" + (newRowMemberLate + 1)).Value = fromDate.GetValueOrDefault(DateTime.Now.Date).ToString("dd-MM-yyyy");
                    worksheet.Range("A" + (newRowMemberLate + 1) + ":P" + (newRowMemberLate + 1)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell("C" + (newRowMemberLate + 2)).Value = "Tên thành viên";
                    worksheet.Range("C" + (newRowMemberLate + 2) + ":H" + (newRowMemberLate + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("I" + (newRowMemberLate + 2)).Value = "Ngành nghề";
                    worksheet.Range("I" + (newRowMemberLate + 2) + ":K" + (newRowMemberLate + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("L" + (newRowMemberLate + 2)).Value = "Ngày hết hạn";
                    worksheet.Range("L" + (newRowMemberLate + 2) + ":P" + (newRowMemberLate + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (int i = 0; i < list.AllMemberLate.Count; i++)
                    {
                        var numCell = newRowMemberLate + 3 + i;

                        worksheet.Cell("A" + numCell).Value = i + 1;
                        worksheet.Range("A" + numCell + ":B" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; 
                        worksheet.Cell("C" + numCell).Value = list.AllMemberLate[i].FullName;
                        worksheet.Range("C" + numCell + ":H" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("I" + numCell).Value = list.AllMemberLate[i].ProfessionName;
                        worksheet.Range("I" + numCell + ":K" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("L" + numCell).Value = "'" + list.AllMemberLate[i].EndDate.ToString("dd-MM-yyyy");
                        worksheet.Range("L" + numCell + ":P" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }

                    var newRowMemberExpend = list.AllMemberLate.Count + newRowMemberLate + 3;
                    worksheet.Cell("A" + newRowMemberExpend).Value = "Thành viên hết hạn/ rời tổ chức từ";
                    worksheet.Range("A" + newRowMemberExpend + ":P" + newRowMemberExpend).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("A" + (newRowMemberExpend + 1)).Value = fromDate.GetValueOrDefault(DateTime.Now.Date).ToString("dd-MM-yyyy");
                    worksheet.Range("A" + (newRowMemberExpend + 1) + ":P" + (newRowMemberExpend + 1)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell("B" + (newRowMemberExpend + 2)).Value = "Tên thành viên";
                    worksheet.Range("B" + (newRowMemberExpend + 2) + ":F" + (newRowMemberExpend + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("G" + (newRowMemberExpend + 2)).Value = "Ngành nghề";
                    worksheet.Range("G" + (newRowMemberExpend + 2) + ":J" + (newRowMemberExpend + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("K" + (newRowMemberExpend + 2)).Value = "Ngày hết hạn/ rời tổ chức";
                    worksheet.Range("K" + (newRowMemberExpend + 2) + ":M" + (newRowMemberExpend + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("N" + (newRowMemberExpend + 2)).Value = "Trạng thái thành viên";
                    worksheet.Range("N" + (newRowMemberExpend + 2) + ":P" + (newRowMemberExpend + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < list.AllMemberExpired.Count; i++)
                    {
                        var numCell = newRowMemberExpend + 3 + i;

                        worksheet.Cell("A" + numCell).Value = i + 1;
                        worksheet.Cell("A" + numCell).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell("B" + numCell).Value = list.AllMemberExpired[i].FullName;
                        worksheet.Range("B" + numCell + ":F" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("G" + numCell).Value = list.AllMemberExpired[i].ProfessionName;
                        worksheet.Range("G" + numCell + ":J" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("K" + numCell).Value = "'" + list.AllMemberExpired[i].DateOut.ToString("dd-MM-yyyy");
                        worksheet.Range("K" + numCell + ":M" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("N" + numCell).Value = list.AllMemberExpired[i].Status;
                        worksheet.Range("N" + numCell + ":P" + numCell).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
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
