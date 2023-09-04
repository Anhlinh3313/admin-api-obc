using System;
using Core.Entity.Abstract;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Core.Entity.Procedures
{
    public class Proc_GetListCustomerCourse : IEntityProcView
    {
        public const string ProcName = "Proc_GetListCustomerCourse";
        [Key]
        public int CustomerCourseId { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string Position { get; set; }
        public string ChapterName { get; set; }
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public string ImagePath { get; set; }
        public string Note { get; set; }
        public int Total { get; set; }


        public Proc_GetListCustomerCourse()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int courseId, int typeId, bool? status, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter CourseId = new SqlParameter("@CourseId", courseId);
            SqlParameter TypeId = new SqlParameter("@TypeId", typeId);
            SqlParameter Status = new SqlParameter("@Status", status);
            if (status == null) Status.Value = DBNull.Value;
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @CourseId,@KeySearch,@TypeId,@Status, @PageNum, @PageSize",
                new SqlParameter[] {
                     CourseId,KeySearch,TypeId, Status, PageNum, PageSize
                }
            );
        }
    }
}
