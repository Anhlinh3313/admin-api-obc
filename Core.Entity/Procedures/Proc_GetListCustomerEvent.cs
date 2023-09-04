using System;
using Core.Entity.Abstract;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Core.Entity.Procedures
{
    public class Proc_GetListCustomerEvent : IEntityProcView
    {
        public const string ProcName = "Proc_GetListCustomerEvent";
        [Key]
        public int CustomerEventId { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string Position { get; set; }
        public string ChapterName { get; set; }
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public bool CheckIn { get; set; }
        public string Note { get; set; }
        public int Total { get; set; }


        public Proc_GetListCustomerEvent()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int eventId, int typeId, bool? status, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter EventId = new SqlParameter("@EventId", eventId);
            SqlParameter TypeId = new SqlParameter("@TypeId", typeId);
            SqlParameter Status = new SqlParameter("@Status", status);
            if (status == null) Status.Value = DBNull.Value;
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @EventId,@KeySearch,@TypeId,@Status, @PageNum, @PageSize",
                new SqlParameter[] {
                     EventId,KeySearch,TypeId, Status, PageNum, PageSize
                }
            );
        }
    }
}
