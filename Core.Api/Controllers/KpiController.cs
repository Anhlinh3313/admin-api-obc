using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Core.Business.Services.Abstract;
using Core.Business.Services.Models;
using Core.Business.ViewModels.Kpi;
using Core.Data.Abstract;
using Core.Entity.Entities;
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
    public class KpiController : BaseController
    {
        private readonly IKpiService _kpiService;

        public KpiController(
            IKpiService kpiService,
            Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOptions<AppSettings> optionsAccessor,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUnitOfWork unitOfWork
            ) : base(logger, optionsAccessor, jwtOptions, unitOfWork)
        {
            _kpiService = kpiService;
        }
       
        [HttpGet("GetKpiMobile")]
        public IActionResult GetKpiMobile( int monthFrom, int yearFrom, int monthTo, int yearTo)
        {
            var curentUserId = GetCurrentUserId();
            return _kpiService.GetKpiMobile(curentUserId,monthFrom,yearFrom, monthTo, yearTo);
        }

        [HttpGet("DownloadExcelKpi")]
        public IActionResult DownloadExcelKpi(int chapterId, DateTime? fromDate)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"KPI-roster-report_{DateTime.Now.ToString("dd/MM/yyy")} {DateTime.Now.ToString("HH:mm")}.xlsx";
            var userFullName = GetCurrentUser().FullName;
            var time = DateTime.Now.ToString("HH:mm dd-MM-yyyy");
            var chapter = _unitOfWork.RepositoryR<Chapter>().GetSingle(x => x.Id == chapterId);

            var data = _kpiService.GetKpiWeb(chapterId, fromDate.GetValueOrDefault(DateTime.Now));
            var success = data.Value.GetType().GetProperty("isSuccess")?.GetValue(data.Value, null);
            var isSuccess = (int)success;
            if (isSuccess == 0)
            {
                return data;
            }
            var value = data.Value.GetType().GetProperty("data")?.GetValue(data.Value, null);
            var list = (List<KpiViewModel>)value;

            KpiViewModel total = new KpiViewModel();

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                        workbook.Worksheets.Add("KPI");

                    worksheet.Style.Font.SetFontName("sans-serif").Font.FontSize = 10;

                    worksheet.Cell("A1").Value = "Chapter ► Summary PALMS Report";
                    //worksheet.Range("A1:M1").Merge().Style.Fill.BackgroundColor = XLColor.FromHtml("#b92719");
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

                    worksheet.Cell("A4").Value = "Thông số";
                    worksheet.Range("A4:P4").Merge();

                    worksheet.Cell("A5").Value = "Chapter:";
                    worksheet.Range("A5:H5").Merge();
                    worksheet.Cell("I5").Value = chapter.Name;
                    worksheet.Range("I5:P5").Merge();

                    worksheet.Cell("A6").Value = "Từ:";
                    worksheet.Range("A6:H6").Merge();
                    worksheet.Cell("I6").Value = "'" + fromDate.GetValueOrDefault(DateTime.Now).ToString("dd/MM/yyyy");
                    worksheet.Range("I6:P6").Merge();

                    worksheet.Cell("A7").Value = "Tới:";
                    worksheet.Range("A7:H7").Merge();
                    worksheet.Cell("I7").Value = "'" + DateTime.Now.ToString("dd/MM/yyyy");
                    worksheet.Range("I7:P7").Merge();

                    worksheet.Cell("A8").Value = "Họ và tên";
                    worksheet.Range("A8:B8").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("C8").Value = "P";
                    worksheet.Cell("D8").Value = "A";
                    worksheet.Cell("E8").Value = "L";
                    worksheet.Cell("F8").Value = "M";
                    worksheet.Cell("G8").Value = "S";
                    worksheet.Cell("H8").Value = "RGI";
                    worksheet.Range("H8:I8").Merge();
                    worksheet.Cell("J8").Value = "RGO";
                    worksheet.Cell("K8").Value = "RRI";
                    worksheet.Cell("L8").Value = "RRO";
                    worksheet.Cell("M8").Value = "V";
                    worksheet.Cell("N8").Value = "F-2-F";
                    worksheet.Cell("O8").Value = "TYFCB";
                    worksheet.Cell("P8").Value = "CEU";
                    worksheet.Range("C8:P8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell("R8").Value = "R";
                    worksheet.Cell("S8").Value = "F-2-F";
                    worksheet.Cell("T8").Value = "V";
                    worksheet.Range("R8:T8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range("R8:T8").Style.Font.SetBold();

                    worksheet.Cell("V8").Value = "#";
                    worksheet.Cell("W8").Value = "Member";
                    worksheet.Cell("X8").Value = "P";
                    worksheet.Cell("Y8").Value = "A";
                    worksheet.Cell("AA8").Value = "L";
                    worksheet.Cell("AC8").Value = "M";
                    worksheet.Cell("AD8").Value = "S";
                    worksheet.Cell("AE8").Value = "RG";
                    worksheet.Cell("AG8").Value = "RR";
                    worksheet.Cell("AH8").Value = "F-2-F";
                    worksheet.Cell("AJ8").Value = "V";
                    worksheet.Cell("AL8").Value = "T";
                    worksheet.Cell("AN8").Value = "TYFCB(VND)";
                    worksheet.Cell("AP8").Value = "Pts";

                    worksheet.Range("V8:AP8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range("V8:AP8").Style.Font.SetBold();
                    worksheet.Range("V8:AP8").Style.Font.SetFontColor(XLColor.White);
                    worksheet.Range("V8:AP8").Style.Fill.BackgroundColor = XLColor.FromHtml("#75140C");

                    for (int i = 0; i < list.Count; i++)
                    {
                        var rowNum = 9 + i;
                        total.P += list[i].P;
                        total.A += list[i].A;
                        total.L += list[i].L;
                        total.M += list[i].M;
                        total.S += list[i].S;
                        total.RGI += list[i].RGI;
                        total.RGO += list[i].RGO;
                        total.RRI += list[i].RRI;
                        total.RRO += list[i].RRO;
                        total.V += list[i].V;
                        total.F2F += list[i].F2F;
                        total.TYFCB += list[i].TYFCB;
                        total.CEU += list[i].CEU;

                        total.R += list[i].R;
                        total.AvgF2F += list[i].AvgF2F;
                        total.AvgV += list[i].AvgV;

                        total.PointA += list[i].PointA;
                        total.PointL += list[i].PointL;
                        total.RG += list[i].RG;
                        total.PointRG += list[i].PointRG;
                        total.RR += list[i].RR;
                        total.PointF2F += list[i].PointF2F;
                        total.PointV += list[i].PointV;
                        total.PointT += list[i].PointT;
                        total.PointTYFCB += list[i].PointTYFCB;
                        total.Pts += list[i].Pts;

                        worksheet.Cell("A" + rowNum).Value = list[i].FullName;
                        worksheet.Range("A"+rowNum+":B"+rowNum).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("C"+rowNum).Value = list[i].P;
                        worksheet.Cell("D"+rowNum).Value = list[i].A;
                        worksheet.Cell("E"+rowNum).Value = list[i].L;
                        worksheet.Cell("F"+ rowNum).Value = list[i].M;
                        worksheet.Cell("G"+rowNum).Value = list[i].S;
                        worksheet.Cell("H"+rowNum).Value = list[i].RGI;
                        worksheet.Range("H"+rowNum+":I"+rowNum).Merge();
                        worksheet.Cell("J"+rowNum).Value = list[i].RGO;
                        worksheet.Cell("K"+rowNum).Value = list[i].RRI;
                        worksheet.Cell("L"+rowNum).Value = list[i].RRO;
                        worksheet.Cell("M"+rowNum).Value = list[i].V;
                        worksheet.Cell("N"+rowNum).Value = list[i].F2F;
                        worksheet.Cell("O"+rowNum).Value = list[i].TYFCB;
                        worksheet.Cell("O" + rowNum).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell("P"+rowNum).Value = list[i].CEU;

                        worksheet.Cell("R"+rowNum).Value = list[i].R;
                        worksheet.Cell("S"+rowNum).Value = list[i].AvgF2F;
                        worksheet.Cell("T"+rowNum).Value = list[i].AvgV;

                        worksheet.Cell("V"+rowNum).Value = i + 1;
                        worksheet.Cell("V"+rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell("V" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("W"+rowNum).Value = list[i].FullName;
                        worksheet.Cell("W" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell("W" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("X"+rowNum).Value = list[i].P;
                        worksheet.Cell("X" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("X" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("Y"+rowNum).Value = list[i].A;
                        worksheet.Cell("Y" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("Y" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("Z" + rowNum).Value = list[i].PointA;
                        worksheet.Cell("Z" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("Z" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorA);
                        worksheet.Cell("AA"+rowNum).Value = list[i].L;
                        worksheet.Cell("AA" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AA" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AB" + rowNum).Value = list[i].PointL;
                        worksheet.Cell("AB" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AB" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorL);
                        worksheet.Cell("AC"+rowNum).Value = list[i].M;
                        worksheet.Cell("AC" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AC" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AD"+rowNum).Value = list[i].S;
                        worksheet.Cell("AD" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AD" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AE"+rowNum).Value = list[i].RG;
                        worksheet.Cell("AE" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AE" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AF" + rowNum).Value = list[i].PointRG;
                        worksheet.Cell("AF" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AF" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRG);
                        worksheet.Cell("AG"+rowNum).Value = list[i].RR;
                        worksheet.Cell("AG" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AG" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AH"+rowNum).Value = list[i].F2F;
                        worksheet.Cell("AH" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AH" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AI" + rowNum).Value = list[i].PointF2F;
                        worksheet.Cell("AI" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AI" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorF2F);
                        worksheet.Cell("AJ"+rowNum).Value = list[i].V;
                        worksheet.Cell("AJ" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AJ" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AK" + rowNum).Value = list[i].PointV;
                        worksheet.Cell("AK" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AK" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorV);
                        worksheet.Cell("AL"+rowNum).Value = list[i].CEU;
                        worksheet.Cell("AL" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AL" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AM" + rowNum).Value = list[i].PointT;
                        worksheet.Cell("AM" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AM" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorT);
                        worksheet.Cell("AN" + rowNum).Value = list[i].TYFCB;
                        worksheet.Cell("AN" + rowNum).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell("AN" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AN" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                        worksheet.Cell("AO" + rowNum).Value = list[i].PointTYFCB;
                        worksheet.Cell("AO" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AO" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorTYFCB);
                        worksheet.Cell("AP"+rowNum).Value = list[i].Pts;
                        worksheet.Cell("AP" + rowNum).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell("AP" + rowNum).Style.Fill.BackgroundColor = XLColor.FromHtml(list[i].ColorRow);
                    }

                    var newRow = 9 + list.Count;
                    worksheet.Cell("A" + newRow).Value = "Tổng số";
                    worksheet.Range("A" + newRow + ":B" + newRow).Merge();
                    worksheet.Cell("C" + newRow).Value = total.P;
                    worksheet.Cell("D" + newRow).Value = total.A;
                    worksheet.Cell("E" + newRow).Value = total.L;
                    worksheet.Cell("F" + newRow).Value = total.M;
                    worksheet.Cell("G" + newRow).Value = total.S;
                    worksheet.Cell("H" + newRow).Value = total.RGI;
                    worksheet.Range("H" + newRow + ":I" + newRow).Merge();
                    worksheet.Cell("J" + newRow).Value = total.RGO;
                    worksheet.Cell("K" + newRow).Value = total.RRI;
                    worksheet.Cell("L" + newRow).Value = total.RRO;
                    worksheet.Cell("M" + newRow).Value = total.V;
                    worksheet.Cell("N" + newRow).Value = total.F2F;
                    worksheet.Cell("O" + newRow).Value = total.TYFCB;
                    worksheet.Cell("O" + newRow).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell("P" + newRow).Value = total.CEU;

                    worksheet.Cell("R" + newRow).Value = total.R;
                    worksheet.Cell("S" + newRow).Value = total.AvgF2F;
                    worksheet.Cell("T" + newRow).Value = total.AvgV;

                    worksheet.Cell("V" + newRow).Value = list.Count + 1;
                    worksheet.Cell("V" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell("W" + newRow).Value = "Tổng số";
                    worksheet.Cell("X" + newRow).Value = total.P;
                    worksheet.Cell("X" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("Y" + newRow).Value = total.A;
                    worksheet.Cell("Y" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("Z" + newRow).Value = total.PointA;
                    worksheet.Cell("Z" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AA" + newRow).Value = total.L;
                    worksheet.Cell("AA" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AB" + newRow).Value = total.PointL;
                    worksheet.Cell("AB" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AC" + newRow).Value = total.M;
                    worksheet.Cell("AC" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AD" + newRow).Value = total.S;
                    worksheet.Cell("AD" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AE" + newRow).Value = total.RG;
                    worksheet.Cell("AE" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AF" + newRow).Value = total.PointRG;
                    worksheet.Cell("AF" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AG" + newRow).Value = total.RR;
                    worksheet.Cell("AG" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AH" + newRow).Value = total.F2F;
                    worksheet.Cell("AH" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AI" + newRow).Value = total.PointF2F;
                    worksheet.Cell("AI" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AJ" + newRow).Value = total.V;
                    worksheet.Cell("AJ" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AK" + newRow).Value = total.PointV;
                    worksheet.Cell("AK" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AL" + newRow).Value = total.CEU;
                    worksheet.Cell("AL" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AM" + newRow).Value = total.PointT;
                    worksheet.Cell("AM" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AN" + newRow).Value = total.TYFCB;
                    worksheet.Cell("AN" + newRow).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell("AN" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AO" + newRow).Value = total.PointTYFCB;
                    worksheet.Cell("AO" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell("AP" + newRow).Value = total.Pts;
                    worksheet.Cell("AP" + newRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("Z9:Z" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AB9:AB" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AF9:AF" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AI9:AI" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AK9:AK" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AM9:AM" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.BottomBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.BottomBorderColor = XLColor.White;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.TopBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.TopBorderColor = XLColor.White;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.RightBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.RightBorderColor = XLColor.White;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.LeftBorder =
                        XLBorderStyleValues.Thick;
                    worksheet.Range("AO9:AO" + (8 + list.Count)).Style.Border.LeftBorderColor = XLColor.White;

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
        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
