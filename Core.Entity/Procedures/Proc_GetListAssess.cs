using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListAssess : IEntityProcView
    {
        public const string ProcName = "Proc_GetListAssess";

        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string Comment { get; set; }
        public int Value { get; set; }


        public Proc_GetListAssess()
		{
		}

        public static IEntityProc GetEntityProc(int courseId)
        {
            SqlParameter CourseId = new SqlParameter("@CourseId", courseId);
            return new EntityProc(
                $"{ProcName} @CourseId",
                new SqlParameter[] {
                    CourseId
                }
            );
        }
    }
}
