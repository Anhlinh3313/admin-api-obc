using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListMemberChapter : IEntityProcView
    {
        public const string ProcName = "Proc_GetListMemberChapter";

        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string FieldOperationName { get; set; }
        public string ProfessionName { get; set; }
        public string Position { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime? DateJoin { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public int? TransactionId { get; set; }
        public int Total { get; set; }

        public Proc_GetListMemberChapter()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId, string keySearch, string fieldOperations, string status, int pageNum, int pageSize)
        {

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter FieldOperations = new SqlParameter("@FieldOperations", fieldOperations);
            if (string.IsNullOrEmpty(fieldOperations)) FieldOperations.Value = "";

            SqlParameter Status = new SqlParameter("@Status", status);
            if (string.IsNullOrEmpty(status)) Status.Value = "";

            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);

            return new EntityProc(
                $"{ProcName} @ChapterId,@KeySearch,@FieldOperations,@Status,@PageNum,@PageSize",
                new SqlParameter[] {
                    ChapterId,KeySearch, FieldOperations, Status, PageNum, PageSize
                }
            );
        }
    }
}
