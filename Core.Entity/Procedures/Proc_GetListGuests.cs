using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListGuests : IEntityProcView
    {
        public const string ProcName = "Proc_GetListGuests";
        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string GuestsName { get; set; }
        public string EventName { get; set; }
        public string StatusName { get; set; }
        public string ChapterName { get; set; }
        public int Total { get; set; }


        public Proc_GetListGuests()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, DateTime fromDate, DateTime toDate, int statusId, int chapterId, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            SqlParameter StatusId = new SqlParameter("@StatusId", statusId);
            SqlParameter ToDate = new SqlParameter("@ToDate", toDate);
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            return new EntityProc(
                $"{ProcName} @KeySearch, @FromDate, @ToDate, @StatusId,@ChapterId, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch, FromDate, ToDate, StatusId,ChapterId, PageNum, PageSize
                }
            );
        }
    }
}
