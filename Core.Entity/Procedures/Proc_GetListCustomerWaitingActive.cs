using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListCustomerWaitingActive : IEntityProcView
    {
        public const string ProcName = "Proc_GetListCustomerWaitingActive";

        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string CodeTransaction { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string ChapterName { get; set; }
        public string ExpenseName { get; set; }
        public string StatusName { get; set; }
        public string ImagePath { get; set; }
        public string Note { get; set; }
        public DateTime CreatedWhen { get; set; }
        public DateTime? DateActive { get; set; }
        public bool Action { get; set; }
        public int Total { get; set; }


        public Proc_GetListCustomerWaitingActive()
		{
		}

        public static IEntityProc GetEntityProc(int transactionId, string keySearch, DateTime fromDate, DateTime toDate, int chapterId, int statusId, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);
            SqlParameter ToDate = new SqlParameter("@ToDate", toDate);
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            SqlParameter StatusId = new SqlParameter("@StatusId", statusId);
            SqlParameter TransactionId = new SqlParameter("@TransactionId", transactionId);


            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @TransactionId,@KeySearch,@FromDate,@ToDate,@ChapterId,@StatusId,@PageNum, @PageSize",
                new SqlParameter[] {
                    TransactionId,
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
