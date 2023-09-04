using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListAssessMobile : IEntityProcView
    {
        public const string ProcName = "Proc_GetListAssessMobile";

        [Key]
        public int Id { get; set; }
        public long RowNum { get; set; }
        public string CustomerName { get; set; }
        public string AvatarPath { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string Comment { get; set; }
        public int Value { get; set; }
        public int Total { get; set; }


        public Proc_GetListAssessMobile()
		{
		}

        public static IEntityProc GetEntityProc(int courseId, int pageNum, int pageSize)
        {
            SqlParameter CourseId = new SqlParameter("@CourseId", courseId);
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            return new EntityProc(
                $"{ProcName} @CourseId,@PageNum,@PageSize",
                new SqlParameter[] {
                    CourseId,PageNum, PageSize
                }
            );
        }
    }
}
