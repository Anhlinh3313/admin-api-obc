using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListThanks : IEntityProcView
    {
        public const string ProcName = "Proc_GetListThanks";
        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string ReceiverName { get; set; }
        public string Type { get; set; }
        public long Value { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string Note { get; set; }
        public int Total { get; set; }


        public Proc_GetListThanks()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, DateTime fromDate, DateTime toDate, string type, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);
            SqlParameter ToDate = new SqlParameter("@ToDate", toDate);
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter Type = new SqlParameter("@Type", type);
            if (string.IsNullOrEmpty(type)) Type.Value = "";
            return new EntityProc(
                $"{ProcName} @KeySearch, @FromDate, @ToDate, @Type, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch, FromDate, ToDate, Type, PageNum, PageSize
                }
            );
        }
    }
}
