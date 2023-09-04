using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListTransactionEvent : IEntityProcView
    {
        public const string ProcName = "Proc_GetListTransactionEvent";

        [Key]
        public int TransactionId { get; set; }
        public int RowNum { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string ChapterName { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string StatusName { get; set; }
        public string ImagePath { get; set; }
        public string Note { get; set; }
        public DateTime? DateActive { get; set; }
        public bool Action { get; set; }
        public int Total { get; set; }


        public Proc_GetListTransactionEvent()
		{
		}

        public static IEntityProc GetEntityProc(int eventId, string keySearch, DateTime fromDate, DateTime toDate, int chapterId, int statusId, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";


            SqlParameter EventId = new SqlParameter("@EventId", eventId);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);
            SqlParameter ToDate = new SqlParameter("@ToDate", toDate);
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            SqlParameter StatusId = new SqlParameter("@StatusId", statusId);
            return new EntityProc(
                $"{ProcName} @EventId,@KeySearch,@FromDate,@ToDate,@ChapterId,@StatusId,@PageNum,@PageSize",
                new SqlParameter[] {
                    EventId,
                    KeySearch,
                    FromDate,
                    ToDate,
                    ChapterId,
                    StatusId,
                    PageNum,
                    PageSize
                }
            );
        }
    }
}
